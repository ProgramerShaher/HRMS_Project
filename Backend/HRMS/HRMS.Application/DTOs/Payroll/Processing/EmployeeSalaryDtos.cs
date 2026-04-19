namespace HRMS.Application.DTOs.Payroll.Processing;

/// <summary>
/// ملخص راتب موظف - للعرض في جدول شامل
/// Employee Salary Summary - For display in comprehensive table
/// </summary>
public class EmployeeSalaryDetailDto
{
    // معلومات الموظف
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeNameAr { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public string? JobTitle { get; set; }
    
    // هيكل الراتب
    public decimal BasicSalary { get; set; }
    public int AllowancesCount { get; set; }
    public int DeductionsCount { get; set; }
    
    // الإجماليات
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }
    
    // تأثير الحضور (آخر شهر محسوب)
    public decimal OvertimeAmount { get; set; }
    public decimal LatenessDeduction { get; set; }
    public decimal AbsenceDeduction { get; set; }
    
    // السلف
    public decimal MonthlyLoanDeduction { get; set; }
    public int ActiveLoansCount { get; set; }
    
    // الحالة
    public bool IsActive { get; set; }
    public DateTime? LastPayrollDate { get; set; }
    public bool HasSalaryStructure { get; set; }
}

/// <summary>
/// تفاصيل دقيقة لراتب موظف
/// Detailed salary breakdown for specific employee
/// </summary>
public class SalaryBreakdownDto
{
    public EmployeeBasicInfo Employee { get; set; } = new();
    
    // هيكل الراتب
    public decimal BasicSalary { get; set; }
    public List<ElementBreakdown> Earnings { get; set; } = new();
    public List<ElementBreakdown> Deductions { get; set; } = new();
    
    // تأثير الحضور
    public AttendanceImpact? Attendance { get; set; }
    
    // السلف
    public List<LoanDeduction> ActiveLoans { get; set; } = new();
    
    // الملخص
    public SalarySummary Summary { get; set; } = new();
}

public class EmployeeBasicInfo
{
    public int EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string EmployeeNameAr { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
    public string? JobTitle { get; set; }
    public string? Email { get; set; }
    public DateTime? HireDate { get; set; }
}

public class ElementBreakdown
{
    public int ElementId { get; set; }
    public string ElementNameAr { get; set; } = string.Empty;
    public string ElementType { get; set; } = string.Empty; // EARNING, DEDUCTION
    public decimal Amount { get; set; }
    public decimal? Percentage { get; set; }
    public bool IsTaxable { get; set; }
    public bool IsGosiBase { get; set; }
    public bool IsRecurring { get; set; }
}

public class AttendanceImpact
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int TotalWorkingDays { get; set; }
    public int ActualWorkingDays { get; set; }
    public int AbsenceDays { get; set; }
    public decimal AbsenceDeduction { get; set; }
    
    public int LateMinutes { get; set; }
    public decimal LatenessDeduction { get; set; }
    
    public decimal OvertimeHours { get; set; }
    public decimal OvertimeAmount { get; set; }
}

public class LoanDeduction
{
    public int LoanId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal MonthlyInstallment { get; set; }
    public decimal RemainingAmount { get; set; }
    public int RemainingInstallments { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class SalarySummary
{
    public decimal GrossSalary { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
    
    // تفصيل الخصومات
    public decimal StructureDeductions { get; set; }
    public decimal LoanDeductions { get; set; }
    public decimal AttendanceDeductions { get; set; }
    public decimal OtherDeductions { get; set; }
}
