using FluentValidation;

namespace HRMS.Application.Features.Core.Jobs.Commands.CreateJob
{
    public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
    {
        public CreateJobCommandValidator()
        {
            RuleFor(x => x.JobTitleAr).NotEmpty().WithMessage("المسمى الوظيفي مطلوب").MaximumLength(100);
        }
    }
}
