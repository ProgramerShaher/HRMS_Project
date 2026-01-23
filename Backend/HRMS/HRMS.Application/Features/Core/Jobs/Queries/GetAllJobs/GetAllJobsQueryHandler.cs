using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Jobs.Queries.GetAllJobs
{
    public class GetAllJobsQueryHandler : IRequestHandler<GetAllJobsQuery, List<Job>>
    {
        private readonly HRMSDbContext _context;
        public GetAllJobsQueryHandler(HRMSDbContext context) => _context = context;

        public async Task<List<Job>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Jobs.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
