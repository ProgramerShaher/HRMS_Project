using HRMS.Application.DTOs.Attendance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Configuration.GetShiftTypes;

/// <summary>
/// Handler for retrieving all shift types.
/// Returns only active (non-deleted) shifts.
/// </summary>
public class GetShiftTypesQueryHandler : IRequestHandler<GetShiftTypesQuery, Result<List<ShiftTypeDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetShiftTypesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ShiftTypeDto>>> Handle(
        GetShiftTypesQuery request, 
        CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // جلب جميع المناوبات النشطة
        // Fetch all active shifts
        // نستخدم AsNoTracking لتحسين الأداء لأننا نقرأ فقط
        // Use AsNoTracking for read-only performance optimization
        // ═══════════════════════════════════════════════════════════
        
        var shifts = await _context.ShiftTypes
            .AsNoTracking()
            .Where(s => s.IsDeleted == 0)
            .OrderBy(s => s.ShiftId)
            .Select(s => new ShiftTypeDto
            {
                ShiftId = s.ShiftId,
                ShiftNameAr = s.ShiftNameAr,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                HoursCount = s.HoursCount,
                IsCrossDay = s.IsCrossDay,
                GracePeriodMins = s.GracePeriodMins
            })
            .ToListAsync(cancellationToken);

        return Result<List<ShiftTypeDto>>.Success(shifts);
    }
}
