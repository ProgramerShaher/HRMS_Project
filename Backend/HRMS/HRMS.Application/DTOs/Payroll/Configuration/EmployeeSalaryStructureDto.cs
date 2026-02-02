namespace HRMS.Application.DTOs.Payroll.Configuration;

public class EmployeeStructureItemDto
{
    public int StructureId { get; set; }
    public int ElementId { get; set; }
    public string ElementNameAr { get; set; } = string.Empty;
    public string ElementType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

public class EmployeeSalaryStructureDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string JobTitleAr { get; set; } = string.Empty; // NEW
    public string GradeNameAr { get; set; } = string.Empty; // NEW

    public List<EmployeeStructureItemDto> Elements { get; set; } = new();
    
    public decimal TotalEarnings => Elements.Where(e => e.ElementType == "EARNING").Sum(e => e.Amount);
    public decimal TotalDeductions => Elements.Where(e => e.ElementType == "DEDUCTION").Sum(e => e.Amount);
    public decimal NetSalary => TotalEarnings - TotalDeductions;
}
