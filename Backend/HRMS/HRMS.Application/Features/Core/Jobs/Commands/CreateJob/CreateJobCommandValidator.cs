using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Jobs.Commands.CreateJob;

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateJobCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.JobTitleAr)
            .NotEmpty().WithMessage("المسمى الوظيفي بالعربية مطلوب")
            .MaximumLength(100).WithMessage("المسمى الوظيفي لا يمكن أن يتجاوز 100 حرف")
            .MustAsync(BeUniqueTitle).WithMessage("المسمى الوظيفي موجود مسبقاً");

        RuleFor(x => x.DefaultGradeId)
            .MustAsync(GradeExists).When(x => x.DefaultGradeId.HasValue)
            .WithMessage("الدرجة الوظيفية غير موجودة");
    }

    private async Task<bool> BeUniqueTitle(string titleAr, CancellationToken cancellationToken)
    {
        return !await _context.Jobs.AnyAsync(j => j.JobTitleAr == titleAr, cancellationToken);
    }

    private async Task<bool> GradeExists(int? gradeId, CancellationToken cancellationToken)
    {
        if (!gradeId.HasValue) return true;
        return await _context.JobGrades.AnyAsync(g => g.JobGradeId == gradeId.Value, cancellationToken);
    }
}
