using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.PublicHolidays.Commands.DeletePublicHoliday;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Command to soft delete a public holiday.
/// Sets IsDeleted flag instead of physical deletion.
/// </summary>
/// <summary>
/// Command to soft delete a public holiday.
/// Sets IsDeleted flag instead of physical deletion.
/// </summary>
public record DeletePublicHolidayCommand(int HolidayId) : IRequest<Result<bool>>;

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Validator for DeletePublicHolidayCommand.
/// Ensures valid holiday ID is provided.
/// </summary>
public class DeletePublicHolidayCommandValidator : AbstractValidator<DeletePublicHolidayCommand>
{
    public DeletePublicHolidayCommandValidator()
    {
        // التحقق من معرف العطلة
        // Validate holiday ID
        RuleFor(x => x.HolidayId)
            .GreaterThan(0).WithMessage("معرف العطلة غير صحيح");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for soft deleting a public holiday.
/// Implements soft delete pattern to maintain data integrity.
/// </summary>
public class DeletePublicHolidayCommandHandler : IRequestHandler<DeletePublicHolidayCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeletePublicHolidayCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeletePublicHolidayCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: التحقق من وجود العطلة
        // Step 1: Verify holiday exists
        // ═══════════════════════════════════════════════════════════════════════════

        // نبحث عن العطلة المراد حذفها
        // Why: لا يمكن حذف عطلة غير موجودة أو محذوفة مسبقاً
        // Find the holiday to delete
        var holiday = await _context.PublicHolidays
            .FirstOrDefaultAsync(h => h.HolidayId == request.HolidayId && h.IsDeleted == 0, cancellationToken);

        if (holiday == null)
        {
            return Result<bool>.Failure("العطلة الرسمية غير موجودة أو تم حذفها مسبقاً", 404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: تنفيذ الحذف المنطقي (Soft Delete)
        // Step 2: Perform soft delete
        // ═══════════════════════════════════════════════════════════════════════════

        // نستخدم الحذف المنطقي بدلاً من الحذف الفعلي
        // Why: للحفاظ على سلامة البيانات التاريخية وإمكانية استرجاع العطلة لاحقاً
        // Use soft delete instead of physical deletion
        holiday.IsDeleted = 1;

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: حفظ التغييرات
        // Step 3: Save changes
        // ═══════════════════════════════════════════════════════════════════════════

        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 4: إرجاع النتيجة بنجاح
        // Step 4: Return success result
        // ═══════════════════════════════════════════════════════════════════════════

        return Result<bool>.Success(true, "تم حذف العطلة الرسمية بنجاح");
    }
}
