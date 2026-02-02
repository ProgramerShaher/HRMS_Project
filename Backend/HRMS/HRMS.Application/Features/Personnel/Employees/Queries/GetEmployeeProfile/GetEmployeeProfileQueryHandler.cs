using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeProfile;

public class GetEmployeeProfileQueryHandler : IRequestHandler<GetEmployeeProfileQuery, EmployeeProfileDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeProfileQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EmployeeProfileDto?> Handle(GetEmployeeProfileQuery request, CancellationToken cancellationToken)
    {
        // Eager Loading for Sub-Entities (Compensation + Documents)
        var employee = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Job)
            .Include(e => e.Compensation)
            .Include(e => e.Documents)
                .ThenInclude(d => d.DocumentType)
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId && e.IsDeleted == 0, cancellationToken);

        if (employee == null) return null;

        var dto = _mapper.Map<EmployeeProfileDto>(employee);
        
        // Use centralized FullNameAr logic
        dto.FullNameAr = employee.FullNameAr;

        return dto;
    }
}
