using FluentValidation;

namespace HRMS.Application.Features.Attendance.Requests.CancelShiftSwap;

public class CancelShiftSwapCommandValidator : AbstractValidator<CancelShiftSwapCommand>
{
    public CancelShiftSwapCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0);
    }
}
