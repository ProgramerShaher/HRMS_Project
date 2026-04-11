using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Punch.Queries.GetDeviceEmployees;

public class GetDeviceEmployeesQueryHandler : IRequestHandler<GetDeviceEmployeesQuery, Result<List<DeviceEmployeeDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetDeviceEmployeesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<DeviceEmployeeDto>>> Handle(GetDeviceEmployeesQuery request, CancellationToken cancellationToken)
    {
        var nowLocal = DateTime.UtcNow.AddHours(3);
        var today = nowLocal.Date;
        
        var startUtc = today.AddHours(-3);
        var endUtc = today.AddDays(1).AddHours(-3);

        // 1. Fetch Active Employees
        var employees = await _context.Employees
            .Include(e => e.Job)
            .Where(e => e.IsActive && e.IsDeleted == 0)
            .ToListAsync(cancellationToken);

        // 2. Fetch Today's Roster
        var todayRosters = await _context.EmployeeRosters
            .Include(r => r.ShiftType)
            .Where(r => r.RosterDate == today && r.IsDeleted == 0)
            .ToListAsync(cancellationToken);

        // 3. Fetch Today's Punches
        var todayPunches = await _context.RawPunchLogs
            .Where(p => p.PunchTime >= startUtc && p.PunchTime < endUtc)
            .OrderBy(p => p.PunchTime)
            .ToListAsync(cancellationToken);

        var resultList = new List<DeviceEmployeeDto>();

        foreach (var emp in employees)
        {
            var roster = todayRosters.FirstOrDefault(r => r.EmployeeId == emp.EmployeeId);
            var empPunches = todayPunches.Where(p => p.EmployeeId == emp.EmployeeId).ToList();

            var lastIn = empPunches.LastOrDefault(p => p.PunchType == "IN")?.PunchTime;
            var lastOut = empPunches.LastOrDefault(p => p.PunchType == "OUT")?.PunchTime;

            string currentShiftStr;
            if (roster != null)
            {
                if (roster.IsOffDay == 1)
                {
                    currentShiftStr = "يوم راحة";
                }
                else if (roster.ShiftType != null)
                {
                    currentShiftStr = $"{roster.ShiftType.ShiftNameAr} ({roster.ShiftType.StartTime} - {roster.ShiftType.EndTime})";
                }
                else
                {
                    currentShiftStr = roster.Status;
                }
            }
            else
            {
                currentShiftStr = "بدون مناوبة";
            }

            var status = "Out";
            // If there's an IN punch, and it's later than the OUT punch (or there is no OUT punch)
            var lastPunch = empPunches.LastOrDefault();
            if (lastPunch != null && lastPunch.PunchType == "IN")
            {
                status = "In";
            }

            resultList.Add(new DeviceEmployeeDto
            {
                EmployeeId = emp.EmployeeId,
                EmployeeNumber = emp.EmployeeNumber,
                FullNameAr = emp.FullNameAr, // Computed property evaluates safely in memory
                JobTitle = emp.Job?.JobTitleAr ?? "موظف",
                CurrentShift = currentShiftStr,
                LastPunchIn = lastIn,
                LastPunchOut = lastOut,
                Status = status
            });
        }

        return Result<List<DeviceEmployeeDto>>.Success(resultList);
    }
}
