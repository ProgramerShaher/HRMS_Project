namespace HRMS.Application.DTOs.Payroll.Processing;

/// <summary>
/// ملخص شهري للرواتب
/// Monthly Payroll Summary
/// </summary>
public class MonthlyPayrollSummaryDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalEmployees { get; set; }
    public decimal TotalBasicSalaries { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalOvertimePayments { get; set; }
    public decimal TotalNetSalaries { get; set; }
    public List<DepartmentPayrollSummary> DepartmentBreakdown { get; set; } = new();
}

/// <summary>
/// ملخص رواتب قسم
/// Department Payroll Summary
/// </summary>
public class DepartmentPayrollSummary
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public decimal TotalBasicSalaries { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNetSalaries { get; set; }
}

/// <summary>
/// سجل تدقيق الرواتب
/// Payroll Audit Log
/// </summary>
public class PayrollAuditDto
{
    public long AuditId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? Notes { get; set; }
}
