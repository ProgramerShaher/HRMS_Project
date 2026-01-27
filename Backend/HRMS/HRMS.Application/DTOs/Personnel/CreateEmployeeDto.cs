namespace HRMS.Application.DTOs.Personnel;

public class CreateEmployeeDto
{
    // --- Personal Information ---
    // EmployeeNumber generated automatically
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstNameAr { get; set; } = string.Empty;
    public string SecondNameAr { get; set; } = string.Empty;
    public string ThirdNameAr { get; set; } = string.Empty;
    public string LastNameAr { get; set; } = string.Empty;
    public string FullNameEn { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = "M";
    public string Mobile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? NationalityId { get; set; }
    public string NationalId { get; set; } = string.Empty;

    // --- Medical Information ---
    public string? LicenseNumber { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
    public string? Specialty { get; set; }

    // --- Employment Information ---
    public int DepartmentId { get; set; }
    public int JobId { get; set; }
    public DateTime HireDate { get; set; }
    public int? ManagerId { get; set; }

    // --- Compensation Information (Financial) ---
    public decimal BasicSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal MedicalAllowance { get; set; }
    public int? BankId { get; set; }
    public string? IbanNumber { get; set; }

    // Sub-Entities Collections
    public List<EmployeeQualificationDto> Qualifications { get; set; } = new();
    public List<EmployeeExperienceDto> Experiences { get; set; } = new();
    public List<EmergencyContactDto> EmergencyContacts { get; set; } = new();
    
    // New Collections
    public List<CreateContractDto> Contracts { get; set; } = new();
    public List<EmployeeCertificationDto> Certifications { get; set; } = new();
    public List<EmployeeBankAccountDto> BankAccounts { get; set; } = new();
    public List<DependentDto> Dependents { get; set; } = new();
    public List<EmployeeAddressDto> Addresses { get; set; } = new();
    public List<EmployeeDocumentDto> Documents { get; set; } = new();
}
