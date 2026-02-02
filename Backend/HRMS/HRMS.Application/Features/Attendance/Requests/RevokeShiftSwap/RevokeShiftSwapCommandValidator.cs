using FluentValidation;

namespace HRMS.Application.Features.Attendance.Requests.RevokeShiftSwap;

public class RevokeShiftSwapCommandValidator : AbstractValidator<RevokeShiftSwapCommand>
{
    public RevokeShiftSwapCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0);
    }
}
