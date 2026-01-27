using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Countries.Commands.UpdateCountry;

/// <summary>
/// التحقق من صحة بيانات تحديث الدولة
/// </summary>
public class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateCountryCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.CountryId)
            .GreaterThan(0).WithMessage("معرف الدولة غير صحيح")
            .MustAsync(CountryExists).WithMessage("الدولة غير موجودة");

        RuleFor(x => x.CountryNameAr)
            .NotEmpty().WithMessage("اسم الدولة بالعربية مطلوب")
            .MaximumLength(100).WithMessage("اسم الدولة لا يمكن أن يتجاوز 100 حرف")
            .MustAsync(BeUniqueCountryNameAr).WithMessage("اسم الدولة بالعربية موجود مسبقاً");

        RuleFor(x => x.CountryNameEn)
            .NotEmpty().WithMessage("اسم الدولة بالإنجليزية مطلوب")
            .MaximumLength(100).WithMessage("اسم الدولة لا يمكن أن يتجاوز 100 حرف")
            .MustAsync(BeUniqueCountryNameEn).WithMessage("اسم الدولة بالإنجليزية موجود مسبقاً");

        RuleFor(x => x.IsoCode)
            .MaximumLength(2).WithMessage("رمز ISO يجب أن يكون حرفين فقط")
            .Matches("^[A-Z]{2}$").When(x => !string.IsNullOrEmpty(x.IsoCode))
            .WithMessage("رمز ISO يجب أن يكون حرفين كبيرين بالإنجليزية")
            .MustAsync(BeUniqueIsoCode).When(x => !string.IsNullOrEmpty(x.IsoCode))
            .WithMessage("رمز ISO موجود مسبقاً");

        RuleFor(x => x.CitizenshipNameAr)
            .MaximumLength(100).WithMessage("اسم الجنسية لا يمكن أن يتجاوز 100 حرف");
    }

    private async Task<bool> CountryExists(int countryId, CancellationToken cancellationToken)
    {
        return await _context.Countries.AnyAsync(c => c.CountryId == countryId, cancellationToken);
    }

    private async Task<bool> BeUniqueCountryNameAr(UpdateCountryCommand command, string nameAr, CancellationToken cancellationToken)
    {
        return !await _context.Countries
            .AnyAsync(c => c.CountryNameAr == nameAr && c.CountryId != command.CountryId, cancellationToken);
    }

    private async Task<bool> BeUniqueCountryNameEn(UpdateCountryCommand command, string nameEn, CancellationToken cancellationToken)
    {
        return !await _context.Countries
            .AnyAsync(c => c.CountryNameEn == nameEn && c.CountryId != command.CountryId, cancellationToken);
    }

    private async Task<bool> BeUniqueIsoCode(UpdateCountryCommand command, string? isoCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(isoCode)) return true;
        
        return !await _context.Countries
            .AnyAsync(c => c.IsoCode == isoCode && c.CountryId != command.CountryId, cancellationToken);
    }
}
