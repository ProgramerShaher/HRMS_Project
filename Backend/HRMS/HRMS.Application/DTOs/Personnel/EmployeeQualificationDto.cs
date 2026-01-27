using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Personnel;

public class EmployeeQualificationDto
{
    public int? QualificationId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string DegreeType { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string MajorAr { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? UniversityAr { get; set; }

    public int? CountryId { get; set; }

    public short? GraduationYear { get; set; }

    [MaxLength(20)]
    public string? Grade { get; set; }
}
