using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Personnel;

public class CreateBasicEmployeeDto
{
    // --- Mandatory Personal Information ---
    public string FirstNameAr { get; set; } = string.Empty;
    public string SecondNameAr { get; set; } = string.Empty;
    public string ThirdNameAr { get; set; } = string.Empty;
    public string LastNameAr { get; set; } = string.Empty;
    public string FullNameEn { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = "M"; // M/F
    public string Mobile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = "Single";

    // --- Mandatory Employment Information ---
    public int DepartmentId { get; set; }
    public int JobId { get; set; }
    public DateTime HireDate { get; set; }
}
