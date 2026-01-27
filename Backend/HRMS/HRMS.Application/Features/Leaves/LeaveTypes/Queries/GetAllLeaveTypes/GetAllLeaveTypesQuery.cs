using AutoMapper;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.LeaveTypes.Queries.GetAllLeaveTypes;

/// <summary>
/// استعلام جلب كافة أنواع الإجازات
/// </summary>
public record GetAllLeaveTypesQuery : IRequest<List<LeaveTypeDto>>;

public class GetAllLeaveTypesQueryHandler : IRequestHandler<GetAllLeaveTypesQuery, List<LeaveTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllLeaveTypesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<LeaveTypeDto>> Handle(GetAllLeaveTypesQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.LeaveTypes.AsNoTracking().ToListAsync(cancellationToken);
        return _mapper.Map<List<LeaveTypeDto>>(list);
    }
}
