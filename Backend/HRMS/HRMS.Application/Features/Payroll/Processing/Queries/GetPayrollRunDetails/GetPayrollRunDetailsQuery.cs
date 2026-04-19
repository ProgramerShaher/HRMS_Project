using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Processing.Queries.GetPayrollRunDetails;

/// <summary>
/// الحصول على تفاصيل مسير رواتب محدد
/// Get details of specific payroll run
/// </summary>
public class GetPayrollRunDetailsQuery : IRequest<Result<PayrollRunDetailsDto>>
{
    public int RunId { get; set; }
}

public class GetPayrollRunDetailsQueryHandler : IRequestHandler<GetPayrollRunDetailsQuery, Result<PayrollRunDetailsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPayrollRunDetailsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PayrollRunDetailsDto>> Handle(GetPayrollRunDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = new PayrollRunDetailsDto();

        // 1. معلومات المسير
        var run = await _context.PayrollRuns
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RunId == request.RunId, cancellationToken);

        if (run == null)
        {
            return Result<PayrollRunDetailsDto>.Failure("مسير الرواتب غير موجود");
        }

        // 2. جلب جميع القسائم
        var payslips = await _context.Payslips
            .Include(p => p.Employee)
            .Where(p => p.RunId == request.RunId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        result.Run = new PayrollRunDto
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

        // 3. ملخص القسائم
        foreach (var payslip in payslips)
        {
            result.Payslips.Add(new PayslipSummaryDto
            {
                PayslipId = payslip.PayslipId,
                EmployeeId = payslip.EmployeeId,
                EmployeeCode = payslip.Employee.EmployeeNumber,
                EmployeeName = payslip.Employee.FullNameAr ?? "Unknown",
                BasicSalary = payslip.BasicSalary ?? 0,
                TotalAllowances = payslip.TotalAllowances ?? 0,
                TotalDeductions = payslip.TotalDeductions ?? 0,
                NetSalary = payslip.NetSalary ?? 0
            });
        }

        // 4. الملخص الإجمالي
        result.Summary = new PayrollRunSummary
        {
            TotalEmployees = payslips.Count,
            TotalBasicSalaries = payslips.Sum(p => p.BasicSalary ?? 0),
            TotalAllowances = payslips.Sum(p => p.TotalAllowances ?? 0),
            TotalDeductions = payslips.Sum(p => p.TotalDeductions ?? 0),
            TotalGross = payslips.Sum(p => (p.BasicSalary ?? 0) + (p.TotalAllowances ?? 0)),
            TotalNet = payslips.Sum(p => p.NetSalary ?? 0),
            TotalLoanDeductions = 0, // TODO: Calculate from payslip details
            TotalOvertimePayments = payslips.Sum(p => p.OvertimeEarnings)
        };

        return Result<PayrollRunDetailsDto>.Success(result);
    }
}
