using AutoMapper;
using AutoMapper.QueryableExtensions;
using HRMS.Application.DTOs.Leaves; // Assuming RequestDto is here or need to create specific one
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Leaves.Requests.Queries.GetMyRequests;

public class GetMyLeaveRequestsQuery : IRequest<List<LeaveRequestDto>>
{
    public int EmployeeId { get; set; }
}

public class GetMyLeaveRequestsQueryHandler : IRequestHandler<GetMyLeaveRequestsQuery, List<LeaveRequestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMyLeaveRequestsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<LeaveRequestDto>> Handle(GetMyLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        return await _context.LeaveRequests
            .Where(r => r.EmployeeId == request.EmployeeId)
            .OrderByDescending(r => r.CreatedAt)
            .ProjectTo<LeaveRequestDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
