using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Requests.CancelShiftSwap;

/// <summary>
/// Command to cancel a pending shift swap request.
/// Only pending requests can be cancelled.
/// </summary>
public record CancelShiftSwapCommand(int RequestId) : IRequest<Result<bool>>;
