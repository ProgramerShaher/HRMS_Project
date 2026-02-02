using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Requests.CancelOvertime;

/// <summary>
/// Command to cancel an overtime request.
/// Triggers attendance reprocessing if request was approved.
/// </summary>
public record CancelOvertimeCommand(int RequestId) : IRequest<Result<bool>>;
