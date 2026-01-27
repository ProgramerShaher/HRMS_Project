using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Experiences;

public record DeleteExperienceCommand(int EmployeeId, int ExperienceId) : IRequest<bool>;

public class DeleteExperienceCommandHandler : IRequestHandler<DeleteExperienceCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteExperienceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteExperienceCommand request, CancellationToken cancellationToken)
    {
        var experience = await _context.Experiences
            .FirstOrDefaultAsync(e => e.ExperienceId == request.ExperienceId && e.EmployeeId == request.EmployeeId, cancellationToken);

        if (experience == null)
            throw new KeyNotFoundException($"Experience {request.ExperienceId} not found for Employee {request.EmployeeId}");

        _context.Experiences.Remove(experience);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
