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
    
    // Attendance Info
    public TimeSpan? ActualIn { get; set; }
    public TimeSpan? ActualOut { get; set; }
    public string Status { get; set; } // Present, Absent, etc.
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
            .Where(x => x.EmployeeId == request.EmployeeId && x.RosterDate >= startOfMonth && x.RosterDate <= endOfNextMonth)
            .OrderBy(x => x.RosterDate)
            .ToListAsync(cancellationToken);

        // Fetch DailyAttendance for the same period
        // Note: Using DailyAttendance (processed) or RawPunchLogs (live). 
        // For "My Roster" which is history + today, DailyAttendance is better for history, but might lack today's live data if not processed.
        // However, usually we want to see "Final" status. Let's start with DailyAttendance.
        var attendanceRecords = await _context.DailyAttendances
            .AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId && x.AttendanceDate >= startOfMonth && x.AttendanceDate <= endOfNextMonth)
            .ToDictionaryAsync(x => x.AttendanceDate, cancellationToken);
            
        // Also fetch active punches for today if not in DailyAttendance? 
        // For now keep it simple: DailyAttendance is the source of truth for "Roster" view.

        var dtos = rosters.Select(r => {
            var att = attendanceRecords.ContainsKey(r.RosterDate) ? attendanceRecords[r.RosterDate] : null;
            
            return new MyRosterDto
            {
                Date = r.RosterDate,
                DayName = r.RosterDate.DayOfWeek.ToString(),
                ShiftName = r.ShiftType != null ? r.ShiftType.ShiftNameAr : "Off",
                StartTime = r.ShiftType != null && TimeSpan.TryParse(r.ShiftType.StartTime, out var start) ? start : TimeSpan.Zero,
                EndTime = r.ShiftType != null && TimeSpan.TryParse(r.ShiftType.EndTime, out var end) ? end : TimeSpan.Zero,
                IsOffDay = r.IsOffDay == 1,
                
                // Map Attendance
                ActualIn = att?.ActualInTime?.TimeOfDay,
                ActualOut = att?.ActualOutTime?.TimeOfDay,
                Status = att?.Status ?? (r.RosterDate < today ? "Absent" : "") // Infer Absent if past and no record
            };
        }).ToList();

        return Result<List<MyRosterDto>>.Success(dtos);
    }
}
