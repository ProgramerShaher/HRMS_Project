using HRMS.Application.DTOs.Attendance;
using HRMS.Application.Interfaces;
using HRMS.Application.Features.Attendance.Queries.GetDailyTimesheet;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Queries.GetAttendanceByRange;

public record GetAttendanceByRangeQuery(int EmployeeId, DateTime StartDate, DateTime EndDate) : IRequest<Result<List<TimesheetDayDto>>>;

public class GetAttendanceByRangeQueryHandler : IRequestHandler<GetAttendanceByRangeQuery, Result<List<TimesheetDayDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAttendanceByRangeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TimesheetDayDto>>> Handle(GetAttendanceByRangeQuery request, CancellationToken cancellationToken)
    {
        var attendanceRecords = await _context.DailyAttendances
            .Where(a => a.EmployeeId == request.EmployeeId && a.AttendanceDate >= request.StartDate && a.AttendanceDate <= request.EndDate)
            .OrderBy(a => a.AttendanceDate)
            .ToListAsync(cancellationToken);

        var resultList = new List<TimesheetDayDto>();

        // We iterate through every day in range to show a complete table (even missing days)
        for (var date = request.StartDate.Date; date <= request.EndDate.Date; date = date.AddDays(1))
        {
            var record = attendanceRecords.FirstOrDefault(a => a.AttendanceDate.Date == date.Date);
            
            var dayDto = new TimesheetDayDto
            {
                Date = date,
                DayName = date.DayOfWeek.ToString(),
                Status = "ABSENT", // Default if no record
                Remarks = ""
            };

            if (record != null)
            {
                dayDto = dayDto with {
                    Status = record.Status,
                    InTime = record.ActualInTime,
                    OutTime = record.ActualOutTime,
                    LateMinutes = record.LateMinutes,
                    OTMinutes = record.OvertimeMinutes,
                    Remarks = FormatRemarks(record)
                };
            }
            else 
            {
               // Check if it's weekend (Simplified logic: Friday/Saturday or Saturday/Sunday depending on setup)
               // For now using simple logic:
               if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
               {
                   dayDto = dayDto with { Status = "WEEKEND" };
               }
            }

            resultList.Add(dayDto);
        }

        return Result<List<TimesheetDayDto>>.Success(resultList);
    }

    private string FormatRemarks(HRMS.Core.Entities.Attendance.DailyAttendance record)
    {
        var parts = new List<string>();
        if (record.LateMinutes > 0) parts.Add($"Late: {record.LateMinutes}m");
        if (record.OvertimeMinutes > 0) parts.Add($"OT: {record.OvertimeMinutes}m");
        if (record.EarlyLeaveMinutes > 0) parts.Add($"Early: {record.EarlyLeaveMinutes}m");
        return string.Join(", ", parts);
    }
}
