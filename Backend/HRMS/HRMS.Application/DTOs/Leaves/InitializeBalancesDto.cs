namespace HRMS.Application.DTOs.Leaves;

/// <summary>
/// Data transfer object for bulk leave balance initialization request.
/// Used to grant leave entitlements to employees based on leave type configuration.
/// </summary>
public class InitializeBalancesDto
{
    /// <summary>
    /// Leave type identifier (e.g., Annual Leave, Sick Leave)
    /// </summary>
    public int LeaveTypeId { get; set; }

    /// <summary>
    /// Target year for balance initialization (e.g., 2026)
    /// </summary>
    public short Year { get; set; }

    /// <summary>
    /// Optional department filter. If null, initializes for all active employees.
    /// If specified, only employees in this department will receive the balance.
    /// </summary>
    public int? DepartmentId { get; set; }

    /// <summary>
    /// Optional custom days override. If null, uses DefaultDays from LeaveType configuration.
    /// Allows HR to grant different entitlement than the standard policy.
    /// </summary>
    public short? CustomDays { get; set; }

    /// <summary>
    /// Enable proration for employees hired mid-year.
    /// If true, calculates balance as: (DefaultDays / 12) * MonthsWorked
    /// </summary>
    public bool EnableProration { get; set; } = false;
}
