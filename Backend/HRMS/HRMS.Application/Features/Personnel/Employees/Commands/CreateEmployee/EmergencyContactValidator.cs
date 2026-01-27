using FluentValidation;
using HRMS.Application.DTOs.Personnel;

namespace HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee;

public class EmergencyContactValidator : AbstractValidator<EmergencyContactDto>
{
    public EmergencyContactValidator()
    {
        RuleFor(x => x.ContactNameAr)
            .NotEmpty().WithMessage("اسم جهة الاتصال مطلوب.")
            .MaximumLength(100).WithMessage("يجب ألا يتجاوز الاسم 100 حرف.")
            .Matches(@"^[\u0600-\u06FF\s]+$").WithMessage("الاسم يجب أن يكون بالعربية.");

        RuleFor(x => x.Relationship)
            .MaximumLength(50).WithMessage("يجب ألا تتجاوز صلة القرابة 50 حرفاً.");

        RuleFor(x => x.PhonePrimary)
            .NotEmpty().WithMessage("رقم الهاتف الأساسي مطلوب.")
            .MaximumLength(20).WithMessage("يجب ألا يتجاوز رقم الهاتف 20 رقماً.")
            .Matches(@"^\d+$").WithMessage("رقم الهاتف يجب أن يحتوي على أرقام فقط.");

        RuleFor(x => x.PhoneSecondary)
            .MaximumLength(20).WithMessage("يجب ألا يتجاوز رقم الهاتف الثانوي 20 رقماً.")
            .Matches(@"^\d+$").When(x => !string.IsNullOrEmpty(x.PhoneSecondary))
            .WithMessage("رقم الهاتف الثانوي يجب أن يحتوي على أرقام فقط.");
    }
}
