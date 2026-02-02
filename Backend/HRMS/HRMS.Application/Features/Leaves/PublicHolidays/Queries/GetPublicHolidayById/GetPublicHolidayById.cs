using FluentValidation;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.PublicHolidays.Queries.GetPublicHolidayById;

// ═══════════════════════════════════════════════════════════════════════════
// 1. QUERY - الاستعلام
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Query to retrieve a single public holiday by its ID.
/// Returns 404 if holiday not found.
/// </summary>
public record GetPublicHolidayByIdQuery : IRequest<Result<PublicHolidayDto>>
{
    /// <summary>
    /// معرف العطلة المراد استرجاعها
    /// Holiday ID to retrieve
    /// </summary>
    public int HolidayId { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Validator for GetPublicHolidayByIdQuery.
/// Ensures valid holiday ID is provided.
/// </summary>
public class GetPublicHolidayByIdQueryValidator : AbstractValidator<GetPublicHolidayByIdQuery>
{
    public GetPublicHolidayByIdQueryValidator()
    {
        // التحقق من معرف العطلة
        // Validate holiday ID
        RuleFor(x => x.HolidayId)
            .GreaterThan(0).WithMessage("معرف العطلة غير صحيح");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الاستعلام
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for retrieving a single public holiday by ID.
/// Uses AsNoTracking for optimized read-only query.
/// </summary>
public class GetPublicHolidayByIdQueryHandler : IRequestHandler<GetPublicHolidayByIdQuery, Result<PublicHolidayDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPublicHolidayByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PublicHolidayDto>> Handle(GetPublicHolidayByIdQuery request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: البحث عن العطلة وتحويلها إلى DTO
        // Step 1: Find holiday and project to DTO
        // ═══════════════════════════════════════════════════════════════════════════

        // نبحث عن العطلة ونحولها مباشرة إلى DTO
        // Why: نستخدم AsNoTracking لتحسين الأداء والإسقاط المباشر لتقليل استهلاك الذاكرة
        // Find holiday and project directly to DTO for performance
        var holiday = await _context.PublicHolidays
            .AsNoTracking()
            .Where(h => h.HolidayId == request.HolidayId && h.IsDeleted == 0)
            .Select(h => new PublicHolidayDto
            {
                HolidayId = h.HolidayId,
                HolidayNameAr = h.HolidayNameAr,
                StartDate = h.StartDate,
                EndDate = h.EndDate,
                Year = h.Year
            })
            .FirstOrDefaultAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: التحقق من وجود العطلة
        // Step 2: Verify holiday exists
        // ═══════════════════════════════════════════════════════════════════════════

        // إذا لم يتم العثور على العطلة، نرجع خطأ 404
        // Why: نتبع معايير REST API حيث عدم وجود المورد يعني 404
        // If holiday not found, return 404 error
        if (holiday == null)
        {
            return Result<PublicHolidayDto>.Failure("العطلة الرسمية غير موجودة", 404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: إرجاع النتيجة بنجاح
        // Step 3: Return success result
        // ═══════════════════════════════════════════════════════════════════════════

        return Result<PublicHolidayDto>.Success(holiday, "تم استرجاع العطلة الرسمية بنجاح");
    }
}
