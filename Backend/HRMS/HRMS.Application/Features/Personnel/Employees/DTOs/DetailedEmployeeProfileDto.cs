using HRMS.Application.DTOs.Personnel;

namespace HRMS.Application.Features.Personnel.Employees.DTOs;

public class DetailedEmployeeProfileDto
{
    public EmployeeDto CoreProfile { get; set; }
    
    public EmployeeCompensationDto? Compensation { get; set; }

    public List<EmployeeQualificationDto> Qualifications { get; set; } = new();
    public List<EmployeeExperienceDto> Experiences { get; set; } = new();
    public List<EmergencyContactDto> EmergencyContacts { get; set; } = new();
    
    public List<ContractDto> Contracts { get; set; } = new();
    public List<EmployeeCertificationDto> Certifications { get; set; } = new();
    public List<EmployeeBankAccountDto> BankAccounts { get; set; } = new();
    public List<DependentDto> Dependents { get; set; } = new();
    public List<EmployeeAddressDto> Addresses { get; set; } = new();
    public List<EmployeeDocumentDto> Documents { get; set; } = new();
}
