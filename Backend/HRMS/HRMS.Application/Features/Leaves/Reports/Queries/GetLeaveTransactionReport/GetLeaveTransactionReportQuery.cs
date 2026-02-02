using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.Reports.Queries.GetLeaveTransactionReport;

// DTO
public record LeaveTransactionReportDto(
    int RequestId,
    string EmployeeName,
    string DepartmentName,
    string LeaveType,
    DateTime StartDate,
    DateTime EndDate,
    int DaysCount,
    string Status,
    DateTime CreatedAt
);

// Query
public record GetLeaveTransactionReportQuery(
    DateTime? FromDate, 
    DateTime? ToDate, 
    int? EmployeeId, 
    string? Status
) : IRequest<Result<List<LeaveTransactionReportDto>>>;

// Handler
public class GetLeaveTransactionReportQueryHandler : IRequestHandler<GetLeaveTransactionReportQuery, Result<List<LeaveTransactionReportDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetLeaveTransactionReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LeaveTransactionReportDto>>> Handle(GetLeaveTransactionReportQuery request, CancellationToken cancellationToken)
    {
        // بناء الاستعلام الأساسي
        var query = _context.LeaveRequests
            .AsNoTracking()
            .Include(r => r.Employee)
            .ThenInclude(e => e.Department)
            .Include(r => r.LeaveType)
            .AsQueryable();

        // تطبيق الفلاتر
        if (request.FromDate.HasValue)
            query = query.Where(r => r.StartDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(r => r.StartDate <= request.ToDate.Value);

        if (request.EmployeeId.HasValue)
            query = query.Where(r => r.EmployeeId == request.EmployeeId.Value);

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(r => r.Status == request.Status);

        // الترتيب والإسقاط (Projection)
        var reportData = await query
            .OrderByDescending(r => r.StartDate)
            .Select(r => new LeaveTransactionReportDto(
                r.RequestId,
                r.Employee.FirstNameAr + " " + r.Employee.FirstNameAr, // الاسم الكامل (بافتراض وجود هذه الحقول)
                r.Employee.Department != null ? r.Employee.Department.DeptNameAr : "",
                r.LeaveType.LeaveNameAr,
                r.StartDate,
                r.EndDate,
                r.DaysCount,
                r.Status ?? "Unknown",
                r.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return Result<List<LeaveTransactionReportDto>>.Success(reportData);
    }
}
