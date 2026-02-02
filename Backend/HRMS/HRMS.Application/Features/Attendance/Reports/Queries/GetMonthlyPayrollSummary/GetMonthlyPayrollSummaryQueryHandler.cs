using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Entities.Leaves;

namespace HRMS.Application.Features.Attendance.Reports.Queries.GetMonthlyPayrollSummary;

public class GetMonthlyPayrollSummaryQueryHandler : IRequestHandler<GetMonthlyPayrollSummaryQuery, Result<List<PayrollAttendanceSummaryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMonthlyPayrollSummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PayrollAttendanceSummaryDto>>> Handle(GetMonthlyPayrollSummaryQuery request, CancellationToken cancellationToken)
    {
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // 1. جلب بيانات الحضور
        var attendanceRecords = await _context.DailyAttendances
            .Include(d => d.Employee)
            .ThenInclude(e => e.Department)
            .AsNoTracking()
            .Where(d => d.AttendanceDate >= startDate 
                     && d.AttendanceDate <= endDate 
                     && d.IsDeleted == 0)
            .ToListAsync(cancellationToken);

        // 2. جلب بيانات الإجازات (للتدقيق الإضافي)
        var leaves = await _context.LeaveRequests
            .Include(l => l.LeaveType)
            .AsNoTracking()
            .Where(l => l.Status == "APPROVED"
                     && l.IsDeleted == 0
                     && l.StartDate <= endDate
                     && l.EndDate >= startDate)
            .ToListAsync(cancellationToken);


        // 3. التجميع حسب الموظف
        var summary = attendanceRecords
            .GroupBy(r => new { r.EmployeeId, EmployeeName = r.Employee.FullNameAr, Dept = r.Employee.Department?.DeptNameAr ?? "N/A" })
            .Select(g => new PayrollAttendanceSummaryDto
            {
                EmployeeId = g.Key.EmployeeId,
                EmployeeName = g.Key.EmployeeName ?? "Unknown",
                FullNameAr = g.Key.EmployeeName ?? "Unknown", // Reuse the key which is FullNameAr
                DepartmentName = g.Key.Dept,
                TotalDeepLateMinutes = g.Sum(x => x.LateMinutes > 15 ? x.LateMinutes : 0), // مثال: تأخير أكثر من 15 دقيقة
                TotalShortLateMinutes = g.Sum(x => x.LateMinutes <= 15 ? x.LateMinutes : 0),
                TotalOvertimeMinutes = g.Sum(x => x.OvertimeMinutes),
                TotalAbsenceDays = g.Count(x => x.Status == "ABSENT" || x.Status == "MISSING_PUNCH"),
                TotalSickLeaveDays = g.Count(x => x.Status == "LEAVE" && IsSickLeave(x, leaves)), // منطق تقريبي
                TotalUnpaidLeaveDays = g.Count(x => x.Status == "LEAVE" && IsUnpaidLeave(x, leaves)),
                ProposedDeductionAmount = 0 // يحتاج خوارزمية رواتب معقدة
            })
            .ToList();

        return Result<List<PayrollAttendanceSummaryDto>>.Success(summary);
    }

    private bool IsSickLeave(DailyAttendance record, List<LeaveRequest> leaves)
    {
        // البحث عن نوع الإجازة في هذا اليوم
        var leave = leaves.FirstOrDefault(l => l.EmployeeId == record.EmployeeId 
                                            && record.AttendanceDate >= l.StartDate 
                                            && record.AttendanceDate <= l.EndDate);
        return leave?.LeaveType?.LeaveNameEn.Contains("Sick") ?? false;
    }

    private bool IsUnpaidLeave(DailyAttendance record, List<LeaveRequest> leaves)
    {
        var leave = leaves.FirstOrDefault(l => l.EmployeeId == record.EmployeeId 
                                            && record.AttendanceDate >= l.StartDate 
                                            && record.AttendanceDate <= l.EndDate);
        return leave?.LeaveType?.LeaveNameEn.Contains("Unpaid") ?? false;
    }
}
