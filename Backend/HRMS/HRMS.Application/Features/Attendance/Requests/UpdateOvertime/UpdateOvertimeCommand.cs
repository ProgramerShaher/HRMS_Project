using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Requests.UpdateOvertime;

/// <summary>
/// Command to update a pending overtime request.
/// Only pending requests can be modified.
/// </summary>
public record UpdateOvertimeCommand : IRequest<Result<bool>>
{
    public int RequestId { get; init; }
    public decimal HoursRequested { get; init; }
    public string Reason { get; init; } = string.Empty;
}
