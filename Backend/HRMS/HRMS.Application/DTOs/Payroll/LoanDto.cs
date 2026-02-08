namespace HRMS.Application.DTOs.Payroll;

/// <summary>
/// Loan Data Transfer Object
/// </summary>
public class LoanDto
{
    public int LoanId { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public decimal LoanAmount { get; set; }
    public DateTime RequestDate { get; set; }
    public short InstallmentCount { get; set; }
    public string? Status { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public int? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? SettlementDate { get; set; }
    public string? SettlementNotes { get; set; }
    
    /// <summary>
    /// قائمة الأقساط
    /// </summary>
    public List<LoanInstallmentDto>? Installments { get; set; }
    
    /// <summary>
    /// المبلغ المتبقي (الأقساط غير المدفوعة)
    /// </summary>
    public decimal RemainingAmount { get; set; }
    
    /// <summary>
    /// المبلغ المدفوع
    /// </summary>
    public decimal PaidAmount { get; set; }
}
