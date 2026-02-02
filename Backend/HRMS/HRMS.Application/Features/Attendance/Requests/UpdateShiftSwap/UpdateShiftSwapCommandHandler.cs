using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Requests.UpdateShiftSwap;

/// <summary>
/// Handler for updating shift swap request.
/// Only allows modification of pending requests.
/// </summary>
public class UpdateShiftSwapCommandHandler : IRequestHandler<UpdateShiftSwapCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateShiftSwapCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateShiftSwapCommand request, CancellationToken cancellationToken)
    {
        var swapRequest = await _context.ShiftSwapRequests
            .FindAsync(new object[] { request.RequestId }, cancellationToken);

        if (swapRequest == null)
            return Result<bool>.Failure("الطلب غير موجود");

        if (swapRequest.Status != "PENDING")
            return Result<bool>.Failure("لا يمكن تعديل طلب غير معلق");

        // التحقق من وجود مناوبة للموظف المستهدف الجديد
        // Verify new target employee has a roster
        var targetHasRoster = await _context.EmployeeRosters
            .AnyAsync(r => r.EmployeeId == request.TargetEmployeeId && 
                           r.RosterDate == swapRequest.RosterDate && 
                           r.IsOffDay == 0, 
                      cancellationToken);

        if (!targetHasRoster)
            return Result<bool>.Failure("الموظف المستهدف ليس لديه مناوبة في هذا التاريخ");

        swapRequest.TargetEmployeeId = request.TargetEmployeeId;
        swapRequest.ManagerComment = request.Reason;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, "تم تعديل الطلب بنجاح");
    }
}
