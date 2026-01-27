using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.EmergencyContacts;

public record AddEmergencyContactCommand(int EmployeeId, EmergencyContactDto Contact) : IRequest<int>;

public class AddEmergencyContactCommandHandler : IRequestHandler<AddEmergencyContactCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddEmergencyContactCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(AddEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new KeyNotFoundException($"Employee {request.EmployeeId} not found");

        var contact = _mapper.Map<EmergencyContact>(request.Contact);
        contact.EmployeeId = request.EmployeeId;

        _context.EmergencyContacts.Add(contact);
        await _context.SaveChangesAsync(cancellationToken);

        return contact.ContactId;
    }
}
