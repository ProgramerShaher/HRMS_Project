using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.EmergencyContacts;

public record DeleteEmergencyContactCommand(int EmployeeId, int ContactId) : IRequest<bool>;

public class DeleteEmergencyContactCommandHandler : IRequestHandler<DeleteEmergencyContactCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteEmergencyContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _context.EmergencyContacts
            .FirstOrDefaultAsync(c => c.ContactId == request.ContactId && c.EmployeeId == request.EmployeeId, cancellationToken);

        if (contact == null)
            throw new KeyNotFoundException($"Emergency Contact {request.ContactId} not found for Employee {request.EmployeeId}");

        _context.EmergencyContacts.Remove(contact);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
