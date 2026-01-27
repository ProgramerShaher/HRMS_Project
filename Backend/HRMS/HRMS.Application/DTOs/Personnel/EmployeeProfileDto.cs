namespace HRMS.Application.DTOs.Personnel;

public class EmployeeProfileDto
{
    public int EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string FullNameEn { get; set; } = string.Empty;
    
    public string DepartmentName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    
    public EmployeeCompensationDto? Compensation { get; set; }
    public List<EmployeeDocumentDto> Documents { get; set; } = new();
}

public class EmployeeCompensationDto
{
    public decimal BasicSalary { get; set; }
    public decimal TotalSalary => BasicSalary + HousingAllowance + TransportAllowance + MedicalAllowance;
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal MedicalAllowance { get; set; }
}


