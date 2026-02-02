using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.Reports.Queries.GetPayrollIntegrationReport;

// DTO
public record PayrollLeaveDto(
    int EmployeeId,
    string EmployeeName,
    string LeaveTypeName,
    int DaysCount,
    bool IsDeductible,
    DateTime StartDate,
    DateTime EndDate
);

// Query
public record GetPayrollIntegrationReportQuery(int Month, int Year) : IRequest<Result<List<PayrollLeaveDto>>>;

// Handler
public class GetPayrollIntegrationReportQueryHandler : IRequestHandler<GetPayrollIntegrationReportQuery, Result<List<PayrollLeaveDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPayrollIntegrationReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PayrollLeaveDto>>> Handle(GetPayrollIntegrationReportQuery request, CancellationToken cancellationToken)
    {
        // تحديد نطاق الشهر
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // جلب الإجازات المعتمدة التي تقع (ولو جزئياً) داخل الشهر المجدد
        // ERP Standard: usually we integrate what was "Approved" in this month, OR what "Occurred" in this month.
        // Assuming "Occurred" (Overlap logic) for payroll deduction calculation.

        var query = _context.LeaveRequests
            .AsNoTracking()
            .Include(r => r.Employee)
            .Include(r => r.LeaveType)
            .Where(r => r.Status == "MANAGER_APPROVED" || r.Status == "HR_APPROVED")
            .Where(r => r.IsDeleted == 0)
            .Where(r => r.StartDate <= endDate && r.EndDate >= startDate); // Overlap check

        var leaves = await query.ToListAsync(cancellationToken);

        var report = leaves.Select(r => new PayrollLeaveDto(
            r.EmployeeId,
            r.Employee.FirstNameAr + " " + r.Employee.FirstNameAr,
            r.LeaveType.LeaveNameAr,
            r.DaysCount, // Note: This is total days. For precise payroll, we might need days *within* the month.
                         // But for now, we return the request data as is.
            r.LeaveType.IsDeductible == 1,
            r.StartDate,
            r.EndDate
        )).ToList();

        return Result<List<PayrollLeaveDto>>.Success(report);
    }
}
