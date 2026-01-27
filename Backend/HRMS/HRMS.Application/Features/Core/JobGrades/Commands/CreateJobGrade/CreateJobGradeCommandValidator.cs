using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HRMS.Application.Features.Core.JobGrades.Commands.CreateJobGrade;

/// <summary>
/// محقق صحة بيانات أمر إنشاء الدرجة الوظيفية
/// </summary>
public class CreateJobGradeCommandValidator : AbstractValidator<CreateJobGradeCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateJobGradeCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.GradeCode)
            .NotEmpty().WithMessage("رمز الدرجة الوظيفية مطلوب")
            .MaximumLength(20).WithMessage("رمز الدرجة لا يمكن أن يتجاوز 20 حرف")
            .MustAsync(BeUniqueCode).WithMessage("رمز الدرجة الوظيفية موجود مسبقاً");

        RuleFor(x => x.GradeNameAr)
            .NotEmpty().WithMessage("اسم الدرجة بالعربية مطلوب")
            .MaximumLength(100).WithMessage("اسم الدرجة لا يمكن أن يتجاوز 100 حرف");

        RuleFor(x => x.GradeNameEn)
            .NotEmpty().WithMessage("اسم الدرجة بالإنجليزية مطلوب")
            .MaximumLength(100).WithMessage("اسم الدرجة لا يمكن أن يتجاوز 100 حرف");

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

    private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
    {
        return !await _context.JobGrades.AnyAsync(g => g.GradeCode == code, cancellationToken);
    }

    private async Task<bool> BeUniqueLevel(int level, CancellationToken cancellationToken)
    {
        return !await _context.JobGrades.AnyAsync(g => g.GradeLevel == level, cancellationToken);
    }

    private bool BeValidJson(string? json)
    {
        if (string.IsNullOrEmpty(json)) return true;
        
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
