using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.UpdateShiftType;

/// <summary>
/// Handler for updating shift type.
/// Recalculates shift hours when timing is modified.
/// </summary>
public class UpdateShiftTypeCommandHandler : IRequestHandler<UpdateShiftTypeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateShiftTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(
        UpdateShiftTypeCommand request, 
        CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // جلب المناوبة المراد تعديلها
        // Fetch shift to update
        // ═══════════════════════════════════════════════════════════
        
        var shift = await _context.ShiftTypes
            .FindAsync(new object[] { request.ShiftId }, cancellationToken);

        if (shift == null)
            return Result<bool>.Failure("المناوبة غير موجودة");

        // ═══════════════════════════════════════════════════════════
        // إعادة حساب عدد الساعات
        // Recalculate shift hours
        // ═══════════════════════════════════════════════════════════
        
        decimal hours = 0;
        
        if (TimeSpan.TryParse(request.StartTime, out var start) && 
            TimeSpan.TryParse(request.EndTime, out var end))
        {
            if (request.IsCrossDay == 1 && end <= start)
            {
                hours = (decimal)(end.Add(TimeSpan.FromDays(1)) - start).TotalHours;
            }
            else
            {
                hours = (decimal)(end - start).TotalHours;
            }
        }

        // ═══════════════════════════════════════════════════════════
        // تحديث البيانات
        // Update shift data
        // ═══════════════════════════════════════════════════════════
        
        shift.ShiftNameAr = request.ShiftNameAr;
        shift.StartTime = request.StartTime;
        shift.EndTime = request.EndTime;
        shift.IsCrossDay = request.IsCrossDay;
        shift.GracePeriodMins = request.GracePeriodMins;
        shift.HoursCount = Math.Abs(hours);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم تعديل المناوبة بنجاح");
    }
}
