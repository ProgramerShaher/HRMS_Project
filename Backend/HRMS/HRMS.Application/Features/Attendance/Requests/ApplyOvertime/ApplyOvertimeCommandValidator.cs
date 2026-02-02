using FluentValidation;

namespace HRMS.Application.Features.Attendance.Requests.ApplyOvertime;

public class ApplyOvertimeCommandValidator : AbstractValidator<ApplyOvertimeCommand>
{
    public ApplyOvertimeCommandValidator()
    {
        RuleFor(p => p.EmployeeId).GreaterThan(0);
        RuleFor(p => p.WorkDate).NotEmpty().WithMessage("تاريخ العمل الإضافي مطلوب");
        RuleFor(p => p.HoursRequested).GreaterThan(0).WithMessage("يجب أن تكون الساعات أكبر من صفر");
        RuleFor(p => p.Reason).NotEmpty().WithMessage("السبب مطلوب");
    }
}
