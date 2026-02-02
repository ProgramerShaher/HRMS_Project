using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.PublicHolidays.Commands.UpdatePublicHoliday;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Command to update an existing public holiday.
/// Validates the holiday exists and checks for date overlaps.
/// </summary>
public record UpdatePublicHolidayCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف العطلة المراد تحديثها
    /// Holiday ID to update
    /// </summary>
    public int HolidayId { get; init; }

    /// <summary>
    /// اسم العطلة بالعربية
    /// Holiday name in Arabic
    /// </summary>
    public string HolidayNameAr { get; init; } = string.Empty;

    /// <summary>
    /// تاريخ بداية العطلة
    /// Holiday start date
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// تاريخ نهاية العطلة
    /// Holiday end date
    /// </summary>
    public DateTime EndDate { get; init; }

    /// <summary>
    /// السنة
    /// Year
    /// </summary>
    public short Year { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Validator for UpdatePublicHolidayCommand.
/// Ensures all required fields are valid before processing.
/// </summary>
public class UpdatePublicHolidayCommandValidator : AbstractValidator<UpdatePublicHolidayCommand>
{
    public UpdatePublicHolidayCommandValidator()
    {
        // التحقق من معرف العطلة
        // Validate holiday ID
        RuleFor(x => x.HolidayId)
            .GreaterThan(0).WithMessage("معرف العطلة غير صحيح");

        // التحقق من اسم العطلة
        // Validate holiday name
        RuleFor(x => x.HolidayNameAr)
            .NotEmpty().WithMessage("اسم العطلة مطلوب")
            .MaximumLength(100).WithMessage("اسم العطلة لا يمكن أن يتجاوز 100 حرف");

        // التحقق من تاريخ البداية
        // Validate start date
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاريخ البداية مطلوب");

        // التحقق من تاريخ النهاية
        // Validate end date
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("تاريخ النهاية مطلوب")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("تاريخ النهاية يجب أن يكون بعد أو يساوي تاريخ البداية");

        // التحقق من السنة
        // Validate year
        RuleFor(x => x.Year)
            .GreaterThan((short)2000).WithMessage("السنة غير صحيحة");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for updating an existing public holiday.
/// Validates existence and checks for overlaps with other holidays.
/// </summary>
public class UpdatePublicHolidayCommandHandler : IRequestHandler<UpdatePublicHolidayCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdatePublicHolidayCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdatePublicHolidayCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: التحقق من وجود العطلة
        // Step 1: Verify holiday exists
        // ═══════════════════════════════════════════════════════════════════════════

        // نبحث عن العطلة المراد تحديثها
        // Why: لا يمكن تحديث عطلة غير موجودة
        // Find the holiday to update
        var holiday = await _context.PublicHolidays
            .FirstOrDefaultAsync(h => h.HolidayId == request.HolidayId && h.IsDeleted == 0, cancellationToken);

        if (holiday == null)
        {
            return Result<bool>.Failure("العطلة الرسمية غير موجودة", 404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: التحقق من عدم التداخل مع عطل أخرى (باستثناء العطلة الحالية)
        // Step 2: Check for overlaps (excluding current holiday)
        // ═══════════════════════════════════════════════════════════════════════════

        // نتحقق من التداخل مع عطل أخرى، لكن نستثني العطلة الحالية من الفحص
        // Why: العطلة الحالية قد تتداخل مع نفسها، لذا يجب استثناؤها
        // Check for overlaps with other holidays (excluding the current one)
        var hasOverlap = await _context.PublicHolidays
            .Where(h => h.IsDeleted == 0 && h.HolidayId != request.HolidayId)
            .AnyAsync(h =>
                (request.StartDate >= h.StartDate && request.StartDate <= h.EndDate) ||
                (request.EndDate >= h.StartDate && request.EndDate <= h.EndDate) ||
                (request.StartDate <= h.StartDate && request.EndDate >= h.EndDate),
                cancellationToken);

        if (hasOverlap)
        {
            return Result<bool>.Failure("يوجد تداخل مع عطلة رسمية أخرى في نفس الفترة", 400);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: تحديث بيانات العطلة
        // Step 3: Update holiday data
        // ═══════════════════════════════════════════════════════════════════════════

        // نقوم بتحديث الحقول المسموح تعديلها فقط
        // Why: نتجنب تحديث حقول النظام مثل CreatedAt أو CreatedBy
        // Update only allowed fields
        holiday.HolidayNameAr = request.HolidayNameAr.Trim();
        holiday.StartDate = request.StartDate.Date;
        holiday.EndDate = request.EndDate.Date;
        holiday.Year = request.Year;

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 4: حفظ التغييرات
        // Step 4: Save changes
        // ═══════════════════════════════════════════════════════════════════════════

        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 5: إرجاع النتيجة بنجاح
        // Step 5: Return success result
        // ═══════════════════════════════════════════════════════════════════════════

        return Result<bool>.Success(true, "تم تحديث العطلة الرسمية بنجاح");
    }
}
