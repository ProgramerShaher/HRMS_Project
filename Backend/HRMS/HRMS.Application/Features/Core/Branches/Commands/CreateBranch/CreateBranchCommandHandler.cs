using AutoMapper;
using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Branches.Commands.CreateBranch
{
    public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, int>
    {
        private readonly HRMSDbContext _context;
        private readonly IMapper _mapper;

        public CreateBranchCommandHandler(HRMSDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
        {
            var branch = _mapper.Map<Branch>(request);
            
            branch.CreatedBy = "API_USER";
            branch.CreatedAt = DateTime.Now;
            branch.VersionNo = 1;
            branch.IsDeleted = 0;

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync(cancellationToken);

            return branch.BranchId;
        }
    }
}
