using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Personnel;

public class RenewContractDto
{
    [Required]
    public int ContractId { get; set; }

    [Required]
    public DateTime NewStartDate { get; set; }

    [Required]
    public DateTime NewEndDate { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
