using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Processing.Queries.GetPayrollRuns;

/// <summary>
/// الحصول على قائمة بجميع مسيرات الرواتب
/// Get list of all payroll runs
/// </summary>
public class GetPayrollRunsQuery : IRequest<Result<List<PayrollRunDto>>>
{
    public int? Year { get; set; }
    public string? Status { get; set; }
}

public class GetPayrollRunsQueryHandler : IRequestHandler<GetPayrollRunsQuery, Result<List<PayrollRunDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPayrollRunsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PayrollRunDto>>> Handle(GetPayrollRunsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PayrollRuns.AsNoTracking();

        // تطبيق الفلاتر
        if (request.Year.HasValue)
        {
            query = query.Where(r => r.Year == request.Year.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(r => r.Status == request.Status);
        }

        var runs = await query
            .OrderByDescending(r => r.Year)
            .ThenByDescending(r => r.Month)
            .ToListAsync(cancellationToken);

        var result = new List<PayrollRunDto>();

        foreach (var run in runs)
        {
            // حساب عدد الموظفين والإجماليات
            var payslips = await _context.Payslips
                .Where(p => p.RunId == run.RunId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var dto = new PayrollRunDto
            {
                RunId = run.RunId,
                Month = run.Month,
                Year = run.Year,
                ProcessDate = run.RunDate,
                Status = run.Status,
                EmployeeCount = payslips.Count,
                TotalGross = payslips.Sum(p => (p.BasicSalary ?? 0) + (p.TotalAllowances ?? 0)),
                TotalNet = payslips.Sum(p => p.NetSalary ?? 0),
                ProcessedBy = run.CreatedBy?.ToString(),
                ApprovedDate = run.CreatedAt,
                ApprovedBy = run.CreatedBy?.ToString()
            };

            result.Add(dto);
        }

        return Result<List<PayrollRunDto>>.Success(result);
    }
}
