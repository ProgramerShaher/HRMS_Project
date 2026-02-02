using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Requests.ApplyOvertime;

/// <summary>
/// Command to apply for overtime work.
/// Creates a new overtime request pending manager approval.
/// </summary>
public record ApplyOvertimeCommand : IRequest<Result<int>>
{
    public int EmployeeId { get; init; }
    public DateTime WorkDate { get; init; }
    public decimal HoursRequested { get; init; }
    public string Reason { get; init; } = string.Empty;
}
