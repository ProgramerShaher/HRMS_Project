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

        // 0. جلب جميع الموظفين النشطين
        var allEmployees = await _context.Employees
            .Include(e => e.Department)
            .AsNoTracking()
            .Where(e => e.IsActive && e.IsDeleted == 0)
            .ToListAsync(cancellationToken);

        // 1. جلب بيانات الحضور
        var attendanceRecords = await _context.DailyAttendances
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
        var attendanceGroups = attendanceRecords.GroupBy(a => a.EmployeeId).ToDictionary(g => g.Key, g => g.ToList());
        var summary = new List<PayrollAttendanceSummaryDto>();

        foreach (var emp in allEmployees)
        {
            if (attendanceGroups.TryGetValue(emp.EmployeeId, out var g))
            {
                summary.Add(new PayrollAttendanceSummaryDto
                {
                    EmployeeId = emp.EmployeeId,
                    EmployeeName = emp.FullNameAr ?? "Unknown",
                    FullNameAr = emp.FullNameAr ?? "Unknown",
                    DepartmentName = emp.Department?.DeptNameAr ?? "N/A",
                    TotalDeepLateMinutes = g.Sum(x => x.LateMinutes > 15 ? x.LateMinutes : 0),
                    TotalShortLateMinutes = g.Sum(x => x.LateMinutes <= 15 ? x.LateMinutes : 0),
                    TotalOvertimeMinutes = g.Sum(x => x.OvertimeMinutes),
                    TotalAbsenceDays = g.Count(x => x.Status == "ABSENT" || x.Status == "MISSING_PUNCH"),
                    TotalSickLeaveDays = g.Count(x => x.Status == "LEAVE" && IsSickLeave(x, leaves)), 
                    TotalUnpaidLeaveDays = g.Count(x => x.Status == "LEAVE" && IsUnpaidLeave(x, leaves)),
                    ProposedDeductionAmount = g.Sum(x => x.DeductionAmount),
                    TotalOvertimeAmount = g.Sum(x => x.OvertimeAmount)
                });
            }
            else
            {
                // لا توجد سجلات حضور معالجة لهذا الموظف بعد
                summary.Add(new PayrollAttendanceSummaryDto
                {
                    EmployeeId = emp.EmployeeId,
                    EmployeeName = emp.FullNameAr ?? "Unknown",
                    FullNameAr = emp.FullNameAr ?? "Unknown",
                    DepartmentName = emp.Department?.DeptNameAr ?? "N/A",
                    TotalDeepLateMinutes = 0,
                    TotalShortLateMinutes = 0,
                    TotalOvertimeMinutes = 0,
                    TotalAbsenceDays = 0,
                    TotalSickLeaveDays = 0,
                    TotalUnpaidLeaveDays = 0,
                    ProposedDeductionAmount = 0,
                    TotalOvertimeAmount = 0
                });
            }
        }

        return Result<List<PayrollAttendanceSummaryDto>>.Success(summary.OrderBy(x => x.FullNameAr).ToList());
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
