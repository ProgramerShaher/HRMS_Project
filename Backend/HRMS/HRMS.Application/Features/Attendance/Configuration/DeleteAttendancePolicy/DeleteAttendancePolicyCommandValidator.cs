using FluentValidation;

namespace HRMS.Application.Features.Attendance.Configuration.DeleteAttendancePolicy;

/// <summary>
/// Validator for deleting attendance policy.
/// Ensures valid policy ID is provided.
/// </summary>
public class DeleteAttendancePolicyCommandValidator : AbstractValidator<DeleteAttendancePolicyCommand>
{
    public DeleteAttendancePolicyCommandValidator()
    {
        // التحقق من معرف السياسة
        // Validate policy ID
        RuleFor(x => x.PolicyId)
            .GreaterThan(0)
            .WithMessage("معرف السياسة مطلوب");
    }
}
