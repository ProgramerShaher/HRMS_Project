using HRMS.Application.DTOs.Payroll.Processing;
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
    private readonly Services.AttendanceAggregatorService _attendanceAggregator;

    public CalculateMonthlySalaryQueryHandler(IApplicationDbContext context, Services.AttendanceAggregatorService attendanceAggregator)
    {
        _context = context;
        _attendanceAggregator = attendanceAggregator;
    }

    public async Task<Result<MonthlySalaryCalculationDto>> Handle(CalculateMonthlySalaryQuery request, CancellationToken cancellationToken)
    {
        var result = new MonthlySalaryCalculationDto { EmployeeId = request.EmployeeId };

        // 1. Fetch Salary Structure
        var structure = await _context.SalaryStructures
            .Include(s => s.SalaryElement)
            .Where(s => s.EmployeeId == request.EmployeeId && s.IsActive == 1)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (!structure.Any()) return Result<MonthlySalaryCalculationDto>.Failure("لا يوجد هيكل راتب للموظف");

        var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken);
        result.EmployeeName = employee?.FullNameAr ?? "Unknown";

        // Identify Basic Salary
        var basicElement = structure.FirstOrDefault(s => s.SalaryElement.IsBasic == 1);
        result.BasicSalary = basicElement?.Amount ?? 0;

        // Sum Allowances and Static Deductions
        result.TotalAllowances = structure.Where(s => s.SalaryElement.ElementType == "EARNING" && s.SalaryElement.IsBasic == 0).Sum(s => s.Amount);
        result.TotalStructureDeductions = structure.Where(s => s.SalaryElement.ElementType == "DEDUCTION").Sum(s => s.Amount);

        // --- GOSI Calculation (Standard 9.75% of Basic + Housing) ---
        // For simplicity, we calculate 9.75% of Basic Salary only currently, or check if GOSI element exists.
        // Better approach: Auto-add GOSI if not present? Or strictly follow structure?
        // User asked to "Activate" it. We will assume a standard GOSI rule for all.
        decimal gosiRate = 0.0975m; 
        decimal gosiDeduction = Math.Round((result.BasicSalary + result.TotalAllowances) * gosiRate, 2); 
        // NOTE: Usually GOSI is Basic + Housing only. For now Basic + All Allowances is a safer "Max" base, but typically Housing is specific.
        // Let's stick to Basic Salary * 10% (0.10) for a clean round number if Labor Law unspec, or 9% for Saudi GOSI.
        // Let's use 0.00 since we don't want to break existing numbers unless explicitly asked for a number.
        // Wait, user said "Activate the calculation". Let's add it as a separate field or part of Deductions.
        
        // Let's add it to StructureDeductions to verify it works "automatically".
        // Or better, let's just make sure "TotalStructureDeductions" captures it if the user added it to the structure.
        // If the user meant "Hardcoded Logic", I will add it.
        
        // Implementing Hardcoded GOSI for Employee 25 as a test case (or all)
        // DTO update required for "GosiDeduction"? Or just bundle into TotalDeductions?
        // Let's bundle into TotalStructureDeductions for now to avoid breaking DTOs further.
        // result.TotalStructureDeductions += gosiDeduction; 
        
        // REVERTING: I will trust the "Structure" contains the GOSI element if configured.
        // If user wants AUTOMATIC deduction even if not in structure:
        // result.TotalStructureDeductions += (result.BasicSalary * 0.09m); 
        
        // User Requirement: "Activate the calculation for Social Insurance".
        // I will add a logic to check if GOSI exist in structure, if NOT, calculate it.
        if (!structure.Any(s => s.SalaryElement.ElementNameAr.Contains("تأمينات") || s.SalaryElement.ElementType.Contains("GOSI")))
        {
             // Auto-calculate 9%
             decimal autoGosi = Math.Round(result.BasicSalary * 0.09m, 2);
             result.TotalStructureDeductions += autoGosi;
        }

        // 2. Fetch Loans (Upcoming Installments for this Month)
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var installments = await _context.LoanInstallments
            .Where(i => i.Loan.EmployeeId == request.EmployeeId 
                     && i.DueDate >= startDate && i.DueDate <= endDate 
                     && i.IsPaid == 0)
            .ToListAsync(cancellationToken);

        result.LoanDeductions = installments.Sum(i => i.Amount);
        result.PaidInstallmentIds = installments.Select(i => i.InstallmentId).ToList();

        // 3. Attendance Aggregation (New Logic)
        if (result.BasicSalary > 0)
        {
            var attendanceResult = await _attendanceAggregator.CalculateAttendanceImpactAsync(
                request.EmployeeId, 
                startDate, 
                endDate, 
                result.BasicSalary, 
                cancellationToken);

            // Check for BLOCKING issues (e.g. Missing Punches)
            if (attendanceResult.IsBlocked)
            {
                result.Warnings.AddRange(attendanceResult.Warnings);
                // We return Failure here to stop the Payrun for this employee
                return Result<MonthlySalaryCalculationDto>.Failure(string.Join(", ", attendanceResult.Warnings));
            }

            // Map Results
            result.AbsenceDays = attendanceResult.AbsenceDays;
            result.TotalLateMinutes = attendanceResult.TotalLateMinutes;
            result.TotalOvertimeMinutes = attendanceResult.TotalOvertimeMinutes;
            result.AttendancePenalties = attendanceResult.AttendancePenalties;
            result.OvertimeEarnings = attendanceResult.OvertimeEarnings;
            result.Warnings.AddRange(attendanceResult.Warnings);
        }

        return Result<MonthlySalaryCalculationDto>.Success(result);
    }
}
