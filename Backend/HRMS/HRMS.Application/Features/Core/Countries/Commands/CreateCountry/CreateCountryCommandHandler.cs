using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Core;
using AutoMapper;

namespace HRMS.Application.Features.Core.Countries.Commands.CreateCountry;

public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateCountryCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = _mapper.Map<Country>(request);
        if (!string.IsNullOrEmpty(country.IsoCode))
            country.IsoCode = country.IsoCode.ToUpper();
            
        country.CreatedAt = DateTime.UtcNow;

        _context.Countries.Add(country);
        await _context.SaveChangesAsync(cancellationToken);

        return country.CountryId;
    }
}
