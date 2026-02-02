using FluentValidation;

namespace HRMS.Application.Features.Attendance.Configuration.CreateAttendancePolicy;

/// <summary>
/// Validator for creating attendance policy.
/// Ensures policy name, grace period, and multipliers are within acceptable ranges.
/// </summary>
public class CreateAttendancePolicyCommandValidator : AbstractValidator<CreateAttendancePolicyCommand>
{
    public CreateAttendancePolicyCommandValidator()
    {
        // التحقق من اسم السياسة
        // Validate policy name
        RuleFor(p => p.PolicyNameAr)
            .NotEmpty()
            .WithMessage("اسم السياسة مطلوب")
            .MaximumLength(100)
            .WithMessage("اسم السياسة يجب ألا يتجاوز 100 حرف");

        // التحقق من فترة السماح للتأخير
        // Validate late grace period
        RuleFor(p => p.LateGraceMins)
            .GreaterThanOrEqualTo((short)0)
            .WithMessage("فترة السماح يجب أن تكون صفر أو أكثر")
            .LessThanOrEqualTo((short)60)
            .WithMessage("فترة السماح يجب ألا تتجاوز 60 دقيقة");

        // التحقق من معامل الوقت الإضافي العادي
        // Validate regular overtime multiplier
        RuleFor(p => p.OvertimeMultiplier)
            .GreaterThan(0)
            .WithMessage("معامل الوقت الإضافي يجب أن يكون أكبر من صفر")
            .LessThanOrEqualTo(5)
            .WithMessage("معامل الوقت الإضافي يجب ألا يتجاوز 5");

        // التحقق من معامل الوقت الإضافي للعطل
        // Validate weekend overtime multiplier
        RuleFor(p => p.WeekendOtMultiplier)
            .GreaterThan(0)
            .WithMessage("معامل الوقت الإضافي للعطل يجب أن يكون أكبر من صفر")
            .LessThanOrEqualTo(5)
            .WithMessage("معامل الوقت الإضافي للعطل يجب ألا يتجاوز 5");
    }
}
