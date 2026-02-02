using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Punch.RegisterPunch;

/// <summary>
/// Command to register employee punch (clock in/out).
/// Records punch time and triggers attendance processing.
/// </summary>
public record RegisterPunchCommand : IRequest<Result<long>>
{
    /// <summary>
    /// Employee identifier
    /// </summary>
    public int EmployeeId { get; init; }

    /// <summary>
    /// Punch type: "IN" for clock in, "OUT" for clock out
    /// </summary>
    public string PunchType { get; init; } = string.Empty;

    /// <summary>
    /// Punch timestamp
    /// </summary>
    public DateTime PunchTime { get; init; }

    /// <summary>
    /// Optional device identifier (e.g., fingerprint scanner ID)
    /// </summary>
    public string? DeviceId { get; init; }

    /// <summary>
    /// Optional GPS coordinates for location-based attendance
    /// </summary>
    public string? LocationCoordinates { get; init; }
}
