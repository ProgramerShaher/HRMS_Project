using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Countries.Commands.CreateCountry
{
    public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, int>
    {
        private readonly HRMSDbContext _context;

        public CreateCountryCommandHandler(HRMSDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = new Country
            {
                CountryNameAr = request.CountryNameAr,
                CountryNameEn = request.CountryNameEn,
                CitizenshipNameAr = request.CitizenshipNameAr,
                IsoCode = request.IsoCode,
                CreatedBy = "API_USER",
                CreatedAt = DateTime.Now
            };

            _context.Countries.Add(country);
            await _context.SaveChangesAsync(cancellationToken);

            return country.CountryId;
        }
    }
}
