using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Requests.UpdateShiftSwap;

/// <summary>
/// Command to update a pending shift swap request.
/// Only pending requests can be modified.
/// </summary>
public record UpdateShiftSwapCommand : IRequest<Result<bool>>
{
    public int RequestId { get; init; }
    public int TargetEmployeeId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
