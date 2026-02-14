namespace HRMS.Application.DTOs.Payroll.Processing;

/// <summary>
/// ملخص مسير رواتب
/// Payroll Run Summary
/// </summary>
public class PayrollRunDto
{
    public int RunId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime ProcessDate { get; set; }
    public string Status { get; set; } = string.Empty; // DRAFT, APPROVED, POSTED
    public int EmployeeCount { get; set; }
    public decimal TotalGross { get; set; }
    public decimal TotalNet { get; set; }
    public string? ProcessedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
}

/// <summary>
/// تفاصيل مسير رواتب
/// Payroll Run Details
/// </summary>
public class PayrollRunDetailsDto
{
    public PayrollRunDto Run { get; set; } = new();
    public List<PayslipSummaryDto> Payslips { get; set; } = new();
    public PayrollRunSummary Summary { get; set; } = new();
}

public class PayslipSummaryDto
{
    public long PayslipId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
}

public class PayrollRunSummary
{
    public int TotalEmployees { get; set; }
    public decimal TotalBasicSalaries { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalGross { get; set; }
    public decimal TotalNet { get; set; }
    public decimal TotalLoanDeductions { get; set; }
    public decimal TotalOvertimePayments { get; set; }
}
