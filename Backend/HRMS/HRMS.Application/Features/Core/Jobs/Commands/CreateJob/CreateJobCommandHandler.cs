using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Core;
using AutoMapper;

namespace HRMS.Application.Features.Core.Jobs.Commands.CreateJob;

public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateJobCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        var job = _mapper.Map<Job>(request);
        job.CreatedAt = DateTime.UtcNow;

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync(cancellationToken);

        return job.JobId;
    }
}
