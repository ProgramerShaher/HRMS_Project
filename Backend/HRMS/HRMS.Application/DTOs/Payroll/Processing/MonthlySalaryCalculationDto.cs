using HRMS.Application.DTOs.Payroll.Configuration;

namespace HRMS.Application.DTOs.Payroll.Processing;

public class MonthlySalaryCalculationDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    
    public decimal BasicSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalStructureDeductions { get; set; }
    
    public decimal LoanDeductions { get; set; }
    public List<long> PaidInstallmentIds { get; set; } = new();

    public decimal AttendancePenalties { get; set; }
    public int AbsenceDays { get; set; }

    public decimal NetSalary => (BasicSalary + TotalAllowances) - (TotalStructureDeductions + LoanDeductions + AttendancePenalties);
}
