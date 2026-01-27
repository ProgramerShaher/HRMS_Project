using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Personnel;

public class EmployeeBankAccountDto
{
    public int? EmployeeBankAccountId { get; set; }

    [Required]
    public int BankId { get; set; }

    [Required]
    [MaxLength(50)]
    public string AccountHolderName { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string IbanNumber { get; set; } = string.Empty;

    public bool IsPrimary { get; set; } = false;
}
