using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Dashboard.Queries.GetLiveAttendanceStatus;

public class GetLiveAttendanceStatusQueryHandler : IRequestHandler<GetLiveAttendanceStatusQuery, Result<LiveStatusDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLiveAttendanceStatusQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LiveStatusDto>> Handle(GetLiveAttendanceStatusQuery request, CancellationToken cancellationToken)
    {
        // استخدام الوقت المحلي للمستشفى (اليمن/السعودية +3)
        // Hospital Local Time (+3)
        var now = DateTime.UtcNow.AddHours(3);
        var today = now.Date; 


        // 1. الموظفين المجدولين لليوم (Scheduled Today)
        var scheduledStaffIds = await _context.EmployeeRosters
            .Where(r => r.RosterDate == today && r.IsOffDay == 0 && r.IsDeleted == 0)
            .Select(r => r.EmployeeId)
            .ToListAsync(cancellationToken);

        var totalScheduled = scheduledStaffIds.Count;

        // 2. الموظفين في إجازة معتمدة اليوم
        var onLeaveIds = await _context.LeaveRequests
            .Where(l => l.Status == "APPROVED" 
                     && l.IsDeleted == 0 
                     && l.StartDate <= today 
                     && l.EndDate >= today)
            .Select(l => l.EmployeeId)
            .ToListAsync(cancellationToken);

        // 3. تحليل البصمات الفعلية لليوم (Actual Punches)
        // تصحيح فجوة التوقيت: جلب البصمات بناءً على "يوم المستشفى المحلي +3"
        // Timezone Adjustment: Fetch punches within the local day window (Shifted UTC)
        var startUtc = today.AddHours(-3); 
        var endUtc = today.AddDays(1).AddHours(-3);

        var todayPunches = await _context.RawPunchLogs
            .AsNoTracking()
            .Where(p => p.PunchTime >= startUtc && p.PunchTime < endUtc)
            .ToListAsync(cancellationToken);

        var punchedEmployeeIds = todayPunches.Select(p => p.EmployeeId).Distinct().ToList();

        // 4. التصنيف الذكي (Smart Classification for 100% Accuracy)
        var currentlyInCount = 0;
        var checkedOutCount = 0;

        foreach (var empId in punchedEmployeeIds)
        {
            var lastPunch = todayPunches
                .Where(p => p.EmployeeId == empId)
                .OrderByDescending(p => p.PunchTime)
                .First();

            if (lastPunch.PunchType == "IN")
            {
                currentlyInCount++;
            }
            else
            {
                checkedOutCount++;
            }
        }

        // الغائبون أو الذين لم يحضروا بعد: من هم في الجدول المشغل، وليسوا في إجازة، ولم يبصموا حتى الآن
        var notPunchedIds = scheduledStaffIds.Except(punchedEmployeeIds).Except(onLeaveIds).ToList();
        
        // حساب "لم يحضر بعد" (إذا كان لا يزال الدوام مبكراً على سبيل المثال) أو مجرد الغياب الفعلي
        // دمجناهم للتبسيط بناءً على اللوحة السابقة، حيث أن غير الحاضرين هم notYetIn
        var notYetInCount = notPunchedIds.Count;
        var absentCount = notYetInCount;

        // الإجازات: من هم في إجازة اليوم
        var leaveCount = onLeaveIds.Distinct().Count();

        // إجمالي الموظفين النشطين
        var totalEmployees = await _context.Employees.CountAsync(e => e.IsActive && e.IsDeleted == 0, cancellationToken);

        return Result<LiveStatusDto>.Success(new LiveStatusDto(
            totalEmployees,
            currentlyInCount,
            checkedOutCount,
            notYetInCount, // Not yet in
            leaveCount,
            absentCount
        ));
    }
}
