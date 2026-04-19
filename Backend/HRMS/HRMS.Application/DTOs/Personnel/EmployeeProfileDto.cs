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
    public string? ProfilePicturePath { get; set; }
}

public class EmployeeCompensationDto
{
    public decimal BasicSalary { get; set; }
    
    /// <summary>
    /// إجمالي الراتب (الراتب الأساسي + جميع البدلات)
    /// </summary>
    public decimal TotalSalary => BasicSalary + HousingAllowance + TransportAllowance + MedicalAllowance + OtherAllowances;
    
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal MedicalAllowance { get; set; }
    
    /// <summary>
    /// البدلات الأخرى (تُضاف إلى إجمالي الراتب)
    /// </summary>
    /// <summary>
    /// البدلات الأخرى (تُضاف إلى إجمالي الراتب)
    /// </summary>
    public decimal OtherAllowances { get; set; }

    public int? BankId { get; set; }
    public string? IbanNumber { get; set; }
}


