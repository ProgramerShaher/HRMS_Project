using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;

public class ProcessAttendanceHandler : IRequestHandler<ProcessAttendanceCommand, int>
{
    private readonly IApplicationDbContext _context;

    public ProcessAttendanceHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(ProcessAttendanceCommand request, CancellationToken cancellationToken)
    {
        var targetDate = request.TargetDate.Date;

        try
        {
            // 1. Fetch Rosters (Who is scheduled today?)
            var rosters = await _context.EmployeeRosters
                .Include(r => r.ShiftType)
                .Where(r => r.RosterDate == targetDate && r.IsOffDay == 0 && r.ShiftId != null)
                .ToListAsync(cancellationToken);

            // 2. Fetch Logs (Who punched today?)
            var logs = await _context.RawPunchLogs
                .Where(p => p.PunchTime.Date == targetDate)
                .OrderBy(p => p.PunchTime)
                .ToListAsync(cancellationToken);

            // 3. Process Each Employee in Roster
            int processedCount = 0;
            foreach (var roster in rosters)
            {
                var employeeLogs = logs.Where(l => l.EmployeeId == roster.EmployeeId).ToList();
                var shift = roster.ShiftType;

                // --- Determine In/Out ---
                DateTime? actualIn = employeeLogs.FirstOrDefault()?.PunchTime;
                DateTime? actualOut = employeeLogs.LastOrDefault()?.PunchTime;
                
                // If only one punch, treat as Missing Punch (unless logic allows single punch)
                if (employeeLogs.Count == 1) actualOut = null; 

                // --- Check Permissions ---
                var permissions = await _context.PermissionRequests
                    .Where(p => p.EmployeeId == roster.EmployeeId && p.PermissionDate == targetDate && p.Status == "Approved")
                    .ToListAsync(cancellationToken);

                decimal latePermissionHours = permissions.Where(p => p.PermissionType == "LateEntry").Sum(p => p.Hours);

                // --- Check Leaves ---
                var leaveRequest = await _context.LeaveRequests
                    .Where(l => l.EmployeeId == roster.EmployeeId 
                             && targetDate >= l.StartDate.Date 
                             && targetDate <= l.EndDate.Date 
                             && l.Status == "APPROVED")
                    .FirstOrDefaultAsync(cancellationToken);

                // --- Calculate Late Minutes ---
                short lateMinutes = 0;
                if (actualIn.HasValue && shift != null && TimeSpan.TryParse(shift.StartTime, out var startTime))
                {
                    var shiftStart = targetDate.Add(startTime);
                    var graceTime = shiftStart.AddMinutes(shift.GracePeriodMins);

                    if (actualIn.Value > graceTime)
                    {
                        var totalLateMins = (short)(actualIn.Value - shiftStart).TotalMinutes;
                        var excusedMins = (short)(latePermissionHours * 60);
                        lateMinutes = (short)Math.Max(0, totalLateMins - excusedMins);
                    }
                }

                // --- Determine Status (Ensuring 100% Accuracy for Reporting) ---
                string status = "Present";
                if (leaveRequest != null) 
                {
                    status = "Leave";
                }
                else if (!actualIn.HasValue) 
                {
                    // If it's a scheduled working day and no punch/leave, mark as Absent
                    status = "Absent";
                }
                else if (lateMinutes > 0) 
                {
                    status = "Late";
                }
                else if (actualIn.HasValue && !actualOut.HasValue) 
                {
                    status = "Missing Punch";
                }

                // --- Save/Update DailyAttendance ---
                var dailyRecord = await _context.DailyAttendances
                    .FirstOrDefaultAsync(d => d.EmployeeId == roster.EmployeeId && d.AttendanceDate == targetDate, cancellationToken);

                if (dailyRecord == null)
                {
                    dailyRecord = new DailyAttendance
                    {
                        EmployeeId = roster.EmployeeId,
                        AttendanceDate = targetDate,
                        PlannedShiftId = roster.ShiftId
                    };
                    _context.DailyAttendances.Add(dailyRecord);
                }

                dailyRecord.ActualInTime = actualIn;
                dailyRecord.ActualOutTime = actualOut;
                dailyRecord.LateMinutes = lateMinutes;
                dailyRecord.Status = status;
                
                // TODO: Calculate OT / EarlyLeave if needed
                
                processedCount++;
            }

            // 4. Handle "Unscheduled" Punches (Employees not in Roster but present) - Optional for now

            await _context.SaveChangesAsync(cancellationToken);
            return processedCount;
        }
        catch (Exception ex)
        {
            throw new Exception($"خطأ أثناء معالجة الحضور: {ex.Message}");
        }
    }
}