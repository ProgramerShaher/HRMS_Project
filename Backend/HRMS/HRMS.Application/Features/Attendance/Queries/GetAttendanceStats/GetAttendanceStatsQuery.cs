using MediatR;
using HRMS.Core.Utilities;

namespace HRMS.Application.Features.Attendance.Queries.GetAttendanceStats;

public record AttendanceStatsDto(
    int TotalLateMinutes, 
    int TotalOTMinutes, 
    int TotalAbsenceDays, 
    int TotalPresentDays,
    double AttendancePercentage
);

public record GetAttendanceStatsQuery(int EmployeeId, int Month, int Year) : IRequest<Result<AttendanceStatsDto>>;
