using FluentValidation;

namespace HRMS.Application.Features.Attendance.Requests.UpdateOvertime;

public class UpdateOvertimeCommandValidator : AbstractValidator<UpdateOvertimeCommand>
{
    public UpdateOvertimeCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0);
        RuleFor(x => x.HoursRequested).GreaterThan(0).WithMessage("يجب أن تكون الساعات أكبر من صفر");
        RuleFor(x => x.Reason).NotEmpty().WithMessage("السبب مطلوب");
    }
}
