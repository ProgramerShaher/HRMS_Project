using MediatR;
using HRMS.Core.Utilities;

namespace HRMS.Application.Features.Attendance.Dashboard.Queries.GetAttendanceExceptions;

public record AttendanceExceptionDto
{
    public long RecordId { get; init; }
    public int EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public DateTime? InTime { get; init; }
    public DateTime? OutTime { get; init; }
    public string Issue { get; init; } = string.Empty; // "Missing Out Punch", "Late > 2Hrs", etc.
}

public record GetAttendanceExceptionsQuery : IRequest<Result<List<AttendanceExceptionDto>>>;
