using MediatR;
using HRMS.Core.Utilities;

namespace HRMS.Application.Features.Attendance.Queries.GetDailyTimesheet;

public record TimesheetDayDto
{
    public DateTime Date { get; init; }
    public string DayName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty; // PRESENT, ABSENT, LEAVE, HOLIDAY, WEEKEND, REST_DAY
    public DateTime? InTime { get; init; }
    public DateTime? OutTime { get; init; }
    public int LateMinutes { get; init; }
    public int OTMinutes { get; init; }
    public string? Remarks { get; init; }
}

public record GetDailyTimesheetQuery(int EmployeeId, int Month, int Year) : IRequest<Result<List<TimesheetDayDto>>>;
