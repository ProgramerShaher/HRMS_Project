namespace HRMS.Application.DTOs.Attendance;

/// <summary>
/// Data transfer object for shift swap request information.
/// Represents a request to exchange shifts between two employees.
/// </summary>
public class ShiftSwapRequestDto
{
    public int RequestId { get; set; }
    public int RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public int TargetEmployeeId { get; set; }
    public string TargetEmployeeName { get; set; } = string.Empty;
    public DateTime RosterDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ManagerComment { get; set; }
}
