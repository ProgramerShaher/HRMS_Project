using FluentValidation;

namespace HRMS.Application.Features.Core.Branches.Commands.CreateBranch
{
    /// <summary>
    /// التحقق من صحة بيانات إنشاء فرع جديد
    /// </summary>
    public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
    {
        public CreateBranchCommandValidator()
        {
            RuleFor(x => x.BranchNameAr)
                .NotEmpty().WithMessage("اسم الفرع بالعربية مطلوب")
                .MaximumLength(100).WithMessage("اسم الفرع يجب ألا يتجاوز 100 حرف");

            RuleFor(x => x.BranchNameEn)
                .MaximumLength(100).WithMessage("اسم الفرع بالإنجليزية يجب ألا يتجاوز 100 حرف");

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("العنوان يجب ألا يتجاوز 200 حرف");
        }
    }
}
