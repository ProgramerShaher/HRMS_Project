using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Personnel;

public class EmployeeExperienceDto
{
    public int? ExperienceId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CompanyNameAr { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? JobTitleAr { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public byte IsCurrent { get; set; }

    [MaxLength(500)]
    public string? Responsibilities { get; set; }

    [MaxLength(200)]
    public string? ReasonForLeaving { get; set; }
}
