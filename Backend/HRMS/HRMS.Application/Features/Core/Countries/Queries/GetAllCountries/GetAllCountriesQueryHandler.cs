using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Countries.Queries.GetAllCountries
{
    public class GetAllCountriesQueryHandler : IRequestHandler<GetAllCountriesQuery, List<Country>>
    {
        private readonly HRMSDbContext _context;

        public GetAllCountriesQueryHandler(HRMSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Country>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Countries
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
