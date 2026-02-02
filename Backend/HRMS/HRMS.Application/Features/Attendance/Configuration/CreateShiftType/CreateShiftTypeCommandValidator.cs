using FluentValidation;

namespace HRMS.Application.Features.Attendance.Configuration.CreateShiftType;

/// <summary>
/// Validator for creating shift type.
/// Ensures shift name and time formats are valid.
/// </summary>
public class CreateShiftTypeCommandValidator : AbstractValidator<CreateShiftTypeCommand>
{
    public CreateShiftTypeCommandValidator()
    {
        // التحقق من اسم المناوبة
        // Validate shift name
        RuleFor(p => p.ShiftNameAr)
            .NotEmpty()
            .WithMessage("اسم المناوبة مطلوب");

        // التحقق من صيغة وقت البدء (HH:mm)
        // Validate start time format
        RuleFor(p => p.StartTime)
            .Matches(@"^([01][0-9]|2[0-3]):[0-5][0-9]$")
            .WithMessage("صيغة وقت البدء غير صحيحة (HH:mm)");

        // التحقق من صيغة وقت النهاية (HH:mm)
        // Validate end time format
        RuleFor(p => p.EndTime)
            .Matches(@"^([01][0-9]|2[0-3]):[0-5][0-9]$")
            .WithMessage("صيغة وقت النهاية غير صحيحة (HH:mm)");
    }
}
