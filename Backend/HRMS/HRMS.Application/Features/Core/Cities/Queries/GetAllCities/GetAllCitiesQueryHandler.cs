using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Cities.Queries.GetAllCities
{
    public class GetAllCitiesQueryHandler : IRequestHandler<GetAllCitiesQuery, List<City>>
    {
        private readonly HRMSDbContext _context;
        public GetAllCitiesQueryHandler(HRMSDbContext context) => _context = context;

        public async Task<List<City>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Cities.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
