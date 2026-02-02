using FluentValidation;

namespace HRMS.Application.Features.Attendance.Requests.ActionShiftSwap;

public class ActionShiftSwapCommandValidator : AbstractValidator<ActionShiftSwapCommand>
{
    public ActionShiftSwapCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0);
        RuleFor(x => x.ManagerId).GreaterThan(0);
        RuleFor(x => x.Action)
            .Must(a => a == "APPROVE" || a == "REJECT")
            .WithMessage("الإجراء يجب أن يكون APPROVE أو REJECT");
    }
}
