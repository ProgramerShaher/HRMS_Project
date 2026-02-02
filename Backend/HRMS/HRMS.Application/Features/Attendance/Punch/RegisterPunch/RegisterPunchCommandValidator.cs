using FluentValidation;

namespace HRMS.Application.Features.Attendance.Punch.RegisterPunch;

/// <summary>
/// Validator for punch registration.
/// Ensures valid employee ID, punch type, and timestamp.
/// </summary>
public class RegisterPunchCommandValidator : AbstractValidator<RegisterPunchCommand>
{
    public RegisterPunchCommandValidator()
    {
        // التحقق من معرف الموظف
        // Validate employee ID
        RuleFor(p => p.EmployeeId)
            .GreaterThan(0)
            .WithMessage("معرف الموظف مطلوب");

        // التحقق من نوع البصمة (دخول أو خروج فقط)
        // Validate punch type (IN or OUT only)
        RuleFor(p => p.PunchType)
            .Must(t => t == "IN" || t == "OUT")
            .WithMessage("نوع البصمة يجب أن يكون IN أو OUT");

        // التحقق من وقت البصمة
        // Validate punch timestamp
        RuleFor(p => p.PunchTime)
            .NotEmpty()
            .WithMessage("وقت البصمة مطلوب");
    }
}
