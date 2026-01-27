using HRMS.Core.Entities.Core;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Countries.Queries.GetCountryById;

/// <summary>
/// معالج استعلام الحصول على دولة بمعرفها
/// </summary>
public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, CountryDto?>
{
    private readonly IApplicationDbContext _context;

    public GetCountryByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CountryDto?> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
    {
        var country = await _context.Countries
            .Where(c => c.CountryId == request.CountryId)
            .Select(c => new CountryDto
            {
                CountryId = c.CountryId,
                CountryNameAr = c.CountryNameAr,
                CountryNameEn = c.CountryNameEn,
                CitizenshipNameAr = c.CitizenshipNameAr,
                IsoCode = c.IsoCode,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                CitiesCount = c.Cities.Count
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return country;
    }
}
