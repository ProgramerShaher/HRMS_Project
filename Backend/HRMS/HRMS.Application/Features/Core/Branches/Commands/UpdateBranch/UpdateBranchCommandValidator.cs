using FluentValidation;

namespace HRMS.Application.Features.Core.Branches.Commands.UpdateBranch
{
    public class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
    {
        public UpdateBranchCommandValidator()
        {
            RuleFor(x => x.BranchId).GreaterThan(0).WithMessage("معرف الفرع مطلوب");
            RuleFor(x => x.BranchNameAr).NotEmpty().MaximumLength(100);
        }
    }
}
