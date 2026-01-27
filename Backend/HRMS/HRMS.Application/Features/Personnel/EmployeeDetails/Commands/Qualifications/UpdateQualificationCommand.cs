using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Qualifications;

public record UpdateQualificationCommand(int EmployeeId, int QualificationId, EmployeeQualificationDto Qualification) : IRequest<bool>;

public class UpdateQualificationCommandHandler : IRequestHandler<UpdateQualificationCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateQualificationCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateQualificationCommand request, CancellationToken cancellationToken)
    {
        var qualification = await _context.Qualifications
            .FirstOrDefaultAsync(q => q.QualificationId == request.QualificationId && q.EmployeeId == request.EmployeeId, cancellationToken);

        if (qualification == null)
            throw new KeyNotFoundException($"Qualification {request.QualificationId} not found for Employee {request.EmployeeId}");

        // Map updates (excluding IDs ideally, but AutoMapper handles this if config is right)
        _mapper.Map(request.Qualification, qualification);
        
        // Ensure critical IDs are not changed by DTO
        qualification.QualificationId = request.QualificationId;
        qualification.EmployeeId = request.EmployeeId;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
