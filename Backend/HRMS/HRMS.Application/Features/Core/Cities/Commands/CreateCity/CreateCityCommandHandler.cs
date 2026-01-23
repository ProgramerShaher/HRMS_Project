using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Cities.Commands.CreateCity
{
    public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, int>
    {
        private readonly HRMSDbContext _context;
        public CreateCityCommandHandler(HRMSDbContext context) => _context = context;

        public async Task<int> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            var city = new City
            {
                CityNameAr = request.CityNameAr,
                CityNameEn = request.CityNameEn,
                CountryId = request.CountryId,
                CreatedBy = "API_USER",
                CreatedAt = DateTime.Now
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync(cancellationToken);
            return city.CityId;
        }
    }
}
