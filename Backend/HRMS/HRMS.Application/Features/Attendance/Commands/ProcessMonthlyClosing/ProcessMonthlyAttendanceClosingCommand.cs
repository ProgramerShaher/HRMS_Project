using HRMS.Application.DTOs.Attendance;
using MediatR;

namespace HRMS.Application.Features.Attendance.Commands.ProcessMonthlyClosing;

/// <summary>
/// Command to process and lock monthly attendance for payroll integration.
/// Implements dynamic policy-driven calculations with zero hardcoded values.
/// </summary>
public record ProcessMonthlyAttendanceClosingCommand : IRequest<MonthlyClosingResultDto>
{
    /// <summary>
    /// Target year for closing (e.g., 2026)
    /// </summary>
    public short Year { get; init; }

    /// <summary>
    /// Target month for closing (1-12)
    /// </summary>
    public byte Month { get; init; }

    /// <summary>
    /// User ID performing the closing operation
    /// </summary>
    public int ClosedByUserId { get; init; }
}
