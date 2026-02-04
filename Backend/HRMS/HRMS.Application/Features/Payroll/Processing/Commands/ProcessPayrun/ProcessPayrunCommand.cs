using HRMS.Application.Features.Payroll.Processing.Queries.CalculateMonthlySalary;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Processing.Commands.ProcessPayrun;

public class ProcessPayrunCommand : IRequest<Result<int>>
{
    public int Month { get; set; }
    public int Year { get; set; }
}

public class ProcessPayrunCommandHandler : IRequestHandler<ProcessPayrunCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public ProcessPayrunCommandHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<int>> Handle(ProcessPayrunCommand request, CancellationToken cancellationToken)
    {
        // 1. Create Payroll Run Header
        // 1. Create Payroll Run Header
        var payrun = new PayrollRun
        {
            Month = (byte)request.Month,
            Year = (short)request.Year,
            RunDate = DateTime.Now,
            Status = "COMPLETED",
            Notes = $"Payroll for {request.Month}/{request.Year}"
        };

        _context.PayrollRuns.Add(payrun);
        await _context.SaveChangesAsync(cancellationToken); // Save to get RunId

        // 2. Get All Active Employees
        var employees = await _context.Employees.Where(e => e.IsDeleted == 0).Select(e => e.EmployeeId).ToListAsync(cancellationToken);
        int processedCount = 0;

        foreach (var employeeId in employees)
        {
            // 3. Calculate Salary using the Logic Query
            var calculationResult = await _mediator.Send(new CalculateMonthlySalaryQuery 
            { 
                EmployeeId = employeeId, 
                Month = request.Month, 
                Year = request.Year 
            }, cancellationToken);

            if (!calculationResult.Succeeded) continue; // Skip if calculation fails (e.g. no structure)

            var calc = calculationResult.Data;

            // 4. Create Payslip
            var payslip = new Payslip
            {
                RunId = payrun.RunId,
                EmployeeId = employeeId,
                BasicSalary = calc.BasicSalary,
                TotalAllowances = calc.TotalAllowances,
                TotalDeductions = calc.TotalStructureDeductions + calc.LoanDeductions + calc.AttendancePenalties,
                NetSalary = calc.NetSalary,
                
                // Detailed Breakdown
                TotalLateMinutes = calc.TotalLateMinutes,
                AbsenceDays = calc.AbsenceDays,
                TotalOvertimeMinutes = calc.TotalOvertimeMinutes,
                OvertimeEarnings = calc.OvertimeEarnings
            };

            _context.Payslips.Add(payslip); 

            // 5. Update Loan Installments Status
            if (calc.PaidInstallmentIds.Any())
            {
                var installmentsToUpdate = await _context.LoanInstallments
                    .Where(i => calc.PaidInstallmentIds.Contains(i.InstallmentId))
                    .ToListAsync(cancellationToken);
                
                foreach (var inst in installmentsToUpdate)
                {
                    inst.IsPaid = 1;
                    inst.PaidInPayrollRun = payrun.RunId;
                }
            }

            processedCount++;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(processedCount, $"تم معالجة الرواتب لـ {processedCount} موظف بنجاح");
    }
}
