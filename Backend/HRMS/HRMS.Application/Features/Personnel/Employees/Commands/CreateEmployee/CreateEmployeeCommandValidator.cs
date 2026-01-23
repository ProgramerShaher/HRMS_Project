using FluentValidation;
using HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee;

namespace HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(p => p.EmployeeNumber)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .MaximumLength(20).WithMessage("{PropertyName} must not exceed 20 characters.");

            RuleFor(p => p.FirstNameAr)
                .NotEmpty().WithMessage("First Name (Arabic) is required.")
                .MaximumLength(50);

            RuleFor(p => p.HijriLastNameAr)
                .NotEmpty().WithMessage("Last Name (Arabic) is required.")
                .MaximumLength(50);

            RuleFor(p => p.FullNameEn)
                .NotEmpty().WithMessage("English Full Name is required.")
                .MaximumLength(200);

            RuleFor(p => p.NationalityId)
                .GreaterThan(0).WithMessage("Nationality is required.");

            RuleFor(p => p.JobId)
                .GreaterThan(0).WithMessage("Job is required.");

            RuleFor(p => p.DeptId)
                .GreaterThan(0).WithMessage("Department is required.");
                
            RuleFor(p => p.Email)
                .EmailAddress().When(p => !string.IsNullOrEmpty(p.Email))
                .WithMessage("Invalid email format.");
        }
    }
}
