using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Banks.Commands.UpdateBank;

/// <summary>
/// محقق صحة بيانات أمر تحديث البنك
/// </summary>
/// <remarks>
/// يتحقق من صحة البيانات المدخلة قبل تحديث البنك في قاعدة البيانات
/// </remarks>
public class UpdateBankCommandValidator : AbstractValidator<UpdateBankCommand>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// المنشئ - يقوم بتهيئة قواعد التحقق
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات</param>
    public UpdateBankCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.BankId)
            .GreaterThan(0).WithMessage("معرف البنك غير صحيح")
            .MustAsync(BankExists).WithMessage("البنك غير موجود");

        RuleFor(x => x.BankNameAr)
            .NotEmpty().WithMessage("اسم البنك بالعربية مطلوب")
            .MaximumLength(100).WithMessage("اسم البنك لا يمكن أن يتجاوز 100 حرف")
            .MustAsync(BeUniqueBankNameAr).WithMessage("اسم البنك بالعربية موجود مسبقاً");

        RuleFor(x => x.BankNameEn)
            .MaximumLength(100).WithMessage("اسم البنك بالإنجليزية لا يمكن أن يتجاوز 100 حرف")
            .MustAsync(BeUniqueBankNameEn).When(x => !string.IsNullOrEmpty(x.BankNameEn))
            .WithMessage("اسم البنك بالإنجليزية موجود مسبقاً");

        RuleFor(x => x.SwiftCode)
            .Length(8, 11).When(x => !string.IsNullOrEmpty(x.SwiftCode))
            .WithMessage("رمز السويفت يجب أن يكون 8 أو 11 حرف")
            .Matches("^[A-Z]{6}[A-Z0-9]{2}([A-Z0-9]{3})?$").When(x => !string.IsNullOrEmpty(x.SwiftCode))
            .WithMessage("رمز السويفت غير صحيح (مثال: NCBKSAJE)")
            .MustAsync(BeUniqueSwiftCode).When(x => !string.IsNullOrEmpty(x.SwiftCode))
            .WithMessage("رمز السويفت موجود مسبقاً");

        RuleFor(x => x.BankCode)
            .MaximumLength(20).WithMessage("رمز البنك لا يمكن أن يتجاوز 20 حرف");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("البريد الإلكتروني غير صحيح");
    }

    /// <summary>
    /// التحقق من وجود البنك
    /// </summary>
    private async Task<bool> BankExists(int bankId, CancellationToken cancellationToken)
    {
        return await _context.Banks.AnyAsync(b => b.BankId == bankId, cancellationToken);
    }

    /// <summary>
    /// التحقق من عدم تكرار اسم البنك بالعربية
    /// </summary>
    private async Task<bool> BeUniqueBankNameAr(UpdateBankCommand command, string nameAr, CancellationToken cancellationToken)
    {
        return !await _context.Banks
            .AnyAsync(b => b.BankNameAr == nameAr && b.BankId != command.BankId, cancellationToken);
    }

    /// <summary>
    /// التحقق من عدم تكرار اسم البنك بالإنجليزية
    /// </summary>
    private async Task<bool> BeUniqueBankNameEn(UpdateBankCommand command, string? nameEn, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(nameEn)) return true;
        
        return !await _context.Banks
            .AnyAsync(b => b.BankNameEn == nameEn && b.BankId != command.BankId, cancellationToken);
    }

    /// <summary>
    /// التحقق من عدم تكرار رمز السويفت
    /// </summary>
    private async Task<bool> BeUniqueSwiftCode(UpdateBankCommand command, string? swiftCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(swiftCode)) return true;
        
        return !await _context.Banks
            .AnyAsync(b => b.SwiftCode == swiftCode && b.BankId != command.BankId, cancellationToken);
    }
}
