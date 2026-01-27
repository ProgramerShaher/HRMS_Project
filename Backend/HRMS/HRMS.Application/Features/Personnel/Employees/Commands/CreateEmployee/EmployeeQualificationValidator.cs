using FluentValidation;
using HRMS.Application.DTOs.Personnel;

namespace HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee;

public class EmployeeQualificationValidator : AbstractValidator<EmployeeQualificationDto>
{
    public EmployeeQualificationValidator()
    {
        RuleFor(x => x.DegreeType)
            .NotEmpty().WithMessage("نوع الدرجة العلمية مطلوب.")
            .MaximumLength(50).WithMessage("يجب ألا يتجاوز نوع الدرجة العلمية 50 حرفاً.");

        RuleFor(x => x.MajorAr)
            .NotEmpty().WithMessage("التخصص مطلوب.")
            .MaximumLength(100).WithMessage("يجب ألا يتجاوز التخصص 100 حرف.")
            .Matches(@"^[\u0600-\u06FF\s]+$").WithMessage("التخصص يجب أن يكون بالعربية.");

        RuleFor(x => x.Grade)
            .MaximumLength(20).WithMessage("يجب ألا يتجاوز التقدير 20 حرفاً.");
            
        RuleFor(x => x.UniversityAr)
            .MaximumLength(200).WithMessage("يجب ألا يتجاوز اسم الجامعة 200 حرف.");
    }
}
