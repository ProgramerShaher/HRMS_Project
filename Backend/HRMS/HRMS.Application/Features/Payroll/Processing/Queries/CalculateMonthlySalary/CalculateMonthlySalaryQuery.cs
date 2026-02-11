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
    private readonly AttendanceAggregatorService _attendanceAggregator;

    public CalculateMonthlySalaryQueryHandler(IApplicationDbContext context, AttendanceAggregatorService attendanceAggregator)
    {
        _context = context;
        _attendanceAggregator = attendanceAggregator;
    }

    public async Task<Result<MonthlySalaryCalculationDto>> Handle(CalculateMonthlySalaryQuery request, CancellationToken cancellationToken)
    {
        var result = new MonthlySalaryCalculationDto { EmployeeId = request.EmployeeId };

        // ═══════════════════════════════════════════════════════════
        // 1. التحقق من وجود الموظف وهيكل الراتب
        // ═══════════════════════════════════════════════════════════
        var employee = await _context.Employees
            .Include(e => e.Job)
            .Include(e => e.Department)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null) return Result<MonthlySalaryCalculationDto>.Failure("الموظف غير موجود");

        result.EmployeeName = employee.FullNameAr ?? "Unknown";

        // جلب هيكل الراتب الفعال
        var structure = await _context.SalaryStructures
            .Include(s => s.SalaryElement)
            .Where(s => s.EmployeeId == request.EmployeeId && s.IsActive == 1)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (!structure.Any()) 
            return Result<MonthlySalaryCalculationDto>.Failure($"لا يوجد هيكل راتب فعال للموظف {result.EmployeeName}");

        // ═══════════════════════════════════════════════════════════
        // 2. حساب الأساسيات (Basic + Allowances)
        // ═══════════════════════════════════════════════════════════
        
        // الراتب الأساسي
        var basicElement = structure.FirstOrDefault(s => s.SalaryElement.IsBasic == 1);
        result.BasicSalary = basicElement?.Amount ?? 0;
        
        if (basicElement != null)
        {
            result.Details.Add(new SalaryDetailItem 
            { 
                NameAr = "الراتب الأساسي", 
                NameEn = "Basic Salary", 
                Amount = basicElement.Amount, 
                Type = "EARNING", 
                Reference = "BASIC",
                ElementId = basicElement.SalaryElement.ElementId
            });
        }

        // البدلات الثابتة (سكن، نقل، إلخ)
        var earningElements = structure.Where(s => s.SalaryElement.ElementType == "EARNING" && s.SalaryElement.IsBasic == 0).ToList();
        result.TotalAllowances = earningElements.Sum(s => s.Amount);
        
        foreach (var item in earningElements)
        {
            result.Details.Add(new SalaryDetailItem
            {
                NameAr = item.SalaryElement.ElementNameAr,
                NameEn = item.SalaryElement.ElementNameAr, // Dictionary lookup in future
                Amount = item.Amount,
                Type = "EARNING",
                Reference = "ALLOWANCE",
                ElementId = item.SalaryElement.ElementId
            });
        }

        // الاستقطاعات الثابتة (تأمينات، ضرائب)
        var deductionElements = structure.Where(s => s.SalaryElement.ElementType == "DEDUCTION").ToList();
        result.TotalStructureDeductions = deductionElements.Sum(s => s.Amount);

        foreach (var item in deductionElements)
        {
            result.Details.Add(new SalaryDetailItem
            {
                NameAr = item.SalaryElement.ElementNameAr,
                NameEn = item.SalaryElement.ElementNameAr,
                Amount = item.Amount,
                Type = "DEDUCTION",
                Reference = "STRUCTURE_DEDUCTION",
                ElementId = item.SalaryElement.ElementId
            });
        }

        // --- التأمينات الاجتماعية (GOSI) - إذا لم تكن مضافة ---
        if (!structure.Any(s => s.SalaryElement.ElementNameAr.Contains("تأمينات") || s.SalaryElement.ElementType.Contains("GOSI")))
        {
            // معادلة تقريبية: 9%
            decimal autoGosi = Math.Round(result.BasicSalary * 0.09m, 2);
            result.TotalStructureDeductions += autoGosi;
            
            result.Details.Add(new SalaryDetailItem
            {
                NameAr = "تأمينات اجتماعية (GOSI)",
                NameEn = "GOSI",
                Amount = autoGosi,
                Type = "DEDUCTION",
                Reference = "AUTO_GOSI"
            });
        }

        // ═══════════════════════════════════════════════════════════
        // 3. القروض (Loans)
        // ═══════════════════════════════════════════════════════════
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var installments = await _context.LoanInstallments
            .Include(i => i.Loan)
            .Where(i => i.Loan.EmployeeId == request.EmployeeId
                      && i.DueDate >= startDate && i.DueDate <= endDate
                      && i.IsPaid == 0)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        result.LoanDeductions = installments.Sum(i => i.Amount);
        result.PaidInstallmentIds = installments.Select(i => i.InstallmentId).ToList();
        
        foreach (var inst in installments)
        {
            result.Details.Add(new SalaryDetailItem
            {
                NameAr = $"قسط قرض - سلفة #{inst.Loan.LoanId}",
                NameEn = $"Loan Installment - Advance #{inst.Loan.LoanId}",
                Amount = inst.Amount,
                Type = "DEDUCTION",
                Reference = "LOAN"
            });
        }

        // ═══════════════════════════════════════════════════════════
        // 4. التسويات المالية والمخالفات (Payroll Adjustments)
        // ═══════════════════════════════════════════════════════════
        
        // Using Raw SQL to guarantee retrieval
        var adjustments = await _context.Database
            .SqlQuery<PayrollAdjustmentRawDto>($@"
                SELECT 
                    AdjustmentId as AdjustmentId,
                    EMPLOYEE_ID as EmployeeId,
                    AdjustmentType as AdjustmentType,
                    Amount as Amount,
                    Reason as Reason
                FROM [HR_PAYROLL].[PAYROLL_ADJUSTMENTS]
                WHERE EMPLOYEE_ID = {request.EmployeeId}
                    AND YEAR(CREATED_AT) = {request.Year}
                    AND MONTH(CREATED_AT) = {request.Month}
                    AND (IS_DELETED = 0 OR IS_DELETED IS NULL)
            ")
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        decimal totalViolations = 0;
        decimal otherDeductions = 0;
        decimal manualEarnings = 0;

        foreach (var adj in adjustments)
        {
            string typeUpper = adj.AdjustmentType?.ToUpper() ?? "";
            
            if (typeUpper == "VIOLATION")
            {
                totalViolations += adj.Amount;
                result.Details.Add(new SalaryDetailItem
                {
                    NameAr = adj.Reason ?? "مخالفة",
                    NameEn = adj.Reason ?? "Violation",
                    Amount = adj.Amount,
                    Type = "DEDUCTION",
                    Reference = "VIOLATION"
                });
            }
            else if (typeUpper == "DEDUCTION")
            {
                otherDeductions += adj.Amount;
                result.Details.Add(new SalaryDetailItem
                {
                    NameAr = adj.Reason ?? "خصم إداري",
                    NameEn = adj.Reason ?? "Admin Deduction",
                    Amount = adj.Amount,
                    Type = "DEDUCTION",
                    Reference = "OTHER_DEDUCTION"
                });
            }
            else if (typeUpper == "EARNING" || typeUpper == "BONUS" || typeUpper == "REWARD")
            {
                manualEarnings += adj.Amount;
                result.Details.Add(new SalaryDetailItem
                {
                    NameAr = adj.Reason ?? "مكافأة",
                    NameEn = adj.Reason ?? "Bonus/Reward",
                    Amount = adj.Amount,
                    Type = "EARNING",
                    Reference = "BONUS"
                });
            }
        }

        decimal manualDeductions = totalViolations + otherDeductions;
        
        result.TotalViolations = totalViolations;
        result.OtherDeductions = otherDeductions;

        // ═══════════════════════════════════════════════════════════
        // 5. التكامل مع الحضور (Attendance)
        // ═══════════════════════════════════════════════════════════
        
        var attendanceImpact = await _attendanceAggregator.CalculateAttendanceImpactAsync(
            request.EmployeeId, startDate, endDate, result.BasicSalary, cancellationToken);

        result.TotalLateMinutes = attendanceImpact.TotalLateMinutes;
        result.AbsenceDays = attendanceImpact.AbsenceDays;
        result.TotalOvertimeMinutes = attendanceImpact.TotalOvertimeMinutes;
        result.Warnings.AddRange(attendanceImpact.Warnings);

        bool isAttendanceAlreadyProcessed = adjustments.Any(a => 
            (a.Reason != null && (a.Reason.Contains("Late") || a.Reason.Contains("تأخير") || a.Reason.Contains("Overtime")))
        );

        if (!isAttendanceAlreadyProcessed)
        {
            result.AttendancePenalties = attendanceImpact.AttendancePenalties; 
            result.OvertimeEarnings = attendanceImpact.OvertimeEarnings;       
            
            if (result.AttendancePenalties > 0)
            {
                result.Details.Add(new SalaryDetailItem
                {
                    NameAr = $"خصم حضور (تأخير/غياب) - {result.AbsenceDays} يوم، {result.TotalLateMinutes} دقيقة",
                    NameEn = $"Attendance Penalty - {result.AbsenceDays} days, {result.TotalLateMinutes} min",
                    Amount = result.AttendancePenalties,
                    Type = "DEDUCTION",
                    Reference = "ATTENDANCE_PENALTY"
                });
            }

            if (result.OvertimeEarnings > 0)
            {
                result.Details.Add(new SalaryDetailItem
                {
                    NameAr = $"ساعات عمل إضافية - {result.TotalOvertimeMinutes} دقيقة",
                    NameEn = $"Overtime Earnings - {result.TotalOvertimeMinutes} min",
                    Amount = result.OvertimeEarnings,
                    Type = "EARNING",
                    Reference = "OVERTIME"
                });
            }
        }
        else
        {
            result.AttendancePenalties = 0;
            result.OvertimeEarnings = 0;
        }

        // ═══════════════════════════════════════════════════════════
        // 6. الحسبة النهائية (Net Salary Formula)
        // ═══════════════════════════════════════════════════════════

        // الإضافات = (الأساسي + البدلات + الإضافي اليدوي + الإضافي المحسوب)
        var totalEarnings = result.BasicSalary + result.TotalAllowances + manualEarnings + result.OvertimeEarnings;
        
        // الخصومات = (الهيكلية + القروض + الخصومات اليدوية + خصومات الحضور المحسوبة)
        var totalDeductions = result.TotalStructureDeductions + result.LoanDeductions + manualDeductions + result.AttendancePenalties;

        result.NetSalary = totalEarnings - totalDeductions;

        // تعبئة حقول العرض (للتقرير)
        // تم فصل الخصومات اليدوية في حقول خاصة (TotalViolations, OtherDeductions) بدلاً من دمجها
        result.TotalAllowances += manualEarnings;

        return Result<MonthlySalaryCalculationDto>.Success(result);
    }

    #region Internal DTOs for Raw SQL Queries

    /// <summary>
    /// DTO for PAYROLL_ADJUSTMENTS raw SQL query
    /// Used to bypass EF Core translation issues and guarantee case-insensitive retrieval
    /// </summary>
    internal class PayrollAdjustmentRawDto
    {
        public int AdjustmentId { get; set; }
        public int EmployeeId { get; set; }
        public string? AdjustmentType { get; set; }
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
    }

    #endregion
}
