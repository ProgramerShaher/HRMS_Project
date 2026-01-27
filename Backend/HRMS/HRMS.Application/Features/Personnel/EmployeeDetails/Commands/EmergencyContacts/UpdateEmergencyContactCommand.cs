using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.EmergencyContacts;

public record UpdateEmergencyContactCommand(int EmployeeId, int ContactId, EmergencyContactDto Contact) : IRequest<bool>;

public class UpdateEmergencyContactCommandHandler : IRequestHandler<UpdateEmergencyContactCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateEmergencyContactCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _context.EmergencyContacts
            .FirstOrDefaultAsync(c => c.ContactId == request.ContactId && c.EmployeeId == request.EmployeeId, cancellationToken);

        if (contact == null)
            throw new KeyNotFoundException($"Emergency Contact {request.ContactId} not found for Employee {request.EmployeeId}");

        _mapper.Map(request.Contact, contact);

        // Ensure critical IDs are not changed
        contact.ContactId = request.ContactId;
        contact.EmployeeId = request.EmployeeId;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
