namespace HRMS.Application.DTOs.Payroll.Processing;

public class BankFileRecordDto
{
    public string EmployeeNameAr { get; set; } = string.Empty;
    public string EmployeeNameEn { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? Iban { get; set; }
    public string BankName { get; set; } = string.Empty;
    public decimal NetSalary { get; set; }
    public string Currency { get; set; } = "YER";
    public string PaymentReference { get; set; } = string.Empty;
}
