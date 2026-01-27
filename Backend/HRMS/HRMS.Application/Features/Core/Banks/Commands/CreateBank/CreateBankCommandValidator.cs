using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Banks.Commands.CreateBank;

/// <summary>
/// محقق صحة بيانات أمر إنشاء البنك
/// </summary>
public class CreateBankCommandValidator : AbstractValidator<CreateBankCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateBankCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.BankNameAr)
            .NotEmpty().WithMessage("اسم البنك بالعربية مطلوب")
            .MaximumLength(100).WithMessage("اسم البنك لا يمكن أن يتجاوز 100 حرف")
            .MustAsync(BeUniqueBankNameAr).WithMessage("اسم البنك بالعربية موجود مسبقاً");

        RuleFor(x => x.SwiftCode)
            .Length(8, 11).When(x => !string.IsNullOrEmpty(x.SwiftCode))
            .WithMessage("رمز السويفت يجب أن يكون 8 أو 11 حرف")
            .Matches("^[A-Z]{6}[A-Z0-9]{2}([A-Z0-9]{3})?$").When(x => !string.IsNullOrEmpty(x.SwiftCode))
            .WithMessage("رمز السويفت غير صحيح")
            .MustAsync(BeUniqueSwiftCode).When(x => !string.IsNullOrEmpty(x.SwiftCode))
            .WithMessage("رمز السويفت موجود مسبقاً");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("البريد الإلكتروني غير صحيح");
    }

    private async Task<bool> BeUniqueBankNameAr(string nameAr, CancellationToken cancellationToken)
    {
        return !await _context.Banks.AnyAsync(b => b.BankNameAr == nameAr, cancellationToken);
    }

    private async Task<bool> BeUniqueSwiftCode(string? swiftCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(swiftCode)) return true;
        return !await _context.Banks.AnyAsync(b => b.SwiftCode == swiftCode, cancellationToken);
    }
}
