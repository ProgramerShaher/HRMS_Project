using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Requests.CreateShiftSwap;

/// <summary>
/// Command to create a shift swap request.
/// Allows employees to request exchanging shifts with colleagues.
/// </summary>
public record CreateShiftSwapCommand : IRequest<Result<int>>
{
    public int RequesterId { get; init; }
    public int TargetEmployeeId { get; init; }
    public DateTime RosterDate { get; init; }
    public string Reason { get; init; } = string.Empty;
}
