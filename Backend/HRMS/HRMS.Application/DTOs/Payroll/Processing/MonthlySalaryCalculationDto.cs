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
    public int TotalLateMinutes { get; set; }
    public int TotalOvertimeMinutes { get; set; }
    public decimal OvertimeEarnings { get; set; }

    // Warnings (e.g., Missing Punches)
    public List<string> Warnings { get; set; } = new();

    public decimal NetSalary => (BasicSalary + TotalAllowances + OvertimeEarnings) - (TotalStructureDeductions + LoanDeductions + AttendancePenalties);
}
