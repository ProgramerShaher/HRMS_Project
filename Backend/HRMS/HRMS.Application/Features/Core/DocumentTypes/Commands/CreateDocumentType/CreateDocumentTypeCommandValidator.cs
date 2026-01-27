using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.DocumentTypes.Commands.CreateDocumentType;

/// <summary>
/// محقق صحة بيانات أمر إنشاء نوع الوثيقة
/// </summary>
public class CreateDocumentTypeCommandValidator : AbstractValidator<CreateDocumentTypeCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateDocumentTypeCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.DocumentTypeNameAr)
            .NotEmpty().WithMessage("اسم نوع الوثيقة بالعربية مطلوب")
            .MaximumLength(100).WithMessage("اسم نوع الوثيقة لا يمكن أن يتجاوز 100 حرف")
            .MustAsync(BeUniqueNameAr).WithMessage("اسم نوع الوثيقة بالعربية موجود مسبقاً");

        RuleFor(x => x.DocumentTypeNameEn)
            .NotEmpty().WithMessage("اسم نوع الوثيقة بالإنجليزية مطلوب")
            .MaximumLength(100).WithMessage("اسم نوع الوثيقة لا يمكن أن يتجاوز 100 حرف");

        RuleFor(x => x.AllowedExtensions)
            .Must(BeValidExtensions).When(x => !string.IsNullOrEmpty(x.AllowedExtensions))
            .WithMessage("الامتدادات غير صحيحة. يجب أن تكون بصيغة: .pdf,.jpg,.png");

        RuleFor(x => x.DefaultExpiryDays)
            .NotNull().When(x => x.HasExpiry)
            .WithMessage("عدد أيام الصلاحية مطلوب عند تفعيل تاريخ الانتهاء")
            .GreaterThan(0).When(x => x.HasExpiry && x.DefaultExpiryDays.HasValue)
            .WithMessage("عدد أيام الصلاحية يجب أن يكون أكبر من صفر");

        RuleFor(x => x.MaxFileSizeMB)
            .GreaterThan(0).When(x => x.MaxFileSizeMB.HasValue)
            .WithMessage("الحد الأقصى لحجم الملف يجب أن يكون أكبر من صفر")
            .LessThanOrEqualTo(100).When(x => x.MaxFileSizeMB.HasValue)
            .WithMessage("الحد الأقصى لحجم الملف لا يمكن أن يتجاوز 100 ميجابايت");
    }

    private async Task<bool> BeUniqueNameAr(string nameAr, CancellationToken cancellationToken)
    {
        return !await _context.DocumentTypes.AnyAsync(d => d.DocumentTypeNameAr == nameAr, cancellationToken);
    }

    private bool BeValidExtensions(string? extensions)
    {
        if (string.IsNullOrEmpty(extensions)) return true;
        
        var parts = extensions.Split(',', StringSplitOptions.RemoveEmptyEntries);
        return parts.All(ext => ext.Trim().StartsWith(".") && ext.Trim().Length > 1);
    }
}
