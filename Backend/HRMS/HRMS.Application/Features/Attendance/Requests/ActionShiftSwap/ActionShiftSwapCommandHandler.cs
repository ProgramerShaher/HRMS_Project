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

    public ActionShiftSwapCommandHandler(IApplicationDbContext context)
    {
        _context = context;
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

        return Result<bool>.Success(true, request.Action == "APPROVE" ? "تمت الموافقة على التبديل" : "تم رفض التبديل");
    }
}
