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

    public CalculateMonthlySalaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
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

        // 3. Attendance Penalties (Simplified Logic)
        // In a real scenario, we'd inject IMediator and call GetMonthlyPayrollSummaryQuery
        // For this task, we calculate directly from DailyAttendance
        var absences = await _context.DailyAttendances
            .Where(d => d.EmployeeId == request.EmployeeId 
                     && d.AttendanceDate >= startDate && d.AttendanceDate <= endDate
                     && (d.Status == "ABSENT" || d.Status == "MISSING_PUNCH"))
            .CountAsync(cancellationToken);

        result.AbsenceDays = absences;
        
        // Penalty Formula: (Basic Salary / 30) * Absence Days
        if (result.BasicSalary > 0)
        {
            decimal dailyRate = result.BasicSalary / 30;
            result.AttendancePenalties = Math.Round(dailyRate * absences, 2);
        }

        return Result<MonthlySalaryCalculationDto>.Success(result);
    }
}
