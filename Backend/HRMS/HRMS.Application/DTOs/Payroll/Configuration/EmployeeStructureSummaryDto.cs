namespace HRMS.Application.DTOs.Payroll.Configuration;

/// <summary>
/// ملخص هيكل راتب موظف
/// Employee Salary Structure Summary
/// </summary>
public class EmployeeStructureSummaryDto
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeNameAr { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public string? JobTitle { get; set; }
    public decimal BasicSalary { get; set; }
    public int AllowancesCount { get; set; }
    public decimal TotalAllowances { get; set; }
    public int DeductionsCount { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }
    public bool HasStructure { get; set; }
    public DateTime? LastUpdated { get; set; }
}
