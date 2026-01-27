using MediatR;
using HRMS.Application.DTOs.Core;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Cities.Queries.GetCityById;

public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, CityDto?>
{
    private readonly IApplicationDbContext _context;

    public GetCityByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CityDto?> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        var city = await _context.Cities
            .Where(c => c.CityId == request.CityId)
            .Select(c => new CityDto
            {
                CityId = c.CityId,
                CountryId = c.CountryId,
                CityNameAr = c.CityNameAr,
                CityNameEn = c.CityNameEn,
                CountryNameAr = c.Country.CountryNameAr,
                CountryNameEn = c.Country.CountryNameEn,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return city;
    }
}
