using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Features.Personnel.Employees.DTOs;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeFullProfile;

public record GetEmployeeFullProfileQuery(int EmployeeId) : IRequest<DetailedEmployeeProfileDto>;

public class GetEmployeeFullProfileQueryHandler : IRequestHandler<GetEmployeeFullProfileQuery, DetailedEmployeeProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeFullProfileQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DetailedEmployeeProfileDto> Handle(GetEmployeeFullProfileQuery request, CancellationToken cancellationToken)
    {
        // Fetch Aggregate Root with All Sub-Entities efficiently
        var employee = await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Job)
            .Include(e => e.Country) // For Nationality Name
            .Include(e => e.Compensation)
            .Include(e => e.Documents)
                .ThenInclude(d => d.DocumentType)
            .Include(e => e.Qualifications)
            .Include(e => e.Experiences)
            .Include(e => e.EmergencyContacts)
            .Include(e => e.Contracts)
            .Include(e => e.Certifications)
            .Include(e => e.BankAccounts)
            .Include(e => e.Dependents)
            .Include(e => e.Addresses)
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new Exceptions.NotFoundException($"Employee {request.EmployeeId} not found");

        // Construct DTO
        return new DetailedEmployeeProfileDto
        {
            CoreProfile = _mapper.Map<EmployeeDto>(employee),
            Compensation = _mapper.Map<EmployeeCompensationDto>(employee.Compensation),
            Qualifications = _mapper.Map<List<EmployeeQualificationDto>>(employee.Qualifications),
            Experiences = _mapper.Map<List<EmployeeExperienceDto>>(employee.Experiences),
            EmergencyContacts = _mapper.Map<List<EmergencyContactDto>>(employee.EmergencyContacts),
            Contracts = _mapper.Map<List<ContractDto>>(employee.Contracts),
            Certifications = _mapper.Map<List<EmployeeCertificationDto>>(employee.Certifications),
            BankAccounts = _mapper.Map<List<EmployeeBankAccountDto>>(employee.BankAccounts),
            Dependents = _mapper.Map<List<DependentDto>>(employee.Dependents),
            Addresses = _mapper.Map<List<EmployeeAddressDto>>(employee.Addresses),
            Documents = _mapper.Map<List<EmployeeDocumentDto>>(employee.Documents)
        };
    }
}
