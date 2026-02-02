using FluentValidation;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.PublicHolidays.Commands.CreatePublicHoliday;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Command to create a new public holiday in the system.
/// Validates date ranges and prevents overlapping holidays.
/// </summary>
public record CreatePublicHolidayCommand : IRequest<Result<int>>
{
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
    /// السنة (اختياري - يتم استخراجها من تاريخ البداية)
    /// Year (optional - extracted from start date if not provided)
    /// </summary>
    public short? Year { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Validator for CreatePublicHolidayCommand.
/// Ensures all required fields are valid before processing.
/// </summary>
public class CreatePublicHolidayCommandValidator : AbstractValidator<CreatePublicHolidayCommand>
{
    public CreatePublicHolidayCommandValidator()
    {
        // التحقق من اسم العطلة
        // Validate holiday name
        RuleFor(x => x.HolidayNameAr)
            .NotEmpty().WithMessage("اسم العطلة مطلوب")
            .MaximumLength(100).WithMessage("اسم العطلة لا يمكن أن يتجاوز 100 حرف");

        // التحقق من تاريخ البداية
        // Validate start date
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاريخ البداية مطلوب")
            .GreaterThanOrEqualTo(new DateTime(2020, 1, 1)).WithMessage("تاريخ البداية يجب أن يكون بعد 2020");

        // التحقق من تاريخ النهاية
        // Validate end date
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("تاريخ النهاية مطلوب")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("تاريخ النهاية يجب أن يكون بعد أو يساوي تاريخ البداية");

        // التحقق من السنة إذا تم تحديدها
        // Validate year if provided
        RuleFor(x => x.Year)
            .GreaterThan((short)2000).WithMessage("السنة غير صحيحة")
            .When(x => x.Year.HasValue);
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for creating a new public holiday.
/// Implements overlap detection and automatic year calculation.
/// </summary>
public class CreatePublicHolidayCommandHandler : IRequestHandler<CreatePublicHolidayCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreatePublicHolidayCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreatePublicHolidayCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: التحقق من عدم وجود تداخل مع عطل رسمية أخرى
        // Step 1: Check for overlapping holidays
        // ═══════════════════════════════════════════════════════════════════════════

        // نتحقق من وجود أي عطلة رسمية تتداخل مع الفترة المطلوبة
        // Why: لمنع تسجيل عطلتين في نفس الفترة مما يسبب تضارب في الحسابات
        // Check if any existing holiday overlaps with the requested date range
        var hasOverlap = await _context.PublicHolidays
            .Where(h => h.IsDeleted == 0)
            .AnyAsync(h => 
                // التداخل يحدث إذا كانت البداية أو النهاية تقع ضمن عطلة موجودة
                // Overlap occurs if start or end falls within an existing holiday
                (request.StartDate >= h.StartDate && request.StartDate <= h.EndDate) ||
                (request.EndDate >= h.StartDate && request.EndDate <= h.EndDate) ||
                // أو إذا كانت العطلة الجديدة تحتوي العطلة الموجودة بالكامل
                // Or if the new holiday completely contains an existing one
                (request.StartDate <= h.StartDate && request.EndDate >= h.EndDate),
                cancellationToken);

        if (hasOverlap)
        {
            return Result<int>.Failure("يوجد تداخل مع عطلة رسمية أخرى في نفس الفترة", 400);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: استخراج السنة تلقائياً إذا لم يتم تحديدها
        // Step 2: Auto-calculate year if not provided
        // ═══════════════════════════════════════════════════════════════════════════

        // إذا لم يحدد المستخدم السنة، نستخرجها من تاريخ البداية
        // Why: لتسهيل الاستخدام وتجنب الأخطاء اليدوية
        // If user didn't specify year, extract it from start date
        short year = request.Year ?? (short)request.StartDate.Year;

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: إنشاء كيان العطلة الرسمية
        // Step 3: Create the public holiday entity
        // ═══════════════════════════════════════════════════════════════════════════

        var holiday = new PublicHoliday
        {
            HolidayNameAr = request.HolidayNameAr.Trim(),
            StartDate = request.StartDate.Date, // نأخذ التاريخ فقط بدون الوقت
            EndDate = request.EndDate.Date,
            Year = year
        };

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 4: حفظ العطلة في قاعدة البيانات
        // Step 4: Save to database
        // ═══════════════════════════════════════════════════════════════════════════

        _context.PublicHolidays.Add(holiday);
        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 5: إرجاع النتيجة بنجاح
        // Step 5: Return success result
        // ═══════════════════════════════════════════════════════════════════════════

        return Result<int>.Success(holiday.HolidayId, "تم إضافة العطلة الرسمية بنجاح");
    }
}
