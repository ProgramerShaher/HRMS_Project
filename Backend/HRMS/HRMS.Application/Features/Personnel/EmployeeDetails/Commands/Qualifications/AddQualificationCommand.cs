using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Qualifications;

public record AddQualificationCommand(int EmployeeId, EmployeeQualificationDto Qualification) : IRequest<int>;

public class AddQualificationCommandHandler : IRequestHandler<AddQualificationCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddQualificationCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(AddQualificationCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Qualifications)
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new KeyNotFoundException($"Employee {request.EmployeeId} not found");

        var qualificationCheck = request.Qualification;
        // Basic duplicate check (optional, but good for data integrity)
        // e.g. check if same degree and major exists
        
        var qualification = _mapper.Map<EmployeeQualification>(request.Qualification);
        qualification.EmployeeId = request.EmployeeId; // Ensure link

        _context.Qualifications.Add(qualification);
        await _context.SaveChangesAsync(cancellationToken);

        return qualification.QualificationId;
    }
}
