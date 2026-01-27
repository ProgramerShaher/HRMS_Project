using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;

namespace HRMS.Application.Features.Core.Jobs.Commands.UpdateJob;

public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateJobCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _context.Jobs
            .FirstOrDefaultAsync(j => j.JobId == request.JobId && j.IsDeleted == 0, cancellationToken);

        if (job == null)
            throw new KeyNotFoundException($"الوظيفة برقم {request.JobId} غير موجودة");

        job.JobTitleAr = request.JobTitleAr;
        job.JobTitleEn = request.JobTitleEn;
        job.DefaultGradeId = request.DefaultGradeId;
        job.IsMedical = request.IsMedical;
        job.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return job.JobId;
    }
}
