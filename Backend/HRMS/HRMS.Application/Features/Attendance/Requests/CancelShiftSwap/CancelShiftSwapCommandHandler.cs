using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Requests.CancelShiftSwap;

/// <summary>
/// Handler for canceling shift swap request.
/// Only allows cancellation of pending requests.
/// </summary>
public class CancelShiftSwapCommandHandler : IRequestHandler<CancelShiftSwapCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CancelShiftSwapCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CancelShiftSwapCommand request, CancellationToken cancellationToken)
    {
        var swapRequest = await _context.ShiftSwapRequests
            .FindAsync(new object[] { request.RequestId }, cancellationToken);

        if (swapRequest == null)
            return Result<bool>.Failure("الطلب غير موجود");

        if (swapRequest.Status != "PENDING")
            return Result<bool>.Failure("يمكن إلغاء الطلبات المعلقة فقط");

        swapRequest.Status = "CANCELLED";
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم إلغاء الطلب بنجاح");
    }
}
