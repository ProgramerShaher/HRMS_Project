using FluentValidation;

namespace HRMS.Application.Features.Core.Countries.Commands.CreateCountry
{
    public class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
    {
        public CreateCountryCommandValidator()
        {
            RuleFor(x => x.CountryNameAr)
                .NotEmpty().WithMessage("اسم الدولة بالعربية مطلوب")
                .MaximumLength(100).WithMessage("اسم الدولة يجب ألا يتجاوز 100 حرف");

            RuleFor(x => x.CountryNameEn)
                .NotEmpty().WithMessage("اسم الدولة بالإنجليزية مطلوب")
                .MaximumLength(100).WithMessage("اسم الدولة يجب ألا يتجاوز 100 حرف");

            RuleFor(x => x.IsoCode)
                .Length(2).When(x => !string.IsNullOrEmpty(x.IsoCode))
                .WithMessage("رمز ISO يجب أن يكون حرفين فقط");
        }
    }
}
