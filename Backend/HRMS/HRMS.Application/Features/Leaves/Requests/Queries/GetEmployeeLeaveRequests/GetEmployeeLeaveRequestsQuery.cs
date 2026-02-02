using AutoMapper;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.Requests.Queries.GetEmployeeLeaveRequests;

// 1. Query
/// <summary>
/// استعلام جلب طلبات الإجازة لموظف محدد
/// Query to get leave requests for a specific employee
/// </summary>
public record GetEmployeeLeaveRequestsQuery(int EmployeeId) : IRequest<Result<List<LeaveRequestDto>>>;

// 2. Handler
public class GetEmployeeLeaveRequestsQueryHandler : IRequestHandler<GetEmployeeLeaveRequestsQuery, Result<List<LeaveRequestDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeLeaveRequestsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<LeaveRequestDto>>> Handle(GetEmployeeLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        // استخدام Include لجلب البيانات المرتبطة (نوع الإجازة)
        // Use Include to fetch related data (LeaveType)
        var requestsQuery = _context.LeaveRequests
            .AsNoTracking()
            .Include(r => r.LeaveType)
            .Where(r => r.EmployeeId == request.EmployeeId && r.IsDeleted == 0); // تصفية المحذوفات

        var requests = await requestsQuery
            .OrderByDescending(r => r.StartDate)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<LeaveRequestDto>>(requests);

        return Result<List<LeaveRequestDto>>.Success(dtos);
    }
}
