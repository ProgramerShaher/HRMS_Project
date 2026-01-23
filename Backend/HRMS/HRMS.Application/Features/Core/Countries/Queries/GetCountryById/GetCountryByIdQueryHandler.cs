using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Countries.Queries.GetCountryById
{
    public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, Country>
    {
        private readonly HRMSDbContext _context;

        public GetCountryByIdQueryHandler(HRMSDbContext context)
        {
            _context = context;
        }

        public async Task<Country> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CountryId == request.Id, cancellationToken);
        }
    }
}
