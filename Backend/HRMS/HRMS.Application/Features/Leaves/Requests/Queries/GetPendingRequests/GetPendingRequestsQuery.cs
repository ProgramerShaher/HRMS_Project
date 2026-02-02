using AutoMapper;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

    public GetPendingRequestsQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

	//public async Task<Result<List<LeaveRequestDto>>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
	//{
	//    // 1. Get Current User ID
	//    var userId = _currentUserService.UserId;
	//    if (string.IsNullOrEmpty(userId))
	//    {
	//        return Result<List<LeaveRequestDto>>.Failure("User is not authenticated", 401);
	//    }

	//    // 2. Get Employee ID for the current user
	//    var employeeId = await _context.Employees
	//        .Where(e => e.UserId == userId && e.IsDeleted == 0)
	//        .Select(e => e.EmployeeId)
	//        .FirstOrDefaultAsync(cancellationToken);

	//    if (employeeId == 0)
	//    {
	//         // If user is Admin but not an employee? Or just return empty?
	//         // Assuming strict mapping for managers.
	//         return Result<List<LeaveRequestDto>>.Failure("User is not linked to an employee record", 403);
	//    }

	//    // 3. Query Requests needing approval by this user
	//    // Join LeaveRequests with WorkflowApprovals
	//    var pendingRequests = await (from lr in _context.LeaveRequests
	//                                 join wa in _context.WorkflowApprovals on lr.RequestId equals wa.RequestId
	//                                 where wa.ApproverId == employeeId
	//                                    && wa.Status == "PENDING"
	//                                    && wa.RequestType == "LEAVE"
	//                                    && lr.IsDeleted == 0
	//                                 orderby lr.StartDate
	//                                 select lr)
	//                                 .Include(r => r.LeaveType)
	//                                 .Include(r => r.Employee)
	//                                 .AsNoTracking()
	//                                 .ToListAsync(cancellationToken);

	//    var dtos = _mapper.Map<List<LeaveRequestDto>>(pendingRequests);

	//    return Result<List<LeaveRequestDto>>.Success(dtos);
	//}
	public async Task<Result<List<LeaveRequestDto>>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
	{
		// --- للتطوير فقط: تجاوز الهوية ---
		var userId = "DEV_USER";
		var employeeId = 7; // افترض أنك الآن المدير رقم 7
							// ----------------------------

		// 3. Query Requests (الكود كما هو رائع جداً)
		var pendingRequests = await (from lr in _context.LeaveRequests
									 join wa in _context.WorkflowApprovals on lr.RequestId equals wa.RequestId
									 where wa.ApproverId == employeeId
										&& wa.Status == "PENDING"
										&& wa.RequestType == "LEAVE"
										&& lr.IsDeleted == 0
									 orderby lr.StartDate
									 select lr)
									 .Include(r => r.LeaveType)
									 .Include(r => r.Employee)
									 .AsNoTracking()
									 .ToListAsync(cancellationToken);

		var dtos = _mapper.Map<List<LeaveRequestDto>>(pendingRequests);
		return Result<List<LeaveRequestDto>>.Success(dtos);
	}

}

