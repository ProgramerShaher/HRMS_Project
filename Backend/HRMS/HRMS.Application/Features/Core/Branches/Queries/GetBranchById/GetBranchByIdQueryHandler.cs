using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Branches.Queries.GetBranchById
{
    public class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, Branch>
    {
        private readonly HRMSDbContext _context;

        public GetBranchByIdQueryHandler(HRMSDbContext context)
        {
            _context = context;
        }

        public async Task<Branch> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BranchId == request.Id, cancellationToken);
        }
    }
}
