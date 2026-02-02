using FluentValidation;

namespace HRMS.Application.Features.Attendance.Requests.UpdateShiftSwap;

public class UpdateShiftSwapCommandValidator : AbstractValidator<UpdateShiftSwapCommand>
{
    public UpdateShiftSwapCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0);
        RuleFor(x => x.TargetEmployeeId).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().WithMessage("السبب مطلوب");
    }
}
