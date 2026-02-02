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

    public ActionOvertimeCommandHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(ActionOvertimeCommand request, CancellationToken cancellationToken)
    {
        var otRequest = await _context.OvertimeRequests.FindAsync(new object[] { request.RequestId }, cancellationToken);
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

        // إعادة معالجة الحضور عند الموافقة لحساب الوقت الإضافي
        // Reprocess attendance on approval to calculate overtime
        if (request.Action == "APPROVE")
        {
            await _mediator.Send(new ProcessAttendanceCommand(otRequest.WorkDate), cancellationToken);
        }

        return Result<bool>.Success(true, request.Action == "APPROVE" ? "تمت الموافقة على الطلب" : "تم رفض الطلب");
    }
}
