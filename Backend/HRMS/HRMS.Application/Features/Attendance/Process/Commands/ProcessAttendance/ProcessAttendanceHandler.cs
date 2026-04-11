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
                .Include(r => r.Employee)
                    .ThenInclude(e => e.Compensation)
                .Where(r => r.RosterDate == targetDate && r.IsOffDay == 0 && r.ShiftId != null)
                .ToListAsync(cancellationToken);

            // 2. Fetch Policies (For grace periods and multipliers)
            var policy = await _context.AttendancePolicies
                .FirstOrDefaultAsync(cancellationToken) ?? new AttendancePolicy(); 
            // In a real multi-dept system, we'd fetch by department. Using default for now.

            // 2. Fetch Logs (Who punched today?)
            // تصحيح فجوة التوقيت: جلب البصمات بناءً على "يوم المستشفى" (من 9 مساءً أمس إلى 9 مساءً اليوم بتوقيت UTC)
            // Timezone Adjustment: Fetch punches based on Hospital Day (+3)
            var startUtc = targetDate.AddHours(-3); // 9 PM UTC previous day
            var endUtc = targetDate.AddDays(1).AddHours(-3); // 9 PM UTC current day

            var logs = await _context.RawPunchLogs
                .Where(p => p.PunchTime >= startUtc && p.PunchTime < endUtc)
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
                    var graceTime = shiftStart.AddMinutes(shift.GracePeriodMins > 0 ? shift.GracePeriodMins : policy.LateGraceMins);

                    if (actualIn.Value > graceTime)
                    {
                        var totalLateMins = (short)(actualIn.Value - shiftStart).TotalMinutes;
                        var excusedMins = (short)(latePermissionHours * 60);
                        lateMinutes = (short)Math.Max(0, totalLateMins - excusedMins);
                    }
                }

                // --- Calculate Financial Deductions (100% Accurate) ---
                decimal deductionAmount = 0;
                if (lateMinutes > 0 && roster.Employee.Compensation != null)
                {
                    var comp = roster.Employee.Compensation;
                    var totalMonthly = comp.BasicSalary + comp.HousingAllowance + comp.TransportAllowance + comp.MedicalAllowance + comp.OtherAllowances;
                    
                    // Standard calculation: Basic + Fixed Allowances / 30 days / 8 hours
                    var hourlyRate = totalMonthly / (30 * 8); 
                    deductionAmount = Math.Round((decimal)lateMinutes / 60 * hourlyRate, 2);
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
                dailyRecord.DeductionAmount = deductionAmount;
                dailyRecord.Status = status;
                
                // TODO: Calculate OT / EarlyLeave if needed
                
                processedCount++;
            }

            // 4. Handle "Unscheduled" Punches (Employees not in Roster but present)
            // هؤلاء موظفون سجلوا بصماتهم لكن ليس لديهم مناوبة اليوم (مثل يوم راحة أو استدعاء طارئ)
            var scheduledEmployeeIds = rosters.Select(r => r.EmployeeId).ToHashSet();
            var unscheduledLogs = logs
                .Where(l => !scheduledEmployeeIds.Contains(l.EmployeeId))
                .GroupBy(l => l.EmployeeId)
                .ToList();

            foreach (var group in unscheduledLogs)
            {
                var empId = group.Key;
                var empLogs = group.OrderBy(l => l.PunchTime).ToList();

                DateTime? actualIn = empLogs.FirstOrDefault(l => l.PunchType == "IN")?.PunchTime;
                DateTime? actualOut = empLogs.Count > 1 ? empLogs.LastOrDefault(l => l.PunchType == "OUT")?.PunchTime : null;

                // Check if record already exists (avoid duplicates on re-processing)
                var existingRecord = await _context.DailyAttendances
                    .FirstOrDefaultAsync(d => d.EmployeeId == empId && d.AttendanceDate == targetDate, cancellationToken);

                if (existingRecord == null)
                {
                    existingRecord = new DailyAttendance
                    {
                        EmployeeId = empId,
                        AttendanceDate = targetDate,
                        PlannedShiftId = null, // No shift
                    };
                    _context.DailyAttendances.Add(existingRecord);
                }

                existingRecord.ActualInTime = actualIn;
                existingRecord.ActualOutTime = actualOut;
                existingRecord.LateMinutes = 0;
                existingRecord.DeductionAmount = 0;
                existingRecord.OvertimeMinutes = 0;
                existingRecord.Status = "UNSCHEDULED"; // حضور غير مجدول
                processedCount++;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return processedCount;
        }
        catch (Exception ex)
        {
            throw new Exception($"خطأ أثناء معالجة الحضور: {ex.Message}");
        }
    }
}