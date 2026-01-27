using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

        // 1. Fetch Rosters for the day
        var rosters = await _context.EmployeeRosters
            .Include(r => r.ShiftType)
            .Where(r => r.RosterDate == targetDate)
            .ToListAsync(cancellationToken);

        if (!rosters.Any()) return 0; // Nothing to process

        var processedCount = 0;

        // 2. Pre-fetch Punch Logs for the relevant employees (Optimization: fetch range)
        // We fetch punches for TargetDate and TargetDate+1 (to cover CrossDay shifts)
        var employeeIds = rosters.Select(r => r.EmployeeId).Distinct().ToList();
        
        var startRange = targetDate;
        var endRange = targetDate.AddDays(2); // Cover up to +1 day end

        var allPunches = await _context.RawPunchLogs
            .Where(p => employeeIds.Contains(p.EmployeeId) 
                     && p.PunchTime >= startRange 
                     && p.PunchTime < endRange)
            .ToListAsync(cancellationToken);

        // 3. Pre-fetch Approved Leaves & Overtime Requests
        var activeLeaves = await _context.LeaveRequests
            .Where(l => employeeIds.Contains(l.EmployeeId)
                     && l.Status == "APPROVED"
                     && l.StartDate <= targetDate 
                     && l.EndDate >= targetDate)
            .ToListAsync(cancellationToken);

        var approvedOvertime = await _context.OvertimeRequests
            .Where(o => employeeIds.Contains(o.EmployeeId)
                     && o.Status == "APPROVED"
                     && o.WorkDate == targetDate)
            .ToListAsync(cancellationToken);

        // 4. Processing Loop
        var recordsToUpsert = new List<DailyAttendance>();

        foreach (var roster in rosters)
        {
            var empPunches = allPunches.Where(p => p.EmployeeId == roster.EmployeeId).ToList();
            var empLeave = activeLeaves.FirstOrDefault(l => l.EmployeeId == roster.EmployeeId);
            var empOT = approvedOvertime.FirstOrDefault(o => o.EmployeeId == roster.EmployeeId);

            var dailyRecord = new DailyAttendance
            {
                EmployeeId = roster.EmployeeId,
                AttendanceDate = targetDate,
                PlannedShiftId = roster.ShiftId
            };

            // Determine Shift Window
            DateTime? shiftStart = null;
            DateTime? shiftEnd = null;
            
            if (roster.ShiftType != null && !string.IsNullOrEmpty(roster.ShiftType.StartTime))
            {
                if (TimeSpan.TryParse(roster.ShiftType.StartTime, out var startTime))
                    shiftStart = targetDate.Add(startTime);
                
                if (TimeSpan.TryParse(roster.ShiftType.EndTime, out var endTime))
                {
                    shiftEnd = targetDate.Add(endTime);
                    if (roster.ShiftType.IsCrossDay == 1 && shiftEnd <= shiftStart)
                        shiftEnd = shiftEnd.Value.AddDays(1);
                }
            }

            // A. Shift Logic & Punch Matching
            if (shiftStart.HasValue && shiftEnd.HasValue)
            {
                // Define window with buffer (e.g., 4 hours before start, 6 hours after end)
                var lookupStart = shiftStart.Value.AddHours(-4);
                var lookupEnd = shiftEnd.Value.AddHours(6);

                var validPunches = empPunches
                    .Where(p => p.PunchTime >= lookupStart && p.PunchTime <= lookupEnd)
                    .OrderBy(p => p.PunchTime)
                    .ToList();

                if (validPunches.Any())
                {
                    dailyRecord.ActualInTime = validPunches.First().PunchTime;
                    dailyRecord.ActualOutTime = validPunches.Last().PunchTime != dailyRecord.ActualInTime 
                        ? validPunches.Last().PunchTime 
                        : null;

                    dailyRecord.Status = "PRESENT";

                    // Late Logic
                    if (dailyRecord.ActualInTime > shiftStart.Value.AddMinutes(roster.ShiftType.GracePeriodMins))
                    {
                        dailyRecord.LateMinutes = (short)(dailyRecord.ActualInTime.Value - shiftStart.Value).TotalMinutes;
                    }

                    // Early Leavel Logic
                    if (dailyRecord.ActualOutTime.HasValue && dailyRecord.ActualOutTime < shiftEnd.Value)
                    {
                        dailyRecord.EarlyLeaveMinutes = (short)(shiftEnd.Value - dailyRecord.ActualOutTime.Value).TotalMinutes;
                    }

                    // ═══════════════════════════════════════════════════════════
                    // OVERTIME CALCULATION (Core Requirement)
                    // ═══════════════════════════════════════════════════════════
                    if (empOT != null && dailyRecord.ActualOutTime.HasValue && dailyRecord.ActualOutTime > shiftEnd.Value)
                    {
                        var actualExtraMinutes = (dailyRecord.ActualOutTime.Value - shiftEnd.Value).TotalMinutes;
                        var approvedMinutes = empOT.ApprovedHours.HasValue ? (double)(empOT.ApprovedHours.Value * 60) : 0;
                        
                        // Min Rule
                        dailyRecord.OvertimeMinutes = (short)Math.Min(actualExtraMinutes, approvedMinutes);
                    }
                }
            }
            // B. Off Day or No Shift Logic
            else 
            {
                // If records found on Off Day -> Overtime or Present
                var dayPunches = empPunches.Where(p => p.PunchTime.Date == targetDate).OrderBy(p => p.PunchTime).ToList();
                if (dayPunches.Any())
                {
                   dailyRecord.ActualInTime = dayPunches.First().PunchTime;
                   dailyRecord.ActualOutTime = dayPunches.LastOrDefault()?.PunchTime;
                   dailyRecord.Status = "PRESENT"; // Or "EXTRA"
                }
            }

            // C. Final Status Fallback
            if (dailyRecord.Status == null)
            {
                if (empLeave != null)
                {
                    dailyRecord.Status = "LEAVE";
                }
                else if (roster.IsOffDay == 1)
                {
                    dailyRecord.Status = "OFF";
                }
                else
                {
                    dailyRecord.Status = "ABSENT";
                }
            }

            recordsToUpsert.Add(dailyRecord);
            processedCount++;
        }

        // 5. Upsert (Delete existing for day then add)
        // Optimization: In real world, merge/update. Here, simpler to remove for that day and re-add.
        
        var existingRecords = await _context.DailyAttendances
            .Where(d => d.AttendanceDate == targetDate && employeeIds.Contains(d.EmployeeId))
            .ToListAsync(cancellationToken);

        if (existingRecords.Any())
            _context.DailyAttendances.RemoveRange(existingRecords);

        await _context.DailyAttendances.AddRangeAsync(recordsToUpsert, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return processedCount;
    }
}
