using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Application.DTOs.Attendance;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Utilities;

namespace HRMS.Application.Features.Attendance.Requests.Queries.GetMyOvertimeRequests;

public class GetMyOvertimeRequestsQuery : IRequest<Result<List<OvertimeRequestDto>>>
{
    public int EmployeeId { get; set; }
}

public class GetMyOvertimeRequestsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetMyOvertimeRequestsQuery, Result<List<OvertimeRequestDto>>>
{
    public async Task<Result<List<OvertimeRequestDto>>> Handle(GetMyOvertimeRequestsQuery request, CancellationToken cancellationToken)
    {
        var requests = await context.OvertimeRequests
            .Include(x => x.Employee)
            .Where(x => x.EmployeeId == request.EmployeeId)
            .OrderByDescending(x => x.RequestDate)
            .Select(x => new OvertimeRequestDto
            {
                OtRequestId = x.OtRequestId,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee != null ? x.Employee.FullNameAr : string.Empty,
                RequestDate = x.RequestDate,
                WorkDate = x.WorkDate,
                HoursRequested = x.HoursRequested,
                ApprovedHours = x.ApprovedHours,
                Reason = x.Reason ?? string.Empty,
                Status = x.Status ?? string.Empty,
                CreatedAt = x.CreatedAt,
                ApprovedBy = x.ApprovedBy,
                ApproverName = x.Approver != null ? x.Approver.FullNameAr : string.Empty
            })
            .ToListAsync(cancellationToken);

        return Result<List<OvertimeRequestDto>>.Success(requests);
    }
}
