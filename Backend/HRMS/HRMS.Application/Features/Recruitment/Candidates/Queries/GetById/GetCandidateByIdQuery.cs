using AutoMapper;
using HRMS.Application.DTOs.Recruitment;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Candidates.Queries.GetById;

public class GetCandidateByIdQuery : IRequest<Result<CandidateDto>>
{
    public int CandidateId { get; set; }
}

public class GetCandidateByIdQueryHandler : IRequestHandler<GetCandidateByIdQuery, Result<CandidateDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCandidateByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<CandidateDto>> Handle(GetCandidateByIdQuery request, CancellationToken cancellationToken)
    {
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.CandidateId == request.CandidateId && c.IsDeleted == 0, cancellationToken);

        if (candidate == null)
            return Result<CandidateDto>.Failure("المرشح غير موجود");

        var dto = _mapper.Map<CandidateDto>(candidate);

        return Result<CandidateDto>.Success(dto);
    }
}
