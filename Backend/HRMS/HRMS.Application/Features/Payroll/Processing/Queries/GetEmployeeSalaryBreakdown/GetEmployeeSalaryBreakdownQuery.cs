using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Processing.Queries.GetEmployeeSalaryBreakdown;

/// <summary>
/// الحصول على تفاصيل دقيقة لراتب موظف محدد
/// Get detailed salary breakdown for specific employee
/// </summary>
public class GetEmployeeSalaryBreakdownQuery : IRequest<Result<SalaryBreakdownDto>>
{
    public int EmployeeId { get; set; }
    public int? Month { get; set; } // للحصول على تأثير الحضور لشهر محدد
    public int? Year { get; set; }
}

public class GetEmployeeSalaryBreakdownQueryHandler : IRequestHandler<GetEmployeeSalaryBreakdownQuery, Result<SalaryBreakdownDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeSalaryBreakdownQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SalaryBreakdownDto>> Handle(GetEmployeeSalaryBreakdownQuery request, CancellationToken cancellationToken)
    {
        var result = new SalaryBreakdownDto();

        // 1. معلومات الموظف الأساسية
        var employee = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Job)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null)
        {
            return Result<SalaryBreakdownDto>.Failure("الموظف غير موجود");
        }

        result.Employee = new EmployeeBasicInfo
        {
            EmployeeId = employee.EmployeeId,
            EmployeeCode = employee.EmployeeNumber,
            EmployeeNameAr = employee.FullNameAr ?? "Unknown",
            DepartmentName = employee.Department?.DeptNameAr,
            JobTitle = employee.Job?.JobTitleAr,
            Email = employee.Email,
            HireDate = employee.HireDate
        };

        // 2. هيكل الراتب
        var structure = await _context.SalaryStructures
            .Include(s => s.SalaryElement)
            .Where(s => s.EmployeeId == request.EmployeeId && s.IsActive == 1)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (!structure.Any())
        {
            return Result<SalaryBreakdownDto>.Failure("لا يوجد هيكل راتب فعال للموظف");
        }

        // الراتب الأساسي
        var basicElement = structure.FirstOrDefault(s => s.SalaryElement.IsBasic == 1);
        result.BasicSalary = basicElement?.Amount ?? 0;

        // البدلات
        var earningElements = structure.Where(s => s.SalaryElement.ElementType == "EARNING" && s.SalaryElement.IsBasic == 0).ToList();
        foreach (var item in earningElements)
        {
            result.Earnings.Add(new ElementBreakdown
            {
                ElementId = item.SalaryElement.ElementId,
                ElementNameAr = item.SalaryElement.ElementNameAr,
                ElementType = "EARNING",
                Amount = item.Amount,
                Percentage = item.SalaryElement.DefaultPercentage,
                IsTaxable = item.SalaryElement.IsTaxable == 1,
                IsGosiBase = item.SalaryElement.IsGosiBase == 1,
                IsRecurring = item.SalaryElement.IsRecurring == 1
            });
        }

        // الاستقطاعات
        var deductionElements = structure.Where(s => s.SalaryElement.ElementType == "DEDUCTION").ToList();
        foreach (var item in deductionElements)
        {
            result.Deductions.Add(new ElementBreakdown
            {
                ElementId = item.SalaryElement.ElementId,
                ElementNameAr = item.SalaryElement.ElementNameAr,
                ElementType = "DEDUCTION",
                Amount = item.Amount,
                Percentage = item.SalaryElement.DefaultPercentage,
                IsTaxable = item.SalaryElement.IsTaxable == 1,
                IsGosiBase = item.SalaryElement.IsGosiBase == 1,
                IsRecurring = item.SalaryElement.IsRecurring == 1
            });
        }

        // إضافة GOSI تلقائياً إذا لم يكن موجود
        if (!structure.Any(s => s.SalaryElement.ElementNameAr.Contains("تأمينات")))
        {
            decimal gosiAmount = Math.Round(result.BasicSalary * 0.09m, 2);
            result.Deductions.Add(new ElementBreakdown
            {
                ElementId = 0,
                ElementNameAr = "تأمينات اجتماعية (GOSI)",
                ElementType = "DEDUCTION",
                Amount = gosiAmount,
                Percentage = 9,
                IsTaxable = false,
                IsGosiBase = true,
                IsRecurring = true
            });
        }

        // 3. السلف النشطة
        var activeLoans = await _context.Loans
            .Where(l => l.EmployeeId == request.EmployeeId && l.Status == "ACTIVE")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var loan in activeLoans)
        {
            var monthlyInstallment = loan.InstallmentCount > 0 ? loan.LoanAmount / loan.InstallmentCount : 0;
            var remainingInstallments = await _context.LoanInstallments
                .CountAsync(i => i.LoanId == loan.LoanId && i.IsPaid == 0, cancellationToken);

            result.ActiveLoans.Add(new LoanDeduction
            {
                LoanId = loan.LoanId,
                TotalAmount = loan.LoanAmount,
                MonthlyInstallment = monthlyInstallment,
                RemainingAmount = loan.LoanAmount,
                RemainingInstallments = remainingInstallments,
                Status = loan.Status
            });
        }

        // 4. تأثير الحضور (إذا تم تحديد شهر/سنة)
        if (request.Month.HasValue && request.Year.HasValue)
        {
            var startDate = new DateTime(request.Year.Value, request.Month.Value, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var dailyAttendances = await _context.DailyAttendances
                .Where(d => d.EmployeeId == request.EmployeeId 
                         && d.AttendanceDate >= startDate 
                         && d.AttendanceDate <= endDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (dailyAttendances.Any())
            {
                decimal dailyRate = result.BasicSalary / 30m;
                decimal hourlyRate = dailyRate / 8m;
                decimal minuteRate = hourlyRate / 60m;

                int absenceDays = dailyAttendances.Count(d => d.Status == "ABSENT");
                int lateMinutes = dailyAttendances.Sum(d => d.LateMinutes);
                int overtimeMinutes = dailyAttendances.Sum(d => d.OvertimeMinutes);

                result.Attendance = new AttendanceImpact
                {
                    Month = request.Month.Value,
                    Year = request.Year.Value,
                    TotalWorkingDays = dailyAttendances.Count,
                    ActualWorkingDays = dailyAttendances.Count(d => d.Status == "PRESENT" || d.Status == "LATE"),
                    AbsenceDays = absenceDays,
                    AbsenceDeduction = Math.Round(absenceDays * dailyRate, 2),
                    LateMinutes = lateMinutes,
                    LatenessDeduction = Math.Round(lateMinutes * minuteRate, 2),
                    OvertimeHours = Math.Round(overtimeMinutes / 60m, 2),
                    OvertimeAmount = Math.Round(overtimeMinutes * minuteRate * 1.5m, 2)
                };
            }
        }

        // 5. الملخص
        decimal totalEarnings = result.BasicSalary + result.Earnings.Sum(e => e.Amount);
        decimal totalDeductions = result.Deductions.Sum(d => d.Amount);
        decimal loanDeductions = result.ActiveLoans.Sum(l => l.MonthlyInstallment);
        decimal attendanceDeductions = 0;

        if (result.Attendance != null)
        {
            totalEarnings += result.Attendance.OvertimeAmount;
            attendanceDeductions = result.Attendance.AbsenceDeduction + result.Attendance.LatenessDeduction;
        }

        result.Summary = new SalarySummary
        {
            GrossSalary = result.BasicSalary + result.Earnings.Sum(e => e.Amount),
            TotalEarnings = totalEarnings,
            TotalDeductions = totalDeductions + loanDeductions + attendanceDeductions,
            NetSalary = totalEarnings - (totalDeductions + loanDeductions + attendanceDeductions),
            StructureDeductions = totalDeductions,
            LoanDeductions = loanDeductions,
            AttendanceDeductions = attendanceDeductions,
            OtherDeductions = 0
        };

        return Result<SalaryBreakdownDto>.Success(result);
    }
}
