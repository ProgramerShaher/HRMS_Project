using System;

namespace HRMS.Application.DTOs.Personnel;

public class ContractDto
{
    public int ContractId { get; set; }
    public int EmployeeId { get; set; }
    public string? ContractType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public byte IsRenewable { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal OtherAllowances { get; set; }
    public short VacationDays { get; set; }
    public byte WorkingHoursDaily { get; set; }
    public string? ContractStatus { get; set; }
}
