using HRMS.Application.DTOs.Attendance; // Need to create RosterDto or use EmployeeRoster
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Roster.Queries.GetMyRoster;

public class GetMyRosterQuery : IRequest<Result<List<MyRosterDto>>>
{
    public int EmployeeId { get; set; }
}

public class MyRosterDto
{
    public DateTime Date { get; set; }
    public string DayName { get; set; } = string.Empty;
    public string ShiftName { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsOffDay { get; set; }
    public DateTime? ActualInTime { get; set; }
    public DateTime? ActualOutTime { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class GetMyRosterQueryHandler : IRequestHandler<GetMyRosterQuery, Result<List<MyRosterDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMyRosterQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MyRosterDto>>> Handle(GetMyRosterQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var endOfNextMonth = startOfMonth.AddMonths(2).AddDays(-1);

        var rosters = await _context.EmployeeRosters
            .Include(x => x.ShiftType)
            .AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId && x.RosterDate >= startOfMonth && x.RosterDate <= endOfNextMonth)
            .OrderBy(x => x.RosterDate)
            .ToListAsync(cancellationToken);

        // Fetch DailyAttendance for the same period
        // Note: Using DailyAttendance (processed) as source of truth for "Roster" view.
        var attendanceRecords = await _context.DailyAttendances
            .AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId && x.AttendanceDate >= startOfMonth && x.AttendanceDate <= endOfNextMonth)
            .ToDictionaryAsync(x => x.AttendanceDate.Date, cancellationToken);

        var dtos = rosters.Select(r => {
            var att = attendanceRecords.ContainsKey(r.RosterDate.Date) ? attendanceRecords[r.RosterDate.Date] : null;
            
            return new MyRosterDto
            {
                Date = r.RosterDate,
                DayName = r.RosterDate.DayOfWeek.ToString(),
                ShiftName = r.ShiftType != null ? r.ShiftType.ShiftNameAr : "Off",
                StartTime = r.ShiftType != null && TimeSpan.TryParse(r.ShiftType.StartTime, out var start) ? start : TimeSpan.Zero,
                EndTime = r.ShiftType != null && TimeSpan.TryParse(r.ShiftType.EndTime, out var end) ? end : TimeSpan.Zero,
                IsOffDay = r.IsOffDay == 1,
                
                // Map Attendance
                ActualInTime = att?.ActualInTime,
                ActualOutTime = att?.ActualOutTime,
                Status = att?.Status ?? (r.RosterDate.Date < today ? "Absent" : (r.IsOffDay == 1 ? "OFF" : "Scheduled"))
            };
        }).ToList();

        return Result<List<MyRosterDto>>.Success(dtos);
    }
}

     
