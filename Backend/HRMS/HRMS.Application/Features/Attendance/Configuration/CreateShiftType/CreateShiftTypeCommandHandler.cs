using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.CreateShiftType;

/// <summary>
/// Handler for creating shift type.
/// Calculates shift hours automatically based on start/end times.
/// </summary>
public class CreateShiftTypeCommandHandler : IRequestHandler<CreateShiftTypeCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateShiftTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(
        CreateShiftTypeCommand request, 
        CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // حساب عدد ساعات المناوبة
        // Calculate shift hours
        // ═══════════════════════════════════════════════════════════
        
        decimal hours = 0;
        
        if (TimeSpan.TryParse(request.StartTime, out var start) && 
            TimeSpan.TryParse(request.EndTime, out var end))
        {
            // التحقق من المناوبة العابرة لمنتصف الليل
            // Check for cross-day shift (e.g., 22:00 to 06:00)
            if (request.IsCrossDay == 1 && end <= start)
            {
                // إضافة يوم كامل لوقت النهاية لحساب الساعات بشكل صحيح
                // Add full day to end time for correct calculation
                hours = (decimal)(end.Add(TimeSpan.FromDays(1)) - start).TotalHours;
            }
            else
            {
                // حساب عادي للمناوبة في نفس اليوم
                // Normal calculation for same-day shift
                hours = (decimal)(end - start).TotalHours;
            }
        }

        // ═══════════════════════════════════════════════════════════
        // إنشاء المناوبة الجديدة
        // Create new shift
        // ═══════════════════════════════════════════════════════════
        
        var shift = new ShiftType
        {
            ShiftNameAr = request.ShiftNameAr,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsCrossDay = request.IsCrossDay,
            GracePeriodMins = request.GracePeriodMins,
            HoursCount = Math.Abs(hours) // استخدام القيمة المطلقة لتجنب القيم السالبة
        };

        _context.ShiftTypes.Add(shift);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(shift.ShiftId, "تم إضافة المناوبة بنجاح");
    }
}
