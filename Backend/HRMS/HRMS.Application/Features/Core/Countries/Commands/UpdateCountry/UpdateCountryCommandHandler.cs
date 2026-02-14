using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;
using AutoMapper;

namespace HRMS.Application.Features.Core.Countries.Commands.UpdateCountry;

/// <summary>
/// معالج أمر تحديث بيانات الدولة
/// </summary>
public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateCountryCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = await _context.Countries
            .FirstOrDefaultAsync(c => c.CountryId == request.CountryId, cancellationToken);

        if (country == null)
            throw new KeyNotFoundException($"الدولة برقم {request.CountryId} غير موجودة");

        _mapper.Map(request, country);
        if (!string.IsNullOrEmpty(country.IsoCode))
            country.IsoCode = country.IsoCode.ToUpper();
            
        country.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return country.CountryId;
    }
}
