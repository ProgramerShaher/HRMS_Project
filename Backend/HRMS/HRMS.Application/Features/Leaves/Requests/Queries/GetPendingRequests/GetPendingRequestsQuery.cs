using AutoMapper;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HRMS.Core.Entities.Identity;
using HRMS.Core.Entities.Leaves;

namespace HRMS.Application.Features.Leaves.Requests.Queries.GetPendingRequests;

// 1. Query
/// <summary>
/// استعلام جلب جميع طلبات الإجازة المعلقة
/// Query to get all pending leave requests (for managers/HR)
/// </summary>
public record GetPendingRequestsQuery : IRequest<Result<List<LeaveRequestDto>>>;

// 2. Handler
public class GetPendingRequestsQueryHandler : IRequestHandler<GetPendingRequestsQuery, Result<List<LeaveRequestDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetPendingRequestsQueryHandler(
        IApplicationDbContext context, 
        IMapper mapper, 
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<Result<List<LeaveRequestDto>>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
    {
        var userIdString = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userIdString))
            return Result<List<LeaveRequestDto>>.Failure("User is not authenticated", 401);

        var user = await _userManager.FindByIdAsync(userIdString);
        if (user == null)
            return Result<List<LeaveRequestDto>>.Failure("User not found", 404);

        var isAdmin = await _userManager.IsInRoleAsync(user, "System_Admin") || await _userManager.IsInRoleAsync(user, "HR_Manager");
        var employeeId = user.EmployeeId;

        IQueryable<LeaveRequest> query;

        if (isAdmin)
        {
            // Admins see all pending requests regardless of workflow records
            query = _context.LeaveRequests
                .Where(lr => lr.Status == "PENDING" && lr.IsDeleted == 0);
        }
        else
        {
            if (!employeeId.HasValue)
                return Result<List<LeaveRequestDto>>.Failure("User is not linked to an employee record", 403);

            // Non-admins see requests assigned to them in workflow
            query = from lr in _context.LeaveRequests
                    join wa in _context.WorkflowApprovals on lr.RequestId equals wa.RequestId
                    where wa.ApproverId == employeeId.Value
                       && wa.Status == "PENDING"
                       && wa.RequestType == "LEAVE"
                       && lr.IsDeleted == 0
                    select lr;
        }

        var pendingRequests = await query
            .Include(r => r.LeaveType)
            .Include(r => r.Employee)
            .OrderBy(r => r.StartDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<LeaveRequestDto>>(pendingRequests);
        return Result<List<LeaveRequestDto>>.Success(dtos);
    }

}
