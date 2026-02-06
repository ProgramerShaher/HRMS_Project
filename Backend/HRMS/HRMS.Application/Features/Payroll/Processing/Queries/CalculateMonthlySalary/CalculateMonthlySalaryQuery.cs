using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.Features.Payroll.Processing.Services;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Processing.Queries.CalculateMonthlySalary;

public class CalculateMonthlySalaryQuery : IRequest<Result<MonthlySalaryCalculationDto>>
{
    public int EmployeeId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

public class CalculateMonthlySalaryQueryHandler : IRequestHandler<CalculateMonthlySalaryQuery, Result<MonthlySalaryCalculationDto>>
{
	private readonly IApplicationDbContext _context;
	private readonly AttendanceAggregatorService _attendanceAggregator; // 👈 إضافة خدمة الحضور

	public CalculateMonthlySalaryQueryHandler(IApplicationDbContext context, AttendanceAggregatorService attendanceAggregator)
	{
		_context = context;
		_attendanceAggregator = attendanceAggregator;
	}

	public async Task<Result<MonthlySalaryCalculationDto>> Handle(CalculateMonthlySalaryQuery request, CancellationToken cancellationToken)
	{
		var result = new MonthlySalaryCalculationDto { EmployeeId = request.EmployeeId };

		// ═══════════════════════════════════════════════════════════
		// 1. جلب هيكل الراتب (الأساس)
		// ═══════════════════════════════════════════════════════════
		var structure = await _context.SalaryStructures
			.Include(s => s.SalaryElement)
			.Where(s => s.EmployeeId == request.EmployeeId && s.IsActive == 1)
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		if (!structure.Any()) return Result<MonthlySalaryCalculationDto>.Failure("لا يوجد هيكل راتب للموظف");

		var employee = await _context.Employees
			.Include(e => e.Job)
			.Include(e => e.Department)
			.FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

		result.EmployeeName = employee?.FullNameAr ?? "Unknown";
		// يمكن إضافة المسمى الوظيفي والقسم للعرض
		// result.JobTitle = employee?.Job?.JobTitleAr;

		// تحديد الراتب الأساسي
		var basicElement = structure.FirstOrDefault(s => s.SalaryElement.IsBasic == 1);
		result.BasicSalary = basicElement?.Amount ?? 0;

		// جمع البدلات الثابتة
		result.TotalAllowances = structure.Where(s => s.SalaryElement.ElementType == "EARNING" && s.SalaryElement.IsBasic == 0).Sum(s => s.Amount);

		// جمع الاستقطاعات الثابتة (ضرائب، تأمين صحي خاص...)
		result.TotalStructureDeductions = structure.Where(s => s.SalaryElement.ElementType == "DEDUCTION").Sum(s => s.Amount);

		// --- حساب التأمينات الاجتماعية (GOSI) ---
		// إذا لم تكن مضافة يدوياً، نحسبها آلياً (مثلاً 9% أو حسب القانون)
		if (!structure.Any(s => s.SalaryElement.ElementNameAr.Contains("تأمينات") || s.SalaryElement.ElementType.Contains("GOSI")))
		{
			// معادلة: 9% من (الأساسي + بدل السكن عادةً)
			// للتبسيط هنا نحسبها من الأساسي، ويمكنك تعديلها
			decimal autoGosi = Math.Round(result.BasicSalary * 0.09m, 2);
			result.TotalStructureDeductions += autoGosi;
		}

		// ═══════════════════════════════════════════════════════════
		// 2. جلب القروض (Loans)
		// ═══════════════════════════════════════════════════════════
		var startDate = new DateTime(request.Year, request.Month, 1);
		var endDate = startDate.AddMonths(1).AddDays(-1);

		var installments = await _context.LoanInstallments
			.Where(i => i.Loan.EmployeeId == request.EmployeeId
					 && i.DueDate >= startDate && i.DueDate <= endDate
					 && i.IsPaid == 0)
			.ToListAsync(cancellationToken);

		result.LoanDeductions = installments.Sum(i => i.Amount);
		result.PaidInstallmentIds = installments.Select(i => i.InstallmentId).ToList();

		// ═══════════════════════════════════════════════════════════
		// 3. جلب التعديلات اليدوية والمخالفات (PAYROLL_ADJUSTMENTS)
		// 🔥 هذا هو الجزء الذي سيخصم مخالفة رهف (11,666)
		// ═══════════════════════════════════════════════════════════
		var adjustments = await _context.PayrollAdjustments
			.Where(a => a.EmployeeId == request.EmployeeId
					 && a.CreatedAt.Month == request.Month
					 && a.CreatedAt.Year == request.Year
					 && a.IsDeleted == 0) // افتراض وجود Soft Delete
			.ToListAsync(cancellationToken);

		decimal manualDeductions = adjustments
			.Where(a => a.AdjustmentType == "DEDUCTION" || a.AdjustmentType == "VIOLATION") // تأكد من الاسم في جدولك
			.Sum(a => a.Amount);

		decimal manualBonuses = adjustments
			.Where(a => a.AdjustmentType == "BONUS" || a.AdjustmentType == "REWARD")
			.Sum(a => a.Amount);

		// إضافة القيم للمجاميع العامة
		result.TotalStructureDeductions += manualDeductions;
		result.TotalAllowances += manualBonuses;

		// ═══════════════════════════════════════════════════════════
		// 4. جلب تأثير الحضور والغياب (Attendance Integration)
		// ═══════════════════════════════════════════════════════════
		// نستدعي الخدمة التي تحسب التأخير والغياب والإضافي من البصمات
		var attendanceImpact = await _attendanceAggregator.CalculateAttendanceImpactAsync(
			request.EmployeeId, startDate, endDate, result.BasicSalary, cancellationToken);

		result.AttendancePenalties = attendanceImpact.AttendancePenalties; // قيمة خصم التأخير والغياب
		result.OvertimeEarnings = attendanceImpact.OvertimeEarnings;       // قيمة الإضافي

		result.TotalLateMinutes = attendanceImpact.TotalLateMinutes;
		result.AbsenceDays = attendanceImpact.AbsenceDays;
		result.TotalOvertimeMinutes = attendanceImpact.TotalOvertimeMinutes;

		if (attendanceImpact.IsBlocked)
		{
			result.Warnings.AddRange(attendanceImpact.Warnings);
			// يمكنك هنا إرجاع Failure إذا أردت منع الراتب في حال وجود بصمات مفقودة
			// return Result<MonthlySalaryCalculationDto>.Failure("لا يمكن حساب الراتب: سجلات الحضور غير مكتملة");
		}

		// ═══════════════════════════════════════════════════════════
		// 5. الحسبة النهائية (Net Salary Formula)
		// ═══════════════════════════════════════════════════════════
		// المعادلة: (الاستحقاقات + الإضافي) - (الاستقطاعات الهيكلية + القروض + جزاءات الحضور)

		var totalEarnings = result.BasicSalary + result.TotalAllowances + result.OvertimeEarnings;
		var totalDeductions = result.TotalStructureDeductions + result.LoanDeductions + result.AttendancePenalties;

		result.NetSalary = totalEarnings - totalDeductions;

		return Result<MonthlySalaryCalculationDto>.Success(result);
	}
}