namespace HRMS.Application.DTOs.Attendance;

/// <summary>
/// Data transfer object for attendance policy configuration.
/// Contains dynamic rules for late grace periods and overtime calculations.
/// </summary>
public class AttendancePolicyDto
{
    /// <summary>
    /// Unique policy identifier
    /// </summary>
    public int PolicyId { get; set; }

    /// <summary>
    /// Policy name in Arabic
    /// </summary>
    public string PolicyNameAr { get; set; } = string.Empty;

    /// <summary>
    /// Department ID this policy applies to (null for default policy)
    /// </summary>
    public int? DeptId { get; set; }

    /// <summary>
    /// Job ID this policy applies to (null for default policy)
    /// </summary>
    public int? JobId { get; set; }

    /// <summary>
    /// Late grace period in minutes (e.g., 15 means first 15 minutes are not charged)
    /// </summary>
    public short LateGraceMins { get; set; }

    /// <summary>
    /// Overtime multiplier for regular days (e.g., 1.5 means 150% of base rate)
    /// </summary>
    public decimal OvertimeMultiplier { get; set; }

    /// <summary>
    /// Overtime multiplier for weekends/holidays (e.g., 2.0 means 200% of base rate)
    /// </summary>
    public decimal WeekendOtMultiplier { get; set; }
}
