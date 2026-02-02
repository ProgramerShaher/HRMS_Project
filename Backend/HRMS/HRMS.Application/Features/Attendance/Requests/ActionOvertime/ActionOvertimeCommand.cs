using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Requests.ActionOvertime;

/// <summary>
/// Command to approve or reject overtime request.
/// Triggers attendance reprocessing on approval.
/// </summary>
public record ActionOvertimeCommand : IRequest<Result<bool>>
{
    public int RequestId { get; init; }
    public int ManagerId { get; init; }
    public string Action { get; init; } = string.Empty; // "APPROVE" or "REJECT"
    public decimal? ApprovedHours { get; init; }
    public string Comment { get; init; } = string.Empty;
}
