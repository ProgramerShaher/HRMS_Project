using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.PublicHolidays.Queries.GetPublicHolidays;

// ═══════════════════════════════════════════════════════════════════════════
// 1. QUERY - الاستعلام
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Query to retrieve all public holidays with optional year filter.
/// Returns a list of holidays ordered by start date.
/// </summary>
public record GetPublicHolidaysQuery : IRequest<Result<List<PublicHolidayDto>>>
{
    /// <summary>
    /// السنة المراد استرجاع عطلها (اختياري)
    /// Year to filter holidays (optional - returns all if null)
    /// </summary>
    public short? Year { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. HANDLER - معالج الاستعلام
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for retrieving public holidays.
/// Uses AsNoTracking for optimized read-only queries.
/// </summary>
public class GetPublicHolidaysQueryHandler : IRequestHandler<GetPublicHolidaysQuery, Result<List<PublicHolidayDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPublicHolidaysQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PublicHolidayDto>>> Handle(GetPublicHolidaysQuery request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: بناء الاستعلام الأساسي
        // Step 1: Build base query
        // ═══════════════════════════════════════════════════════════════════════════

        // نبدأ باستعلام العطل الرسمية غير المحذوفة
        // Why: نستخدم AsNoTracking لتحسين الأداء لأننا لا نحتاج لتتبع التغييرات
        // Start with non-deleted holidays using AsNoTracking for performance
        var query = _context.PublicHolidays
            .AsNoTracking()
            .Where(h => h.IsDeleted == 0);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: تطبيق فلتر السنة إذا تم تحديده
        // Step 2: Apply year filter if provided
        // ═══════════════════════════════════════════════════════════════════════════

        // إذا حدد المستخدم سنة معينة، نفلتر العطل بناءً عليها
        // Why: لتسهيل عرض عطل سنة محددة فقط
        // If user specified a year, filter by it
        if (request.Year.HasValue)
        {
            query = query.Where(h => h.Year == request.Year.Value);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: استرجاع البيانات وتحويلها إلى DTOs
        // Step 3: Retrieve data and project to DTOs
        // ═══════════════════════════════════════════════════════════════════════════

        // نقوم بالإسقاط المباشر إلى DTO لتحسين الأداء
        // Why: نتجنب تحميل الكيانات الكاملة ثم تحويلها، بل نحمل الحقول المطلوبة فقط
        // Project directly to DTO for better performance
        var holidays = await query
            .OrderBy(h => h.StartDate) // ترتيب حسب تاريخ البداية
            .Select(h => new PublicHolidayDto
            {
                HolidayId = h.HolidayId,
                HolidayNameAr = h.HolidayNameAr,
                StartDate = h.StartDate,
                EndDate = h.EndDate,
                Year = h.Year
            })
            .ToListAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 4: إرجاع النتيجة
        // Step 4: Return result
        // ═══════════════════════════════════════════════════════════════════════════

        // نرجع قائمة العطل حتى لو كانت فارغة
        // Why: قائمة فارغة تعني عدم وجود عطل، وليس خطأ في النظام
        // Return the list even if empty (empty list is valid, not an error)
        return Result<List<PublicHolidayDto>>.Success(
            holidays,
            holidays.Count > 0 
                ? $"تم استرجاع {holidays.Count} عطلة رسمية" 
                : "لا توجد عطل رسمية مسجلة"
        );
    }
}
