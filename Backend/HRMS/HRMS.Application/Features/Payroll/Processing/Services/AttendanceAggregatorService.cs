using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Processing.Services;

public class AttendanceAggregationResult
{
    public int AbsenceDays { get; set; }
    public int TotalLateMinutes { get; set; }
    public int TotalOvertimeMinutes { get; set; }
    public decimal AttendancePenalties { get; set; }
    public decimal OvertimeEarnings { get; set; }
    public List<string> Warnings { get; set; } = new();
    public bool IsBlocked { get; set; } // If Missing Punches exist
}

public class AttendanceAggregatorService
{
    private readonly IApplicationDbContext _context;

    public AttendanceAggregatorService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AttendanceAggregationResult> CalculateAttendanceImpactAsync(
        int employeeId, 
        DateTime startDate, 
        DateTime endDate, 
        decimal basicSalary,
        CancellationToken cancellationToken)
    {
        var result = new AttendanceAggregationResult();

        // 0. Base Financial Rates (High Precision)
        // Rule: Daily = Basic / 30, Hourly = Daily / 8, Minute = Hourly / 60
        // Use decimal(18, 4) for intermediate calcs
        if (basicSalary <= 0) return result;

        decimal dailyRate = basicSalary / 30m;
        decimal hourlyRate = dailyRate / 8m;
        decimal minuteRate = hourlyRate / 60m;

        // 1. Fetch Daily Attendance Logs
        var logs = await _context.DailyAttendances
            .Where(d => d.EmployeeId == employeeId 
                     && d.AttendanceDate >= startDate && d.AttendanceDate <= endDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // 2. Fetch Approved Leaves (to exclude from absence)
        var approvedLeaves = await _context.LeaveRequests
            .Where(l => l.EmployeeId == employeeId 
                     && l.Status == "HR_APPROVED"
                     && l.EndDate >= startDate && l.StartDate <= endDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // 3. Locking Check: Missing Punches
        if (logs.Any(l => l.Status == "MISSING_PUNCH"))
        {
            result.Warnings.Add($"الموظف لديه سجلات حضور غير مكتملة (بصمات مفقودة) في شهر المعالجة ({logs.Count(l => l.Status == "MISSING_PUNCH")} يوم).");
            result.IsBlocked = true;
            return result; // Stop processing for this employee
        }

        // 4. Calculate Absences
        // Count days marked as ABSENT in DailyAttendance
        // BUT verify they are not covered by an approved leave 
        // (Assuming DailyAttendance generation *already* checks leaves, but as a safety net we double check)
        
        int unexcusedAbsenceDays = 0;
        foreach (var log in logs.Where(l => l.Status == "ABSENT"))
        {
            bool isCoveredByLeave = approvedLeaves.Any(l => log.AttendanceDate >= l.StartDate && log.AttendanceDate <= l.EndDate);
            if (!isCoveredByLeave)
            {
                unexcusedAbsenceDays++;
            }
        }
        result.AbsenceDays = unexcusedAbsenceDays;
        
        // Penalty: Days * DailyRate * 1.0
        decimal absencePenalty = unexcusedAbsenceDays * dailyRate;


        // 5. Calculate Lateness
        int totalLateMinutes = logs.Sum(l => l.LateMinutes);
        result.TotalLateMinutes = totalLateMinutes;

        // Penalty: Minutes * MinuteRate * 1.0
        decimal latenessPenalty = totalLateMinutes * minuteRate;


        // 6. Calculate Overtime
        // Assuming OvertimeMinutes in DailyAttendance are "Approved" or system-generated valid OT
        int totalOvertimeMinutes = logs.Sum(l => l.OvertimeMinutes);
        result.TotalOvertimeMinutes = totalOvertimeMinutes;

        // Earnings: Minutes * MinuteRate * 1.5
        decimal overtimeEarnings = totalOvertimeMinutes * minuteRate * 1.5m;


        // 7. Final Aggregation (Round to 2 decimals for payment)
        result.AttendancePenalties = Math.Round(absencePenalty + latenessPenalty, 2);
        result.OvertimeEarnings = Math.Round(overtimeEarnings, 2);

        return result;
    }
}
