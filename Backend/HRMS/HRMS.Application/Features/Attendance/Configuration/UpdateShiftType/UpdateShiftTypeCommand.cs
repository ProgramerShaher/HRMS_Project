using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.UpdateShiftType;

/// <summary>
/// Command to update an existing shift type.
/// Allows modification of shift timing and configuration.
/// </summary>
public record UpdateShiftTypeCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Shift identifier
    /// </summary>
    public int ShiftId { get; init; }

    /// <summary>
    /// Shift name in Arabic
    /// </summary>
    public string ShiftNameAr { get; init; } = string.Empty;

    /// <summary>
    /// Shift start time in HH:mm format
    /// </summary>
    public string StartTime { get; init; } = string.Empty;

    /// <summary>
    /// Shift end time in HH:mm format
    /// </summary>
    public string EndTime { get; init; } = string.Empty;

    /// <summary>
    /// Indicates if shift crosses midnight
    /// </summary>
    public byte IsCrossDay { get; init; }

    /// <summary>
    /// Grace period in minutes (deprecated - use AttendancePolicy)
    /// </summary>
    [Obsolete("Use AttendancePolicy.LateGraceMins instead")]
    public short GracePeriodMins { get; init; }
}
