using AutoMapper;
using HRMS.Application.DTOs.Recruitment;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Candidates.Queries.GetAll;

public class GetCandidatesQuery : IRequest<Result<List<CandidateDto>>>
{
}

public class GetCandidatesQueryHandler : IRequestHandler<GetCandidatesQuery, Result<List<CandidateDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCandidatesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<CandidateDto>>> Handle(GetCandidatesQuery request, CancellationToken cancellationToken)
    {
        var candidates = await _context.Candidates
            .Where(c => c.IsDeleted == 0)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<CandidateDto>>(candidates);

        return Result<List<CandidateDto>>.Success(dtos);
    }
}
