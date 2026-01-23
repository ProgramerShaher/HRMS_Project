using HRMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Branches.Commands.UpdateBranch
{
    public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, bool>
    {
        private readonly HRMSDbContext _context;

        public UpdateBranchCommandHandler(HRMSDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
        {
            var branch = await _context.Branches.FindAsync(request.BranchId);
            
            if (branch == null)
                return false;

            branch.BranchNameAr = request.BranchNameAr;
            branch.BranchNameEn = request.BranchNameEn;
            branch.CityId = request.CityId;
            branch.Address = request.Address;
            branch.UpdatedBy = "API_USER";
            branch.UpdatedAt = DateTime.Now;
            branch.VersionNo = branch.VersionNo + 1;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
