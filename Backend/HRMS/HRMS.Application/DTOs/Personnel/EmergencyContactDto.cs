using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Personnel;

public class EmergencyContactDto
{
    public int? ContactId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ContactNameAr { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Relationship { get; set; }

    [Required]
    [MaxLength(20)]
    public string PhonePrimary { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneSecondary { get; set; }

    public byte IsPrimary { get; set; }
}
