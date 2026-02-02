using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.CreateShiftType;

/// <summary>
/// Command to create a new shift type with timing configuration.
/// Defines work shift hours and cross-day behavior.
/// </summary>
public record CreateShiftTypeCommand : IRequest<Result<int>>
{
    /// <summary>
    /// Shift name in Arabic
    /// </summary>
    public string ShiftNameAr { get; init; } = string.Empty;

    /// <summary>
    /// Shift start time in HH:mm format (e.g., "08:00")
    /// </summary>
    public string StartTime { get; init; } = string.Empty;

    /// <summary>
    /// Shift end time in HH:mm format (e.g., "16:00")
    /// </summary>
    public string EndTime { get; init; } = string.Empty;

    /// <summary>
    /// Indicates if shift crosses midnight (1=Yes, 0=No).
    /// Example: Night shift from 22:00 to 06:00
    /// </summary>
    public byte IsCrossDay { get; init; }

    /// <summary>
    /// Grace period in minutes for late arrivals.
    /// NOTE: This field is deprecated. Grace period should be defined in AttendancePolicy.
    /// Kept for backward compatibility only.
    /// </summary>
    [Obsolete("Use AttendancePolicy.LateGraceMins instead")]
    public short GracePeriodMins { get; init; } = 0;
}
