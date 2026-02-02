namespace HRMS.Application.DTOs.Payroll;

public class LoanInstallmentDto
{
    public long InstallmentId { get; set; }
    public int LoanId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal TotalLoanAmount { get; set; }
    public short InstallmentNumber { get; set; }
    public decimal InstallmentAmount { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsPaid { get; set; }
}
