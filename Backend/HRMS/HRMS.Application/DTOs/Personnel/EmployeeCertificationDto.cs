using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Personnel;

public class EmployeeCertificationDto
{
    public int? CertificationId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string CertificationName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? IssuingOrganization { get; set; }

    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    [MaxLength(50)]
    public string? CredentialId { get; set; }

    [MaxLength(200)]
    public string? CredentialUrl { get; set; }
}
