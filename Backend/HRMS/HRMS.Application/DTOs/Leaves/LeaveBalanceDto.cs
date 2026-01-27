namespace HRMS.Application.DTOs.Leaves;

/// <summary>
/// نقل بيانات رصيد الإجازة للموظف
/// </summary>
public class LeaveBalanceDto
{
    public int BalanceId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;

    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;

    public decimal CurrentBalance { get; set; }
    public short Year { get; set; }
}
