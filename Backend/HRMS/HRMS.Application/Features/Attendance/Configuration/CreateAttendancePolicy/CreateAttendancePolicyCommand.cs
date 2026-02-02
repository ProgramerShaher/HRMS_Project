using HRMS.Application.DTOs.Attendance;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.CreateAttendancePolicy;

/// <summary>
/// Command to create a new attendance policy with dynamic rules.
/// Policies define late grace periods and overtime multipliers per department/job.
/// </summary>
public record CreateAttendancePolicyCommand : IRequest<Result<int>>
{
    /// <summary>
    /// Policy name in Arabic
    /// </summary>
    public string PolicyNameAr { get; init; } = string.Empty;

    /// <summary>
    /// Department ID (null for default policy)
    /// </summary>
    public int? DeptId { get; init; }

    /// <summary>
    /// Job ID (null for default policy)
    /// </summary>
    public int? JobId { get; init; }

    /// <summary>
    /// Late grace period in minutes
    /// </summary>
    public short LateGraceMins { get; init; } = 15;

    /// <summary>
    /// Overtime multiplier for regular days
    /// </summary>
    public decimal OvertimeMultiplier { get; init; } = 1.5m;

    /// <summary>
    /// Overtime multiplier for weekends/holidays
    /// </summary>
    public decimal WeekendOtMultiplier { get; init; } = 2.0m;
}
