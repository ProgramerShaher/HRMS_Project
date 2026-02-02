using HRMS.Application.DTOs.Leaves;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Leaves.Balances.Commands.InitializeBalances;

/// <summary>
/// Command to initialize leave balances for employees in bulk.
/// Supports department filtering, custom days override, and proration for mid-year hires.
/// </summary>
public record InitializeBalancesCommand : IRequest<Result<InitializeBalancesResultDto>>
{
    /// <summary>
    /// Leave type identifier
    /// </summary>
    public int LeaveTypeId { get; init; }

    /// <summary>
    /// Target year for balance initialization
    /// </summary>
    public short Year { get; init; }

    /// <summary>
    /// Optional department filter (null = all employees)
    /// </summary>
    public int? DepartmentId { get; init; }

    /// <summary>
    /// Optional custom days (null = use LeaveType.DefaultDays)
    /// </summary>
    public decimal? CustomDays { get; init; }

    /// <summary>
    /// Enable proration for employees hired mid-year
    /// </summary>
    public bool EnableProration { get; init; }
}
