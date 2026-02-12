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

        var rosters = await (from r in _context.EmployeeRosters.Include(x => x.ShiftType)
                             join da in _context.DailyAttendances
                             on new { r.EmployeeId, JoinDate = r.RosterDate.Date } equals new { da.EmployeeId, JoinDate = da.AttendanceDate.Date } into attendanceGroup
                             from da in attendanceGroup.DefaultIfEmpty()
                             where r.EmployeeId == request.EmployeeId && r.RosterDate >= startOfMonth && r.RosterDate <= endOfNextMonth
                             orderby r.RosterDate
                             select new { r, da })
                             .ToListAsync(cancellationToken);

        var dtos = rosters.Select(item => new MyRosterDto
        {
            Date = item.r.RosterDate,
            DayName = item.r.RosterDate.DayOfWeek.ToString(),
            ShiftName = item.r.ShiftType != null ? item.r.ShiftType.ShiftNameAr : "Off",
            StartTime = item.r.ShiftType != null && TimeSpan.TryParse(item.r.ShiftType.StartTime, out var start) ? start : TimeSpan.Zero,
            EndTime = item.r.ShiftType != null && TimeSpan.TryParse(item.r.ShiftType.EndTime, out var end) ? end : TimeSpan.Zero,
            IsOffDay = item.r.IsOffDay == 1,
            ActualInTime = item.da?.ActualInTime,
            ActualOutTime = item.da?.ActualOutTime,
            Status = item.da?.Status ?? (item.r.IsOffDay == 1 ? "OFF" : "Scheduled")
        }).ToList();

        return Result<List<MyRosterDto>>.Success(dtos);
    }
}
