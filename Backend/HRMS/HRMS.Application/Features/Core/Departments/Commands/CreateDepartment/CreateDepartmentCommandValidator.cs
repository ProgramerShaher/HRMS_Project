using FluentValidation;

namespace HRMS.Application.Features.Core.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator()
        {
            RuleFor(x => x.DeptNameAr).NotEmpty().WithMessage("اسم القسم مطلوب").MaximumLength(100);
        }
    }
}
