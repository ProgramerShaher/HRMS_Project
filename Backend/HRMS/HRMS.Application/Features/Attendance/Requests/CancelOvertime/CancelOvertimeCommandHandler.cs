using HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Requests.CancelOvertime;

/// <summary>
/// Handler for canceling overtime request.
/// Reprocesses attendance if request was approved.
/// </summary>
public class CancelOvertimeCommandHandler : IRequestHandler<CancelOvertimeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public CancelOvertimeCommandHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(CancelOvertimeCommand request, CancellationToken cancellationToken)
    {
        var otRequest = await _context.OvertimeRequests.FindAsync(new object[] { request.RequestId }, cancellationToken);
        if (otRequest == null) return Result<bool>.Failure("الطلب غير موجود");

        // التحقق من قفل الرواتب
        // Check payroll lock
        if (await _context.PayrollRuns.AnyAsync(
            p => p.Year == otRequest.WorkDate.Year && 
                 p.Month == otRequest.WorkDate.Month && 
                 (p.Status == "APPROVED" || p.Status == "PAID"), 
            cancellationToken))
            return Result<bool>.Failure("الشهر المالي مغلق");

        bool wasApproved = otRequest.Status == "APPROVED";
        DateTime workDate = otRequest.WorkDate;

        otRequest.Status = "CANCELLED";
        await _context.SaveChangesAsync(cancellationToken);

        // إعادة معالجة الحضور إذا كان الطلب معتمداً
        // Reprocess attendance if request was approved
        if (wasApproved)
        {
            await _mediator.Send(new ProcessAttendanceCommand(workDate), cancellationToken);
        }

        return Result<bool>.Success(true, "تم إلغاء الطلب بنجاح");
    }
}
