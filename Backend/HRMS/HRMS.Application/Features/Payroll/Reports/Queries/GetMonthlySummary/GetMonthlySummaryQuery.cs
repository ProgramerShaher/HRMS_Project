using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Reports.Queries.GetMonthlySummary;

/// <summary>
/// الحصول على ملخص شهري للرواتب
/// Get monthly payroll summary report
/// </summary>
public class GetMonthlySummaryQuery : IRequest<Result<MonthlyPayrollSummaryDto>>
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int? DepartmentId { get; set; }
}

public class GetMonthlySummaryQueryHandler : IRequestHandler<GetMonthlySummaryQuery, Result<MonthlyPayrollSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMonthlySummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MonthlyPayrollSummaryDto>> Handle(GetMonthlySummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // البحث عن مسير الرواتب للشهر المحدد
            // Find payroll run for specified month
            var payrollRun = await _context.PayrollRuns
                .Include(r => r.Payslips)
                    .ThenInclude(p => p.Employee)
                        .ThenInclude(e => e.Department)
                .FirstOrDefaultAsync(r => r.Month == request.Month && r.Year == request.Year, cancellationToken);

            if (payrollRun == null)
            {
                return Result<MonthlyPayrollSummaryDto>.Failure("No payroll run found for the specified month");
            }

            // تطبيق فلتر القسم
            // Apply department filter
            var payslips = payrollRun.Payslips.AsEnumerable();
            if (request.DepartmentId.HasValue)
            {
                payslips = payslips.Where(p => p.Employee.DepartmentId == request.DepartmentId.Value);
            }

            var payslipsList = payslips.ToList();

            // حساب الإجماليات
            // Calculate totals
            var summary = new MonthlyPayrollSummaryDto
            {
                Month = request.Month,
                Year = request.Year,
                TotalEmployees = payslipsList.Count,
                TotalBasicSalaries = payslipsList.Sum(p => p.BasicSalary ?? 0),
                TotalAllowances = payslipsList.Sum(p => p.TotalAllowances ?? 0),
                TotalDeductions = payslipsList.Sum(p => p.TotalDeductions ?? 0),
                TotalOvertimePayments = payslipsList.Sum(p => p.OvertimeEarnings),
                TotalNetSalaries = payslipsList.Sum(p => p.NetSalary ?? 0),
                DepartmentBreakdown = payslipsList
                    .GroupBy(p => new { p.Employee.DepartmentId, p.Employee.Department.DeptNameAr })
                    .Select(g => new DepartmentPayrollSummary
                    {
                        DepartmentId = g.Key.DepartmentId,
                        DepartmentName = g.Key.DeptNameAr ?? "غير محدد",
                        EmployeeCount = g.Count(),
                        TotalBasicSalaries = g.Sum(p => p.BasicSalary ?? 0),
                        TotalAllowances = g.Sum(p => p.TotalAllowances ?? 0),
                        TotalDeductions = g.Sum(p => p.TotalDeductions ?? 0),
                        TotalNetSalaries = g.Sum(p => p.NetSalary ?? 0)
                    })
                    .ToList()
            };

            return Result<MonthlyPayrollSummaryDto>.Success(summary);
        }
        catch (Exception ex)
        {
            return Result<MonthlyPayrollSummaryDto>.Failure($"Error generating monthly summary: {ex.Message}");
        }
    }
}
