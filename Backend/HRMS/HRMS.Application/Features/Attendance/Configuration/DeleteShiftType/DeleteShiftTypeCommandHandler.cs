using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.DeleteShiftType;

/// <summary>
/// Handler for deleting shift type.
/// Performs soft delete to preserve historical data.
/// </summary>
public class DeleteShiftTypeCommandHandler : IRequestHandler<DeleteShiftTypeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteShiftTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(
        DeleteShiftTypeCommand request, 
        CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // جلب المناوبة المراد حذفها
        // Fetch shift to delete
        // ═══════════════════════════════════════════════════════════
        
        var shift = await _context.ShiftTypes
            .FindAsync(new object[] { request.ShiftId }, cancellationToken);

        if (shift == null)
            return Result<bool>.Failure("المناوبة غير موجودة");

        // ═══════════════════════════════════════════════════════════
        // حذف ناعم (Soft Delete)
        // Soft delete - preserves historical roster assignments
        // الحذف الناعم يحافظ على البيانات التاريخية للروستر
        // ═══════════════════════════════════════════════════════════
        
        shift.IsDeleted = 1;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم حذف المناوبة بنجاح");
    }
}
