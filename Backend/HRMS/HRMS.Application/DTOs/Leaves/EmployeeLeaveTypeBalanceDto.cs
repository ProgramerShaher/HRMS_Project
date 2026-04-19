namespace HRMS.Application.DTOs.Leaves;

public class EmployeeLeaveTypeBalanceDto
{
    public int EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string EmployeeNameAr { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentNameAr { get; set; } = string.Empty;

    public int LeaveTypeId { get; set; }
    public string LeaveTypeNameAr { get; set; } = string.Empty;
    public short Year { get; set; }

    public decimal EntitlementDays { get; set; }
    public decimal ConsumedDays { get; set; }
    public decimal RemainingDays { get; set; }
}

