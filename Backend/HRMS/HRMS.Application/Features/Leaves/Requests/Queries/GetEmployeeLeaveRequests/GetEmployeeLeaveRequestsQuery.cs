using AutoMapper;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Exceptions;

namespace HRMS.Application.Features.Leaves.Requests.Queries.GetEmployeeLeaveRequests;

// 1. Query
/// <summary>
/// استعلام جلب طلبات الإجازة لموظف محدد
/// </summary>
public record GetEmployeeLeaveRequestsQuery(int EmployeeId) : IRequest<List<LeaveRequestDto>>;

// 2. Handler
public class GetEmployeeLeaveRequestsQueryHandler : IRequestHandler<GetEmployeeLeaveRequestsQuery, List<LeaveRequestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeLeaveRequestsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<LeaveRequestDto>> Handle(GetEmployeeLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        var requests = await _context.LeaveRequests
            .AsNoTracking()
            .Include(r => r.LeaveType)
            .Where(r => r.EmployeeId == request.EmployeeId)
            .OrderByDescending(r => r.StartDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<LeaveRequestDto>>(requests);
    }
}
