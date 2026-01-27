using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Personnel;

public class DependentDto
{
    public int? DependentId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullNameAr { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? FullNameEn { get; set; }

    [Required]
    [MaxLength(20)]
    public string Relationship { get; set; } = string.Empty; // Wife, Son, Daughter

    public DateTime BirthDate { get; set; }
    
    [MaxLength(20)]
    public string? NationalId { get; set; }

    public string? Gender { get; set; }
}
