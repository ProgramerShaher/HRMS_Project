using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Jobs.Commands.UpdateJob;

public class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateJobCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.JobId)
            .GreaterThan(0).WithMessage("معرف الوظيفة غير صحيح");

        RuleFor(x => x.JobTitleAr)
            .NotEmpty().WithMessage("المسمى الوظيفي بالعربية مطلوب")
            .MaximumLength(100).WithMessage("المسمى الوظيفي لا يمكن أن يتجاوز 100 حرف")
            .MustAsync(BeUniqueTitle).WithMessage("المسمى الوظيفي موجود مسبقاً");

        RuleFor(x => x.DefaultGradeId)
            .MustAsync(GradeExists).When(x => x.DefaultGradeId.HasValue)
            .WithMessage("الدرجة الوظيفية غير موجودة");
    }

    private async Task<bool> BeUniqueTitle(UpdateJobCommand command, string titleAr, CancellationToken cancellationToken)
    {
        return !await _context.Jobs.AnyAsync(
            j => j.JobTitleAr == titleAr && j.JobId != command.JobId && j.IsDeleted == 0, 
            cancellationToken);
    }

    private async Task<bool> GradeExists(int? gradeId, CancellationToken cancellationToken)
    {
        if (!gradeId.HasValue) return true;
        return await _context.JobGrades.AnyAsync(g => g.JobGradeId == gradeId.Value && g.IsDeleted == 0, cancellationToken);
    }
}
