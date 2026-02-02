using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Requests.RevokeShiftSwap;

/// <summary>
/// Command to revoke an approved shift swap.
/// Reverts rosters back to original state.
/// </summary>
public record RevokeShiftSwapCommand(int RequestId) : IRequest<Result<bool>>;
