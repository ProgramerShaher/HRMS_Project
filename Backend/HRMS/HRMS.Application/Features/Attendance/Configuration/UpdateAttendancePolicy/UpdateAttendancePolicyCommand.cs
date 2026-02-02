using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.UpdateAttendancePolicy;

/// <summary>
/// Command to update an existing attendance policy.
/// Allows modification of policy rules including grace periods and overtime multipliers.
/// </summary>
public record UpdateAttendancePolicyCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// Policy identifier
    /// </summary>
    public int PolicyId { get; init; }

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
    public short LateGraceMins { get; init; }

    /// <summary>
    /// Overtime multiplier for regular days
    /// </summary>
    public decimal OvertimeMultiplier { get; init; }

    /// <summary>
    /// Overtime multiplier for weekends/holidays
    /// </summary>
    public decimal WeekendOtMultiplier { get; init; }
}
