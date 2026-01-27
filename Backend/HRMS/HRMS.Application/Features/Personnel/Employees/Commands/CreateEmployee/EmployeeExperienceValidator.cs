using FluentValidation;
using HRMS.Application.DTOs.Personnel;

namespace HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee;

public class EmployeeExperienceValidator : AbstractValidator<EmployeeExperienceDto>
{
    public EmployeeExperienceValidator()
    {
        RuleFor(x => x.CompanyNameAr)
            .NotEmpty().WithMessage("اسم الشركة مطلوب.")
            .MaximumLength(200).WithMessage("يجب ألا يتجاوز اسم الشركة 200 حرف.");

        RuleFor(x => x.JobTitleAr)
            .MaximumLength(100).WithMessage("يجب ألا يتجاوز المسمى الوظيفي 100 حرف.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاريخ البداية مطلوب.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("تاريخ البداية لا يمكن أن يكون في المستقبل.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).When(x => x.EndDate.HasValue)
            .WithMessage("تاريخ النهاية يجب أن يكون بعد تاريخ البداية.");

        RuleFor(x => x.Responsibilities)
            .MaximumLength(500).WithMessage("يجب ألا تتجاوز المسؤوليات 500 حرف.");
            
        RuleFor(x => x.ReasonForLeaving)
            .MaximumLength(200).WithMessage("يجب ألا يتجاوز سبب ترك العمل 200 حرف.");
    }
}
