namespace HRMS.Application.DTOs.Attendance;

/// <summary>
/// Data transfer object for shift type information.
/// Represents a work shift with its timing and configuration.
/// </summary>
public class ShiftTypeDto
{
    /// <summary>
    /// Shift identifier
    /// </summary>
    public int ShiftId { get; set; }

    /// <summary>
    /// Shift name in Arabic
    /// </summary>
    public string ShiftNameAr { get; set; } = string.Empty;

    /// <summary>
    /// Shift start time (HH:mm format)
    /// </summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>
    /// Shift end time (HH:mm format)
    /// </summary>
    public string EndTime { get; set; } = string.Empty;

    /// <summary>
    /// Total hours in shift (calculated)
    /// </summary>
    public decimal HoursCount { get; set; }

    /// <summary>
    /// Indicates if shift crosses midnight (1=Yes, 0=No)
    /// </summary>
    public byte IsCrossDay { get; set; }

    /// <summary>
    /// Grace period in minutes for late arrivals.
    /// NOTE: This is deprecated and will be removed. Use AttendancePolicy instead.
    /// </summary>
    [Obsolete("Grace period should be retrieved from AttendancePolicy, not ShiftType")]
    public short GracePeriodMins { get; set; }
}
