namespace HRMS.Application.DTOs.Attendance;

public class PermissionRequestDto
{
    public int PermissionRequestId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime PermissionDate { get; set; }
    public string PermissionType { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApproverName { get; set; }
}

public class OvertimeRequestDto
{
    public int OtRequestId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime WorkDate { get; set; }
    public decimal HoursRequested { get; set; }
    public decimal? ApprovedHours { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? ApprovedBy { get; set; }
    public string? ApproverName { get; set; }
}

public class ShiftSwapRequestDto // Renamed from file if exists, or use this
{
    public int RequestId { get; set; }
    public int RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public int TargetEmployeeId { get; set; }
    public string TargetEmployeeName { get; set; } = string.Empty;
    public DateTime RosterDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ManagerComment { get; set; }
    public DateTime CreatedAt { get; set; }
}
