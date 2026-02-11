using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Requests.ActionShiftSwap;

/// <summary>
/// Handler for approving/rejecting shift swap requests.
/// Swaps employee rosters on approval.
/// </summary>
public class ActionShiftSwapCommandHandler : IRequestHandler<ActionShiftSwapCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public ActionShiftSwapCommandHandler(IApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(ActionShiftSwapCommand request, CancellationToken cancellationToken)
    {
        var swapRequest = await _context.ShiftSwapRequests
            .FindAsync(new object[] { request.RequestId }, cancellationToken);

        if (swapRequest == null)
            return Result<bool>.Failure("الطلب غير موجود");

        if (swapRequest.Status != "PENDING")
            return Result<bool>.Failure("لا يمكن اتخاذ إجراء على طلب غير معلق");

        if (request.Action == "APPROVE")
        {
            // جلب الروستر للموظفين
            // Fetch rosters for both employees
            var requesterRoster = await _context.EmployeeRosters
                .FirstOrDefaultAsync(r => r.EmployeeId == swapRequest.RequesterId && 
                                          r.RosterDate == swapRequest.RosterDate, 
                                     cancellationToken);

            var targetRoster = await _context.EmployeeRosters
                .FirstOrDefaultAsync(r => r.EmployeeId == swapRequest.TargetEmployeeId && 
                                          r.RosterDate == swapRequest.RosterDate, 
                                     cancellationToken);

            if (requesterRoster == null || targetRoster == null)
                return Result<bool>.Failure("لم يتم العثور على الروستر لأحد الموظفين");

            // تبديل المناوبات بين الموظفين
            // Swap shifts between employees
            var tempShiftId = requesterRoster.ShiftId;
            requesterRoster.ShiftId = targetRoster.ShiftId;
            targetRoster.ShiftId = tempShiftId;

            swapRequest.Status = "APPROVED";
            swapRequest.CreatedBy = request.ManagerId.ToString();
            swapRequest.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            swapRequest.Status = "REJECTED";
        }

        swapRequest.ManagerComment = request.Comment;
        await _context.SaveChangesAsync(cancellationToken);

        // إرسال التنبيهات
        try
        {
            var requester = await _context.Employees.FindAsync(new object[] { swapRequest.RequesterId }, cancellationToken);
            var target = await _context.Employees.FindAsync(new object[] { swapRequest.TargetEmployeeId }, cancellationToken);

            // 1. تنبيه مقدم الطلب
            if (requester?.UserId != null)
            {
                var title = request.Action == "APPROVE" ? "موافقة على تبديل المناوبة" : "رفض تبديل المناوبة";
                var message = request.Action == "APPROVE"
                    ? $"تمت الموافقة على طلب تبديل المناوبة ليوم {swapRequest.RosterDate:yyyy-MM-dd}."
                    : $"تم رفض طلب تبديل المناوبة ليوم {swapRequest.RosterDate:yyyy-MM-dd}.";
                
                await _notificationService.SendAsync(requester.UserId, title, message);
            }

            // 2. تنبيه الطرف الآخر (في حال الموافقة فقط)
            if (request.Action == "APPROVE" && target?.UserId != null)
            {
                await _notificationService.SendAsync(
                    userId: target.UserId, 
                    title: "تغيير في المناوبة", 
                    message: $"تم اعتماد تبديل مناوبتك ليوم {swapRequest.RosterDate:yyyy-MM-dd} مع الموظف {requester?.FullNameAr}."
                );
            }
        }
        catch (Exception)
        {
            // Ignore logic
        }

        return Result<bool>.Success(true, request.Action == "APPROVE" ? "تمت الموافقة على التبديل" : "تم رفض التبديل");
    }
}
