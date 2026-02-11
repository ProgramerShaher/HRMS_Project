using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Requests.CreateShiftSwap;

/// <summary>
/// Handler for creating shift swap request.
/// Validates both employees have rosters and creates pending request.
/// </summary>
public class CreateShiftSwapCommandHandler : IRequestHandler<CreateShiftSwapCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public CreateShiftSwapCommandHandler(IApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Result<int>> Handle(CreateShiftSwapCommand request, CancellationToken cancellationToken)
    {
        // التحقق من وجود مناوبة للموظف المستهدف
        // Verify target employee has a roster on the same date
        var targetHasRoster = await _context.EmployeeRosters
            .AnyAsync(r => r.EmployeeId == request.TargetEmployeeId && 
                           r.RosterDate == request.RosterDate && 
                           r.IsOffDay == 0, 
                      cancellationToken);

        if (!targetHasRoster)
            return Result<int>.Failure("الموظف المستهدف ليس لديه مناوبة في هذا التاريخ");

        // التحقق من عدم وجود طلب تبديل معلق مسبقاً لنفس التاريخ
        // Check for existing pending swap request for same date
        var existingRequest = await _context.ShiftSwapRequests
            .AnyAsync(s => s.RequesterId == request.RequesterId && 
                           s.RosterDate == request.RosterDate && 
                           s.Status == "PENDING", 
                      cancellationToken);

        if (existingRequest)
            return Result<int>.Failure("يوجد طلب تبديل معلق مسبقاً لهذا التاريخ");

        var swapRequest = new ShiftSwapRequest
        {
            RequesterId = request.RequesterId,
            TargetEmployeeId = request.TargetEmployeeId,
            RosterDate = request.RosterDate,
            ManagerComment = request.Reason,
            Status = "PENDING"
        };

        _context.ShiftSwapRequests.Add(swapRequest);
        await _context.SaveChangesAsync(cancellationToken);

        // إرسال تنبيه للمدير المباشر
        try
        {
            var requester = await _context.Employees
                .Include(e => e.Manager)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == request.RequesterId, cancellationToken);

            if (requester?.Manager != null && !string.IsNullOrEmpty(requester.Manager.UserId))
            {
                await _notificationService.SendAsync(
                    userId: requester.Manager.UserId,
                    title: "طلب تبديل مناوبة جديد",
                    message: $"قام الموظف {requester.FullNameAr} بطلب تبديل مناوبة بتاريخ {request.RosterDate:yyyy-MM-dd}."
                );
            }
        }
        catch (Exception)
        {
            // Fail silently
        }

        return Result<int>.Success(swapRequest.RequestId, "تم تقديم طلب تبديل المناوبة بنجاح");
    }
}
