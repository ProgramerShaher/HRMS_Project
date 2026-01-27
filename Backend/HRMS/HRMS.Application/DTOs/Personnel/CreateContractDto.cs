using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Personnel;

public class CreateContractDto
{
    public int? ContractId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [MaxLength(50)]
    public string? ContractType { get; set; } // FULL_TIME, PART_TIME

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsRenewable { get; set; } = true;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal BasicSalary { get; set; }

    [Range(0, double.MaxValue)]
    public decimal HousingAllowance { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TransportAllowance { get; set; }

    [Range(0, double.MaxValue)]
    public decimal OtherAllowances { get; set; }

    public short VacationDays { get; set; } = 30;
    public byte WorkingHoursDaily { get; set; } = 8;
}
