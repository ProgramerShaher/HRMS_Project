using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Application.DTOs.Attendance;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Utilities;

namespace HRMS.Application.Features.Attendance.Requests.Queries.GetMyShiftSwapRequests;

public class GetMyShiftSwapRequestsQuery : IRequest<Result<List<ShiftSwapRequestDto>>>
{
    public int EmployeeId { get; set; }
}

public class GetMyShiftSwapRequestsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetMyShiftSwapRequestsQuery, Result<List<ShiftSwapRequestDto>>>
{
    public async Task<Result<List<ShiftSwapRequestDto>>> Handle(GetMyShiftSwapRequestsQuery request, CancellationToken cancellationToken)
    {
        // Get swaps where user is requester OR target
        var requests = await context.ShiftSwapRequests
            .Include(x => x.Requester)
            .Include(x => x.TargetEmployee)
            .Where(x => x.RequesterId == request.EmployeeId || x.TargetEmployeeId == request.EmployeeId)
            .OrderByDescending(x => x.RosterDate)
            .Select(x => new ShiftSwapRequestDto
            {
                RequestId = x.RequestId,
                RequesterId = x.RequesterId,
                RequesterName = x.Requester != null ? x.Requester.FullNameAr : string.Empty,
                TargetEmployeeId = x.TargetEmployeeId,
                TargetEmployeeName = x.TargetEmployee != null ? x.TargetEmployee.FullNameAr : string.Empty,
                RosterDate = x.RosterDate,
                Status = x.Status ?? string.Empty,
                ManagerComment = x.ManagerComment,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<ShiftSwapRequestDto>>.Success(requests);
    }
}
