using FluentValidation;

namespace HRMS.Application.Features.Attendance.Requests.CancelOvertime;

public class CancelOvertimeCommandValidator : AbstractValidator<CancelOvertimeCommand>
{
    public CancelOvertimeCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0);
    }
}
