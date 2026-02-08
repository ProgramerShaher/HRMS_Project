using AutoMapper;
using HRMS.Application.DTOs.Recruitment;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Offers.Queries.GetById;

/// <summary>
/// Get Job Offer By ID Query
/// </summary>
public class GetOfferByIdQuery : IRequest<Result<JobOfferDto>>
{
    public int OfferId { get; set; }
}

/// <summary>
/// Get Offer By ID Query Handler
/// </summary>
public class GetOfferByIdQueryHandler : IRequestHandler<GetOfferByIdQuery, Result<JobOfferDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOfferByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<JobOfferDto>> Handle(GetOfferByIdQuery request, CancellationToken cancellationToken)
    {
        // جلب عرض العمل مع كافة البيانات المرتبطة
        var offer = await _context.JobOffers
            .Include(o => o.Application)
                .ThenInclude(a => a.Candidate)
            .Include(o => o.Application)
                .ThenInclude(a => a.Vacancy)
                    .ThenInclude(v => v.Job)
            .Include(o => o.Application)
                .ThenInclude(a => a.Vacancy)
                    .ThenInclude(v => v.Department)
            .FirstOrDefaultAsync(o => o.OfferId == request.OfferId && o.IsDeleted == 0, cancellationToken);

        if (offer == null)
            return Result<JobOfferDto>.Failure("عرض العمل غير موجود");

        var dto = _mapper.Map<JobOfferDto>(offer);

        return Result<JobOfferDto>.Success(dto);
    }
}
