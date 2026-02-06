using AutoMapper;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.DisciplinaryActions.Queries.GetAll;

public class GetDisciplinaryActionsQuery : IRequest<Result<List<DisciplinaryActionDto>>>
{
}

public class GetDisciplinaryActionsQueryHandler : IRequestHandler<GetDisciplinaryActionsQuery, Result<List<DisciplinaryActionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDisciplinaryActionsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<DisciplinaryActionDto>>> Handle(GetDisciplinaryActionsQuery request, CancellationToken cancellationToken)
    {
        var actions = await _context.DisciplinaryActions
            .OrderBy(a => a.DeductionDays)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<DisciplinaryActionDto>>(actions);

        return Result<List<DisciplinaryActionDto>>.Success(dtos);
    }
}
