namespace HRMS.Application.DTOs.Leaves;

/// <summary>
/// Result object containing summary of bulk leave balance initialization operation.
/// </summary>
public class InitializeBalancesResultDto
{
    /// <summary>
    /// Total number of employees evaluated for balance initialization
    /// </summary>
    public int TotalEmployeesProcessed { get; set; }

    /// <summary>
    /// Number of new balances successfully created
    /// </summary>
    public int BalancesCreated { get; set; }

    /// <summary>
    /// Number of employees skipped because they already have a balance for this leave type/year
    /// </summary>
    public int BalancesSkipped { get; set; }

    /// <summary>
    /// List of warning messages (e.g., employees with invalid data)
    /// </summary>
    public List<string> Warnings { get; set; } = new List<string>();

    /// <summary>
    /// Leave type name that was initialized
    /// </summary>
    public string LeaveTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Days granted per employee (may vary if proration is enabled)
    /// </summary>
    public decimal StandardDaysGranted { get; set; }
}
