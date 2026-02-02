namespace HRMS.Application.DTOs.Attendance;

/// <summary>
/// Result object containing summary of monthly attendance closing operation
/// </summary>
public class MonthlyClosingResultDto
{
    /// <summary>
    /// Total number of employees processed
    /// </summary>
    public int TotalEmployeesProcessed { get; set; }

    /// <summary>
    /// Total late minutes charged (after grace period)
    /// </summary>
    public int TotalLateMinutesCharged { get; set; }

    /// <summary>
    /// Total overtime minutes calculated
    /// </summary>
    public int TotalOvertimeMinutes { get; set; }

    /// <summary>
    /// Period ID that was locked
    /// </summary>
    public int LockedPeriodId { get; set; }

    /// <summary>
    /// Timestamp when closing completed
    /// </summary>
    public DateTime ClosedAt { get; set; }
}
