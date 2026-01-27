using AutoMapper;
using AutoMapper.QueryableExtensions;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Queries.EmergencyContacts;

public record GetEmployeeEmergencyContactsQuery(int EmployeeId) : IRequest<List<EmergencyContactDto>>;

public class GetEmployeeEmergencyContactsQueryHandler : IRequestHandler<GetEmployeeEmergencyContactsQuery, List<EmergencyContactDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeEmergencyContactsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<EmergencyContactDto>> Handle(GetEmployeeEmergencyContactsQuery request, CancellationToken cancellationToken)
    {
        return await _context.EmergencyContacts
            .Where(c => c.EmployeeId == request.EmployeeId)
            .ProjectTo<EmergencyContactDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
