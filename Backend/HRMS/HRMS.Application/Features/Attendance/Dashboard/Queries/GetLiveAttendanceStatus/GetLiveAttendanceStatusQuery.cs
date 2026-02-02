using MediatR;
using HRMS.Core.Utilities;

namespace HRMS.Application.Features.Attendance.Dashboard.Queries.GetLiveAttendanceStatus;

public record LiveStatusDto(
    int TotalEmployees,
    int CurrentlyIn,
    int CheckedOut,
    int NotYetIn,
    int OnLeave,
    int Absent // Confirmed absent (e.g. shift ended)
);

public record GetLiveAttendanceStatusQuery : IRequest<Result<LiveStatusDto>>;
