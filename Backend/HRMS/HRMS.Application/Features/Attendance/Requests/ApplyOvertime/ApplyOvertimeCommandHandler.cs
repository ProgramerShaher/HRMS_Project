using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Requests.ApplyOvertime;

/// <summary>
/// Handler for applying overtime request.
/// Validates payroll lock and creates pending request.
/// </summary>
public class ApplyOvertimeCommandHandler : IRequestHandler<ApplyOvertimeCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public ApplyOvertimeCommandHandler(IApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Result<int>> Handle(ApplyOvertimeCommand request, CancellationToken cancellationToken)
    {
        // التحقق من قفل الرواتب - منع التعديلات على الأشهر المغلقة
        // Check payroll lock - prevent modifications to closed months
        if (await _context.PayrollRuns.AnyAsync(
            p => p.Year == request.WorkDate.Year && 
                 p.Month == request.WorkDate.Month && 
                 (p.Status == "APPROVED" || p.Status == "PAID"), 
            cancellationToken))
            return Result<int>.Failure("الشهر المالي مغلق");

        var otRequest = new OvertimeRequest
        {
            EmployeeId = request.EmployeeId,
            WorkDate = request.WorkDate,
            HoursRequested = request.HoursRequested,
            Reason = request.Reason,
            Status = "PENDING"
        };

        _context.OvertimeRequests.Add(otRequest);
        await _context.SaveChangesAsync(cancellationToken);

        // إرسال تنبيه للمدير المباشر
        try
        {
            var employee = await _context.Employees
                .Include(e => e.Manager)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

            if (employee?.Manager != null && !string.IsNullOrEmpty(employee.Manager.UserId))
            {
                await _notificationService.SendAsync(
                    userId: employee.Manager.UserId,
                    title: "طلب عمل إضافي جديد",
                    message: $"قام الموظف {employee.FullNameAr} بتقديم طلب عمل إضافي بتاريخ {request.WorkDate:yyyy-MM-dd} لمدة {request.HoursRequested} ساعة."
                );
            }
        }
        catch (Exception)
        {
            // Log error silently
        }

        return Result<int>.Success(otRequest.OtRequestId, "تم تقديم طلب العمل الإضافي بنجاح");
    }
}
