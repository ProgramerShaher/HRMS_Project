using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Branches.Queries.GetAllBranches
{
    public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, List<Branch>>
    {
        private readonly HRMSDbContext _context;

        public GetAllBranchesQueryHandler(HRMSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Branch>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Branches
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
