using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Requests.UpdateOvertime;

/// <summary>
/// Handler for updating overtime request.
/// Only allows modification of pending requests.
/// </summary>
public class UpdateOvertimeCommandHandler : IRequestHandler<UpdateOvertimeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateOvertimeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateOvertimeCommand request, CancellationToken cancellationToken)
    {
        var otRequest = await _context.OvertimeRequests.FindAsync(new object[] { request.RequestId }, cancellationToken);
        if (otRequest == null) return Result<bool>.Failure("الطلب غير موجود");

        // السماح بالتعديل فقط للطلبات المعلقة
        // Only allow modification of pending requests
        if (otRequest.Status != "PENDING") 
            return Result<bool>.Failure("لا يمكن تعديل طلب غير معلق");

        // التحقق من قفل الرواتب
        // Check payroll lock
        if (await _context.PayrollRuns.AnyAsync(
            p => p.Year == otRequest.WorkDate.Year && 
                 p.Month == otRequest.WorkDate.Month && 
                 (p.Status == "APPROVED" || p.Status == "PAID"), 
            cancellationToken))
            return Result<bool>.Failure("الشهر المالي مغلق");

        otRequest.HoursRequested = request.HoursRequested;
        otRequest.Reason = request.Reason;
        
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, "تم تعديل الطلب بنجاح");
    }
}
