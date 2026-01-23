using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Jobs.Commands.CreateJob
{
    public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, int>
    {
        private readonly HRMSDbContext _context;
        public CreateJobCommandHandler(HRMSDbContext context) => _context = context;

        public async Task<int> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            var job = new Job
            {
                JobTitleAr = request.JobTitleAr,
                JobTitleEn = request.JobTitleEn,
                DefaultGradeId = request.DefaultGradeId,
                CreatedBy = "API_USER",
                CreatedAt = DateTime.Now
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync(cancellationToken);
            return job.JobId;
        }
    }
}
