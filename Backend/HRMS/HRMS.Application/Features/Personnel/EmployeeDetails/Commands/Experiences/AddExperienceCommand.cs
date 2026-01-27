using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Experiences;

public record AddExperienceCommand(int EmployeeId, EmployeeExperienceDto Experience) : IRequest<int>;

public class AddExperienceCommandHandler : IRequestHandler<AddExperienceCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddExperienceCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(AddExperienceCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new KeyNotFoundException($"Employee {request.EmployeeId} not found");

        var experience = _mapper.Map<EmployeeExperience>(request.Experience);
        experience.EmployeeId = request.EmployeeId;

        _context.Experiences.Add(experience);
        await _context.SaveChangesAsync(cancellationToken);

        return experience.ExperienceId;
    }
}
