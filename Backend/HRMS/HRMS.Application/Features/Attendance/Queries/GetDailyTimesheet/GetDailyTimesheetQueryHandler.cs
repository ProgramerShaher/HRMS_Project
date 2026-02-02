using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Attendance;

namespace HRMS.Application.Features.Attendance.Queries.GetDailyTimesheet;

public class GetDailyTimesheetQueryHandler : IRequestHandler<GetDailyTimesheetQuery, Result<List<TimesheetDayDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetDailyTimesheetQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TimesheetDayDto>>> Handle(GetDailyTimesheetQuery request, CancellationToken cancellationToken)
    {
        var resultList = new List<TimesheetDayDto>();

        // 1. تحديد النطاق الزمني
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        
        // 2. جلب البيانات من الجداول المختلفة دفعة واحدة لتقليل الاستعلامات
        
        // أ) الحضور اليومي
        var attendanceRecords = await _context.DailyAttendances
            .AsNoTracking()
            .Where(d => d.EmployeeId == request.EmployeeId 
                     && d.AttendanceDate >= startDate 
                     && d.AttendanceDate <= endDate
                     && d.IsDeleted == 0)
            .ToDictionaryAsync(d => d.AttendanceDate.Date, cancellationToken);

        // ب) العطل الرسمية
        var holidays = await _context.PublicHolidays
            .AsNoTracking()
            .Where(h => h.IsDeleted == 0 
                     && h.StartDate <= endDate 
                     && h.EndDate >= startDate)
            .ToListAsync(cancellationToken);

        // ج) الإجازات المعتمدة
        var leaves = await _context.LeaveRequests
            .AsNoTracking()
            .Where(l => l.EmployeeId == request.EmployeeId
                     && l.Status == "APPROVED" // أو MANAGER_APPROVED حسب المنطق
                     && l.IsDeleted == 0
                     && l.StartDate <= endDate 
                     && l.EndDate >= startDate)
            .Include(l => l.LeaveType)
            .ToListAsync(cancellationToken);

        // 3. بناء الجدول اليومي
        // Iterate through each day of the month
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dayDto = new TimesheetDayDto
            {
                Date = date,
                DayName = date.DayOfWeek.ToString() // يمكن تعريبه لاحقاً أو في الواجهة
            };

            // المنطق الطبقي لتحديد الحالة (Layered Logic)
            // الأولوية: 1. سجل الحضور الموجود 2. عطلة رسمية 3. إجازة 4. عطلة نهاية أسبوع 5. غياب

            if (attendanceRecords.TryGetValue(date, out var record))
            {
                // يوجد سجل حضور
                dayDto = dayDto with {
                    Status = record.Status ?? "UNKNOWN",
                    InTime = record.ActualInTime,
                    OutTime = record.ActualOutTime,
                    LateMinutes = record.LateMinutes,
                    OTMinutes = record.OvertimeMinutes,
                    Remarks = FormatRemarks(record)
                };
            }
            else
            {
                // لا يوجد سجل حضور، نستنتج الحالة
                
                // أ) هل هو عطلة رسمية؟
                var holiday = holidays.FirstOrDefault(h => date >= h.StartDate && date <= h.EndDate);
                if (holiday != null)
                {
                    dayDto = dayDto with { Status = "HOLIDAY", Remarks = holiday.HolidayNameAr };
                }
                // ب) هل هو إجازة معتمدة؟
                else 
                {
                    var leave = leaves.FirstOrDefault(l => date >= l.StartDate && date <= l.EndDate);
                    if (leave != null)
                    {
                        dayDto = dayDto with { Status = "LEAVE", Remarks = leave.LeaveType?.LeaveNameAr ?? "Leave" };
                    }
                    // ج) هل هو عطلة نهاية أسبوع؟ (الجمعة والسبت مثلاً)
                    // TODO: يجب جلب هذا من إعدادات المناوبة أو النظام، سنفترض الجمعة/السبت افتراضياً
                    else if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dayDto = dayDto with { Status = "WEEKEND" };
                    }
                    else if (date > DateTime.Today)
                    {
                        dayDto = dayDto with { Status = "FUTURE" };
                    }
                    else
                    {
                        dayDto = dayDto with { Status = "ABSENT" };
                    }
                }
            }

            resultList.Add(dayDto);
        }

        return Result<List<TimesheetDayDto>>.Success(resultList);
    }

    private string FormatRemarks(DailyAttendance record)
    {
        var parts = new List<string>();
        if (record.LateMinutes > 0) parts.Add($"Late: {record.LateMinutes}m");
        if (record.OvertimeMinutes > 0) parts.Add($"OT: {record.OvertimeMinutes}m");
        if (record.EarlyLeaveMinutes > 0) parts.Add($"Early: {record.EarlyLeaveMinutes}m");
        return string.Join(", ", parts);
    }
}
