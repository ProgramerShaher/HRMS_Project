using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Application.DTOs.Attendance;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Utilities;

namespace HRMS.Application.Features.Attendance.Requests.Permissions.Queries.GetMyPermissions;

public class GetMyPermissionsQuery : IRequest<Result<List<PermissionRequestDto>>>
{
    public int EmployeeId { get; set; }
}

public class GetMyPermissionsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetMyPermissionsQuery, Result<List<PermissionRequestDto>>>
{
    public async Task<Result<List<PermissionRequestDto>>> Handle(GetMyPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await context.PermissionRequests
            .Include(x => x.Employee)
            .Where(x => x.EmployeeId == request.EmployeeId)
            .OrderByDescending(x => x.PermissionDate)
            .Select(x => new PermissionRequestDto
            {
                PermissionRequestId = x.PermissionRequestId,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee.FullNameAr, // Or use a helper for bilingual
                PermissionDate = x.PermissionDate,
                PermissionType = x.PermissionType,
                Hours = x.Hours,
                Reason = x.Reason,
                Status = x.Status,
                RejectionReason = x.RejectionReason,
                CreatedAt = x.CreatedAt,
                ApprovedBy = x.ApprovedBy,
                ApproverName = string.Empty // Entity lacks Approver navigation
            })
            .ToListAsync(cancellationToken);

        return Result<List<PermissionRequestDto>>.Success(permissions);
    }
}
