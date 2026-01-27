using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Experiences;

public record UpdateExperienceCommand(int EmployeeId, int ExperienceId, EmployeeExperienceDto Experience) : IRequest<bool>;

public class UpdateExperienceCommandHandler : IRequestHandler<UpdateExperienceCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateExperienceCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateExperienceCommand request, CancellationToken cancellationToken)
    {
        var experience = await _context.Experiences
            .FirstOrDefaultAsync(e => e.ExperienceId == request.ExperienceId && e.EmployeeId == request.EmployeeId, cancellationToken);

        if (experience == null)
            throw new KeyNotFoundException($"Experience {request.ExperienceId} not found for Employee {request.EmployeeId}");

        _mapper.Map(request.Experience, experience);

        // Ensure critical IDs are not changed
        experience.ExperienceId = request.ExperienceId;
        experience.EmployeeId = request.EmployeeId;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
