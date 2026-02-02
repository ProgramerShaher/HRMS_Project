using FluentValidation;

namespace HRMS.Application.Features.Attendance.Configuration.DeleteShiftType;

/// <summary>
/// Validator for deleting shift type.
/// Ensures valid shift ID is provided.
/// </summary>
public class DeleteShiftTypeCommandValidator : AbstractValidator<DeleteShiftTypeCommand>
{
    public DeleteShiftTypeCommandValidator()
    {
        // التحقق من معرف المناوبة
        // Validate shift ID
        RuleFor(x => x.ShiftId)
            .GreaterThan(0)
            .WithMessage("معرف المناوبة مطلوب");
    }
}
