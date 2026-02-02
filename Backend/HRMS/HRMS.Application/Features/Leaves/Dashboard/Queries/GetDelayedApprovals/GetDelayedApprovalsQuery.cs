using AutoMapper;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.Dashboard.Queries.GetDelayedApprovals;

// Query
public record GetDelayedApprovalsQuery : IRequest<Result<List<LeaveRequestDto>>>;

// Handler
public class GetDelayedApprovalsQueryHandler : IRequestHandler<GetDelayedApprovalsQuery, Result<List<LeaveRequestDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDelayedApprovalsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<LeaveRequestDto>>> Handle(GetDelayedApprovalsQuery request, CancellationToken cancellationToken)
    {
        var thresholdDate = DateTime.UtcNow.AddHours(-48);

        var delayedRequests = await _context.LeaveRequests
            .AsNoTracking()
            .Include(r => r.Employee)
            .Include(r => r.LeaveType)
            .Where(r => r.Status == "PENDING" 
                     && r.IsDeleted == 0 
                     && r.CreatedAt <= thresholdDate) // تأخرت أكثر من 48 ساعة
            .OrderBy(r => r.CreatedAt) // الأقدم أولاً
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<LeaveRequestDto>>(delayedRequests);

        return Result<List<LeaveRequestDto>>.Success(dtos);
    }
}
