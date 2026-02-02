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
        var today = DateTime.Today;

        // 1. إحصائيات الموظفين النشطين
        var totalEmployees = await _context.Employees
            .CountAsync(e => e.IsDeleted == 0, cancellationToken);

        // 2. إحصائيات الإجازات اليوم
        var onLeave = await _context.LeaveRequests
            .CountAsync(l => l.Status == "APPROVED" 
                          && l.IsDeleted == 0 
                          && l.StartDate <= today 
                          && l.EndDate >= today, cancellationToken);

        // 3. تحليل البصمات لليوم (Raw Punches)
        // لمعرفة من "بالداخل" حالياً، نبحث عن آخر بصمة لكل موظف
        // To know who is currently IN, look for the last punch
        
        // ملاحظة: هذا يتطلب تجميع (GroupBy) وهو قد يكون ثقيلاً إذا كان الجدول ضخماً
        // الأفضل وجود جدول وسيط "CurrentStatus" ولكن سنستخدم RawPunchLogs للوقت الحالي
        
        var todayPunches = await _context.RawPunchLogs
            .AsNoTracking()
            .Where(p => p.PunchTime >= today && p.PunchTime < today.AddDays(1))
            .Select(p => new { p.EmployeeId, p.PunchType, p.PunchTime })
            .ToListAsync(cancellationToken);

        var employeePunches = todayPunches
            .GroupBy(p => p.EmployeeId)
            .Select(g => new 
            { 
                EmployeeId = g.Key, 
                LastPunch = g.OrderByDescending(p => p.PunchTime).First() 
            })
            .ToList();

        var currentlyIn = employeePunches.Count(ep => ep.LastPunch.PunchType == "IN" || ep.LastPunch.PunchType == "BREAK_IN"); // Adjust types as needed
        var checkedOut = employeePunches.Count(ep => ep.LastPunch.PunchType == "OUT" || ep.LastPunch.PunchType == "BREAK_OUT");

        // Absent / Not Yet In
        // Total - (OnLeave + In + Out) = Not accounted for
        // الفرق بين "لم يحضر بعد" و "غائب" يعتمد على وقت بداية الدوام
        // للتبسيط في الداشبورد، سنجمعهم في NotYetIn إذا لم ينته الدوام، و Absent إذا انتهى
        
        var notAccounted = totalEmployees - (onLeave + currentlyIn + checkedOut);
        var notYetIn = notAccounted > 0 ? notAccounted : 0; 

        // TODO: Refine "Absent" based on shift end time later logic

        return Result<LiveStatusDto>.Success(new LiveStatusDto(
            totalEmployees,
            currentlyIn,
            checkedOut,
            notYetIn,
            onLeave,
            0 // Placeholder for confirmed absent
        ));
    }
}
