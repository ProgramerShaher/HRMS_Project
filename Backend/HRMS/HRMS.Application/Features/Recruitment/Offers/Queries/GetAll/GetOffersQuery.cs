using AutoMapper;
using HRMS.Application.DTOs.Recruitment;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Offers.Queries.GetAll;

/// <summary>
/// استعلام للحصول على قائمة العروض الوظيفية
/// </summary>
public class GetOffersQuery : IRequest<Result<List<JobOfferDto>>>
{
    /// <summary>
    /// تصفية حسب طلب التوظيف
    /// </summary>
    public int? AppId { get; set; }

    /// <summary>
    /// تصفية حسب الحالة (PENDING, ACCEPTED, REJECTED)
    /// </summary>
    public string? Status { get; set; }
}

public class GetOffersQueryHandler : IRequestHandler<GetOffersQuery, Result<List<JobOfferDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOffersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<JobOfferDto>>> Handle(GetOffersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.JobOffers
            .Include(o => o.Application)
                .ThenInclude(a => a.Candidate)
            .Include(o => o.JoiningDate)
            .AsQueryable();

        // تطبيق الفلاتر
        if (request.AppId.HasValue)
        {
            query = query.Where(o => o.AppId == request.AppId.Value);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(o => o.Status == request.Status);
        }

        var offers = await query
            .OrderByDescending(o => o.OfferDate)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<JobOfferDto>>(offers);

        return Result<List<JobOfferDto>>.Success(dtos);
    }
}
