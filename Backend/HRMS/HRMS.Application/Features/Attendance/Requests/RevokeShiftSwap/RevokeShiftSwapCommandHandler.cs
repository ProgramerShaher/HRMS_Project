using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Requests.RevokeShiftSwap;

/// <summary>
/// Handler for revoking approved shift swap.
/// Swaps rosters back to original state.
/// </summary>
public class RevokeShiftSwapCommandHandler : IRequestHandler<RevokeShiftSwapCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public RevokeShiftSwapCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(RevokeShiftSwapCommand request, CancellationToken cancellationToken)
    {
        var swapRequest = await _context.ShiftSwapRequests
            .FindAsync(new object[] { request.RequestId }, cancellationToken);

        if (swapRequest == null)
            return Result<bool>.Failure("الطلب غير موجود");

        if (swapRequest.Status != "APPROVED")
            return Result<bool>.Failure("يمكن إلغاء الطلبات المعتمدة فقط");

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

        // إعادة تبديل المناوبات للحالة الأصلية
        // Swap shifts back to original state
        var tempShiftId = requesterRoster.ShiftId;
        requesterRoster.ShiftId = targetRoster.ShiftId;
        targetRoster.ShiftId = tempShiftId;

        swapRequest.Status = "REVOKED";
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم إلغاء التبديل بنجاح");
    }
}
