using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HRMS.Application.Features.Core.JobGrades.Commands.UpdateJobGrade;

public class UpdateJobGradeCommandValidator : AbstractValidator<UpdateJobGradeCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateJobGradeCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.JobGradeId)
            .GreaterThan(0).WithMessage("معرف الدرجة الوظيفية غير صحيح");

        RuleFor(x => x.GradeCode)
            .NotEmpty().WithMessage("رمز الدرجة الوظيفية مطلوب")
            .MaximumLength(20).WithMessage("رمز الدرجة لا يمكن أن يتجاوز 20 حرف")
            .MustAsync(BeUniqueCode).WithMessage("رمز الدرجة الوظيفية موجود مسبقاً");

        RuleFor(x => x.GradeLevel)
            .GreaterThan(0).WithMessage("مستوى الدرجة يجب أن يكون أكبر من صفر")
            .MustAsync(BeUniqueLevel).WithMessage("مستوى الدرجة موجود مسبقاً");

        RuleFor(x => x.MinSalary)
            .GreaterThan(0).WithMessage("الحد الأدنى للراتب يجب أن يكون أكبر من صفر");

        RuleFor(x => x.MaxSalary)
            .GreaterThan(0).WithMessage("الحد الأقصى للراتب يجب أن يكون أكبر من صفر")
            .GreaterThan(x => x.MinSalary).WithMessage("الحد الأقصى للراتب يجب أن يكون أكبر من الحد الأدنى");

        RuleFor(x => x.BenefitsConfig)
            .Must(BeValidJson).When(x => !string.IsNullOrEmpty(x.BenefitsConfig))
            .WithMessage("إعدادات المزايا يجب أن تكون بصيغة JSON صحيحة");
    }

    private async Task<bool> BeUniqueCode(UpdateJobGradeCommand command, string code, CancellationToken cancellationToken)
    {
        return !await _context.JobGrades.AnyAsync(
            g => g.GradeCode == code && g.JobGradeId != command.JobGradeId, 
            cancellationToken);
    }

    private async Task<bool> BeUniqueLevel(UpdateJobGradeCommand command, int level, CancellationToken cancellationToken)
    {
        return !await _context.JobGrades.AnyAsync(
            g => g.GradeLevel == level && g.JobGradeId != command.JobGradeId, 
            cancellationToken);
    }

    private bool BeValidJson(string? json)
    {
        if (string.IsNullOrEmpty(json)) return true;
        try { JsonDocument.Parse(json); return true; } catch { return false; }
    }
}
