using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Requests.ActionShiftSwap;

/// <summary>
/// Command to approve or reject shift swap request.
/// Swaps rosters on approval.
/// </summary>
public record ActionShiftSwapCommand : IRequest<Result<bool>>
{
    public int RequestId { get; init; }
    public int ManagerId { get; init; }
    public string Action { get; init; } = string.Empty; // "APPROVE" or "REJECT"
    public string Comment { get; init; } = string.Empty;

    
}
