using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Qualifications;

public record DeleteQualificationCommand(int EmployeeId, int QualificationId) : IRequest<bool>;

public class DeleteQualificationCommandHandler : IRequestHandler<DeleteQualificationCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteQualificationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteQualificationCommand request, CancellationToken cancellationToken)
    {
        var qualification = await _context.Qualifications
            .FirstOrDefaultAsync(q => q.QualificationId == request.QualificationId && q.EmployeeId == request.EmployeeId, cancellationToken);

        if (qualification == null)
            throw new KeyNotFoundException($"Qualification {request.QualificationId} not found for Employee {request.EmployeeId}");

        _context.Qualifications.Remove(qualification);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
