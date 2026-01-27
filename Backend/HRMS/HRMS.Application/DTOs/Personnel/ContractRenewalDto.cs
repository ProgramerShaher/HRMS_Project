using System;

namespace HRMS.Application.DTOs.Personnel;

public class ContractRenewalDto
{
    public int RenewalId { get; set; }
    public int ContractId { get; set; }
    public DateTime OldEndDate { get; set; }
    public DateTime NewStartDate { get; set; }
    public DateTime NewEndDate { get; set; }
    public DateTime RenewalDate { get; set; }
    public string? Notes { get; set; }
}
