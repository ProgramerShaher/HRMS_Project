using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Queries.GetAttendanceStats;

public class GetAttendanceStatsQueryHandler : IRequestHandler<GetAttendanceStatsQuery, Result<AttendanceStatsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAttendanceStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AttendanceStatsDto>> Handle(GetAttendanceStatsQuery request, CancellationToken cancellationToken)
    {
        // 1. تحديد بداية ونهاية الشهر
        // Determine start and end of month
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        if (endDate > DateTime.Today) endDate = DateTime.Today; // لا نحسب المستقبل

        // 2. جلب سجلات الحضور لهذا الموظف في الفترة المحددة
        // Fetch daily attendance records
        var records = await _context.DailyAttendances
            .AsNoTracking()
            .Where(d => d.EmployeeId == request.EmployeeId 
                     && d.AttendanceDate >= startDate 
                     && d.AttendanceDate <= endDate
                     && d.IsDeleted == 0)
            .ToListAsync(cancellationToken);

        // 3. حساب الإحصائيات
        // Calculate statistics
        var totalLate = records.Sum(r => r.LateMinutes);
        var totalOT = records.Sum(r => r.OvertimeMinutes);
        
        var presentDays = records.Count(r => r.Status == "PRESENT" || r.Status == "LATE"); // الحضور يشمل المتأخرين
        var absenceDays = records.Count(r => r.Status == "ABSENT");

        // نسبة الحضور = (أيام الحضور / إجمالي أيام العمل المخططة) * 100
        // Attendance Percentage calculation
        // هنا نفترض أن كل يوم له سجل DailyAttendance هو يوم عمل مخطط له
        // أو يمكن الاعتماد على TotalDays في الشهر (أقل دقة)
        var totalPlannedDays = records.Count(r => r.PlannedShiftId.HasValue);
        double percentage = 0;
        
        if (totalPlannedDays > 0)
        {
            percentage = Math.Round(((double)presentDays / totalPlannedDays) * 100, 1);
        }

        var dto = new AttendanceStatsDto(
            totalLate, 
            totalOT, 
            absenceDays, 
            presentDays,
            percentage
        );

        return Result<AttendanceStatsDto>.Success(dto);
    }
}
