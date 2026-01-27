using MediatR;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Cities.Commands.UpdateCity;

public class UpdateCityCommandHandler : IRequestHandler<UpdateCityCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateCityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var city = await _context.Cities
            .FirstOrDefaultAsync(c => c.CityId == request.CityId, cancellationToken);

        if (city == null)
            throw new KeyNotFoundException($"المدينة برقم {request.CityId} غير موجودة");

        city.CountryId = request.CountryId;
        city.CityNameAr = request.CityNameAr;
        city.CityNameEn = request.CityNameEn;
        city.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return city.CityId;
    }
}
