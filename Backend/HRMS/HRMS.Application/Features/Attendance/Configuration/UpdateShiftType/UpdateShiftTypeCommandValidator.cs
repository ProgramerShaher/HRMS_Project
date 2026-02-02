using FluentValidation;

namespace HRMS.Application.Features.Attendance.Configuration.UpdateShiftType;

/// <summary>
/// Validator for updating shift type.
/// Ensures all required fields are valid.
/// </summary>
public class UpdateShiftTypeCommandValidator : AbstractValidator<UpdateShiftTypeCommand>
{
    public UpdateShiftTypeCommandValidator()
    {
        // التحقق من معرف المناوبة
        // Validate shift ID
        RuleFor(p => p.ShiftId)
            .GreaterThan(0)
            .WithMessage("معرف المناوبة مطلوب");

        // التحقق من اسم المناوبة
        // Validate shift name
        RuleFor(p => p.ShiftNameAr)
            .NotEmpty()
            .WithMessage("اسم المناوبة مطلوب");

        // التحقق من صيغة وقت البدء
        // Validate start time format
        RuleFor(p => p.StartTime)
            .Matches(@"^([01][0-9]|2[0-3]):[0-5][0-9]$")
            .WithMessage("صيغة وقت البدء غير صحيحة (HH:mm)");

        // التحقق من صيغة وقت النهاية
        // Validate end time format
        RuleFor(p => p.EndTime)
            .Matches(@"^([01][0-9]|2[0-3]):[0-5][0-9]$")
            .WithMessage("صيغة وقت النهاية غير صحيحة (HH:mm)");
    }
}
