using AutoMapper;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Application.Features.Leaves.Requests.Queries.GetAllRequests;

public record GetAllRequestsQuery : IRequest<Result<List<LeaveRequestDto>>>;

public class GetAllRequestsQueryHandler : IRequestHandler<GetAllRequestsQuery, Result<List<LeaveRequestDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAllRequestsQueryHandler(
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

    public async Task<Result<List<LeaveRequestDto>>> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
    {
        var userIdString = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userIdString))
            return Result<List<LeaveRequestDto>>.Failure("User is not authenticated", 401);

        var user = await _userManager.FindByIdAsync(userIdString);
        if (user == null)
            return Result<List<LeaveRequestDto>>.Failure("User not found", 404);

        var isAdmin = await _userManager.IsInRoleAsync(user, "System_Admin") || await _userManager.IsInRoleAsync(user, "HR_Manager");
        var employeeId = user.EmployeeId;

        IQueryable<HRMS.Core.Entities.Leaves.LeaveRequest> query;

        if (isAdmin)
        {
            query = _context.LeaveRequests.Where(lr => lr.IsDeleted == 0);
        }
        else
        {
            if (!employeeId.HasValue)
                return Result<List<LeaveRequestDto>>.Failure("User is not linked to an employee record", 403);

            var myDeptId = await _context.Employees
                .Where(e => e.EmployeeId == employeeId.Value)
                .Select(e => e.DepartmentId)
                .FirstOrDefaultAsync(cancellationToken);
                
            query = _context.LeaveRequests.Where(lr => lr.IsDeleted == 0 && lr.Employee.DepartmentId == myDeptId);
        }

        var reqs = await query
            .Include(r => r.LeaveType)
            .Include(r => r.Employee)
            .Include(r => r.Employee.Department)
            .OrderByDescending(r => r.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<LeaveRequestDto>>(reqs);
        return Result<List<LeaveRequestDto>>.Success(dtos);
    }
}
