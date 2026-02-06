using AutoMapper;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.AppraisalCycles.Queries.GetAll;

public class GetAppraisalCyclesQuery : IRequest<Result<List<AppraisalCycleDto>>>
{
}

public class GetAppraisalCyclesQueryHandler : IRequestHandler<GetAppraisalCyclesQuery, Result<List<AppraisalCycleDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAppraisalCyclesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<AppraisalCycleDto>>> Handle(GetAppraisalCyclesQuery request, CancellationToken cancellationToken)
    {
        var cycles = await _context.AppraisalCycles
            .OrderByDescending(c => c.StartDate)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<AppraisalCycleDto>>(cycles);

        return Result<List<AppraisalCycleDto>>.Success(dtos);
    }
}
