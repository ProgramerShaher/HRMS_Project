using FluentValidation;

namespace HRMS.Application.Features.Core.Cities.Commands.CreateCity
{
    public class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
    {
        public CreateCityCommandValidator()
        {
            RuleFor(x => x.CityNameAr).NotEmpty().MaximumLength(100);
            RuleFor(x => x.CountryId).GreaterThan(0).WithMessage("الدولة مطلوبة");
        }
    }
}
