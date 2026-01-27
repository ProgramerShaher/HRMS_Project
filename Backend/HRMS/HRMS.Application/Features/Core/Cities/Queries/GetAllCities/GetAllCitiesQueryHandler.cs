using MediatR;
using HRMS.Application.DTOs.Core;
using HRMS.Application.DTOs;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Cities.Queries.GetAllCities;

public class GetAllCitiesQueryHandler : IRequestHandler<GetAllCitiesQuery, PagedResult<CityListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllCitiesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CityListDto>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Cities.AsQueryable();

        if (request.CountryId.HasValue)
            query = query.Where(c => c.CountryId == request.CountryId.Value);

        if (request.IsActive.HasValue)
            query = query.Where(c => c.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.CityNameAr)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CityListDto
            {
                CityId = c.CityId,
                CityNameAr = c.CityNameAr,
                CityNameEn = c.CityNameEn,
                CountryNameAr = c.Country.CountryNameAr,
                IsActive = c.IsActive
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedResult<CityListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
