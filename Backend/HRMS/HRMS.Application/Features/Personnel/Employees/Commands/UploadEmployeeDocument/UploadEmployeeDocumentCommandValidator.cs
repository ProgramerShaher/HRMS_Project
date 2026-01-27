using FluentValidation;

namespace HRMS.Application.Features.Personnel.Employees.Commands.UploadEmployeeDocument;

public class UploadEmployeeDocumentCommandValidator : AbstractValidator<UploadEmployeeDocumentCommand>
{
    public UploadEmployeeDocumentCommandValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.DocumentTypeId).GreaterThan(0);
        
        RuleFor(x => x.File)
            .NotNull().WithMessage("لم يتم إرفاق ملف")
            .Must(file => file.Length > 0).WithMessage("الملف فارغ");

        // تحقق من تاريخ الانتهاء إذا كانت الوثيقة تحتاج ذلك (يمكن تخصيصه بناءً على النوع)
        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.Today).When(x => x.ExpiryDate.HasValue)
            .WithMessage("تاريخ الانتهاء يجب أن يكون في المستقبل");
    }
}
