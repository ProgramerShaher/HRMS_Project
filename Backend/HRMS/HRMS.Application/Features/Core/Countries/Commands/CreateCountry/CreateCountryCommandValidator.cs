using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Countries.Commands.CreateCountry
{
    public class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateCountryCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.CountryNameAr)
                .NotEmpty().WithMessage("اسم الدولة بالعربية مطلوب")
                .MaximumLength(100).WithMessage("اسم الدولة يجب ألا يتجاوز 100 حرف")
                .MustAsync(BeUniqueNameAr).WithMessage("اسم الدولة بالعربية موجود مسبقاً");

            RuleFor(x => x.CountryNameEn)
                .NotEmpty().WithMessage("اسم الدولة بالإنجليزية مطلوب")
                .MaximumLength(100).WithMessage("اسم الدولة يجب ألا يتجاوز 100 حرف")
                .MustAsync(BeUniqueNameEn).WithMessage("اسم الدولة بالإنجليزية موجود مسبقاً");

            RuleFor(x => x.IsoCode)
                .NotEmpty().WithMessage("رمز ISO مطلوب")
                .Length(2).WithMessage("رمز ISO يجب أن يكون حرفين فقط")
                .Matches("^[a-zA-Z]{2}$").WithMessage("رمز ISO يجب أن يحتوي على حرفين بالإنجليزية فقط")
                .MustAsync(BeUniqueIsoCode).WithMessage("رمز ISO موجود مسبقاً");
        }

        private async Task<bool> BeUniqueNameAr(string nameAr, CancellationToken cancellation)
        {
            return !await _context.Countries.AnyAsync(c => c.CountryNameAr == nameAr, cancellation);
        }

        private async Task<bool> BeUniqueNameEn(string nameEn, CancellationToken cancellation)
        {
            return !await _context.Countries.AnyAsync(c => c.CountryNameEn == nameEn, cancellation);
        }

        private async Task<bool> BeUniqueIsoCode(string isoCode, CancellationToken cancellation)
        {
            if (string.IsNullOrEmpty(isoCode)) return true;
            var upperIso = isoCode.ToUpper();
            return !await _context.Countries.AnyAsync(c => c.IsoCode == upperIso, cancellation);
        }
    }
}
