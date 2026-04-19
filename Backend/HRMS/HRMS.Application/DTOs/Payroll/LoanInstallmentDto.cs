namespace HRMS.Application.DTOs.Payroll;

/// <summary>
/// Loan Installment Data Transfer Object
/// </summary>
public class LoanInstallmentDto
{
    public long InstallmentId { get; set; }
    public int LoanId { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public decimal TotalLoanAmount { get; set; }
    public short InstallmentNumber { get; set; }
    public decimal InstallmentAmount { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "UNPAID";
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public int? PaidInPayrollRun { get; set; }
    public string? SettlementNotes { get; set; }
}
