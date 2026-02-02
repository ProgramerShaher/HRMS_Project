using FluentValidation;

namespace HRMS.Application.Features.Leaves.Balances.Commands.InitializeBalances;

/// <summary>
/// Validator for leave balance initialization command.
/// Ensures valid leave type, year range, and custom days if provided.
/// </summary>
public class InitializeBalancesValidator : AbstractValidator<InitializeBalancesCommand>
{
    public InitializeBalancesValidator()
    {
        // التحقق من معرف نوع الإجازة
        // Validate leave type ID
        RuleFor(x => x.LeaveTypeId)
            .GreaterThan(0)
            .WithMessage("معرف نوع الإجازة مطلوب");

        // التحقق من السنة
        // Validate year is within reasonable range
        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo((short)2020)
            .WithMessage("السنة يجب أن تكون 2020 أو أحدث")
            .LessThanOrEqualTo((short)2030)
            .WithMessage("السنة يجب ألا تتجاوز 2030");

        // التحقق من الأيام المخصصة (إذا تم توفيرها)
        // Validate custom days if provided
        RuleFor(x => x.CustomDays)
            .GreaterThan(0)
            .When(x => x.CustomDays.HasValue)
            .WithMessage("الأيام المخصصة يجب أن تكون أكبر من صفر")
            .LessThanOrEqualTo(365)
            .When(x => x.CustomDays.HasValue)
            .WithMessage("الأيام المخصصة يجب ألا تتجاوز 365 يوم");

        // التحقق من معرف القسم (إذا تم توفيره)
        // Validate department ID if provided
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .When(x => x.DepartmentId.HasValue)
            .WithMessage("معرف القسم غير صحيح");
    }
}
