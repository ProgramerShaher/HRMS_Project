using HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Requests.ActionOvertime;

/// <summary>
/// Handler for approving/rejecting overtime requests.
/// Reprocesses attendance on approval to calculate overtime pay.
/// </summary>
public class ActionOvertimeCommandHandler : IRequestHandler<ActionOvertimeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly INotificationService _notificationService;

    public ActionOvertimeCommandHandler(IApplicationDbContext context, IMediator mediator, INotificationService notificationService)
    {
        _context = context;
        _mediator = mediator;
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(ActionOvertimeCommand request, CancellationToken cancellationToken)
    {
        var otRequest = await _context.OvertimeRequests
            .Include(r => r.Employee)
            .FirstOrDefaultAsync(r => r.OtRequestId == request.RequestId, cancellationToken);

        if (otRequest == null) return Result<bool>.Failure("الطلب غير موجود");

        if (otRequest.Status != "PENDING") 
            return Result<bool>.Failure("لا يمكن اتخاذ إجراء على طلب غير معلق");

        // التحقق من قفل الرواتب
        // Check payroll lock
        if (await _context.PayrollRuns.AnyAsync(
            p => p.Year == otRequest.WorkDate.Year && 
                 p.Month == otRequest.WorkDate.Month && 
                 (p.Status == "APPROVED" || p.Status == "PAID"), 
            cancellationToken))
            return Result<bool>.Failure("الشهر المالي مغلق");

        // تحديث حالة الطلب
        // Update request status
        if (request.Action == "APPROVE")
        {
            otRequest.Status = "APPROVED";
            otRequest.ApprovedHours = request.ApprovedHours;
            otRequest.ApprovedBy = request.ManagerId;
        }
        else
        {
            otRequest.Status = "REJECTED";
        }

        await _context.SaveChangesAsync(cancellationToken);

        // إرسال تنبيه للموظف
        try
        {
            if (otRequest.Employee?.UserId != null)
            {
                var title = request.Action == "APPROVE" ? "موافقة على العمل الإضافي" : "رفض طلب العمل الإضافي";
                var message = request.Action == "APPROVE" 
                    ? $"تمت الموافقة على طلب العمل الإضافي ليوم {otRequest.WorkDate:yyyy-MM-dd}."
                    : $"تم رفض طلب العمل الإضافي ليوم {otRequest.WorkDate:yyyy-MM-dd}.";

                await _notificationService.SendAsync(otRequest.Employee.UserId, title, message);
            }
        }
        catch (Exception)
        {
            // Ignore notification errors
        }

        // إعادة معالجة الحضور عند الموافقة لحساب الوقت الإضافي
        // Reprocess attendance on approval to calculate overtime
        if (request.Action == "APPROVE")
        {
            await _mediator.Send(new ProcessAttendanceCommand(otRequest.WorkDate), cancellationToken);
        }

        return Result<bool>.Success(true, request.Action == "APPROVE" ? "تمت الموافقة على الطلب" : "تم رفض الطلب");
    }
}
