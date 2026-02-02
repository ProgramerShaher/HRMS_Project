using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Attendance;

namespace HRMS.Application.Features.Attendance.Dashboard.Queries.GetAttendanceExceptions;

public class GetAttendanceExceptionsQueryHandler : IRequestHandler<GetAttendanceExceptionsQuery, Result<List<AttendanceExceptionDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAttendanceExceptionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

  public async Task<Result<List<AttendanceExceptionDto>>> Handle(GetAttendanceExceptionsQuery request, CancellationToken cancellationToken)
{
    // 1. تحديد نطاق البحث (آخر 30 يوم)
    var startDate = DateTime.Today.AddDays(-30);

    // 2. الاستعلام المباشر من SQL (Server-Side Filtering)
    var exceptions = await _context.DailyAttendances
        .Include(d => d.Employee)
        .Where(d => d.AttendanceDate >= startDate
                 && d.IsDeleted == 0
                 && (
                        // الحالة 1: بصمة دخول بدون خروج (ليوم مضى)
                        (d.ActualInTime != null && d.ActualOutTime == null && d.AttendanceDate < DateTime.Today)
                        || 
                        // الحالة 2: دوام قصير جداً (أقل من ساعة) - منطق "المسطرة"
                        (d.ActualInTime != null && d.ActualOutTime != null && d.ActualOutTime.Value.AddMinutes(-60) < d.ActualInTime)
                        || 
                        // الحالة 3: حالة مسجلة يدوياً كـ بصمة مفقودة
                        (d.Status == "MISSING_PUNCH")
                    )
        )
        .OrderByDescending(d => d.AttendanceDate)
        .ToListAsync(cancellationToken);

    // 3. تحويل النتائج إلى DTO (الآن الاسم العربي متاح بفضل تعديلنا السابق)
    var dtos = exceptions.Select(d => new AttendanceExceptionDto
    {
        RecordId = d.RecordId,
        EmployeeId = d.EmployeeId,
        EmployeeName = d.Employee.FullNameAr, // تم التفعيل بنجاح ✅
        Date = d.AttendanceDate,
        InTime = d.ActualInTime,
        OutTime = d.ActualOutTime,
        Issue = DetermineIssue(d)
    }).ToList();

    return Result<List<AttendanceExceptionDto>>.Success(dtos);
}
    private string DetermineIssue(DailyAttendance record)
    {
        if (record.Status == "MISSING_PUNCH") return "بصمة مفقودة";
        if (record.ActualInTime != null && record.ActualOutTime == null) return "لم يسجل خروج";
        if (record.ActualInTime != null && record.ActualOutTime != null)
        {
            var duration = (record.ActualOutTime.Value - record.ActualInTime.Value).TotalMinutes;
            if (duration < 60) return "دوام قصير جداً";
        }
        return "استثناء غير محدد";
    }
}
