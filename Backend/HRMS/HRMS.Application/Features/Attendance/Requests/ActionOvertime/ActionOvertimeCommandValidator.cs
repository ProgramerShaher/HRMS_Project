using FluentValidation;

namespace HRMS.Application.Features.Attendance.Requests.ActionOvertime;

public class ActionOvertimeCommandValidator : AbstractValidator<ActionOvertimeCommand>
{
    public ActionOvertimeCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0);
        RuleFor(x => x.ManagerId).GreaterThan(0);
        RuleFor(x => x.Action).Must(a => a == "APPROVE" || a == "REJECT").WithMessage("الإجراء يجب أن يكون APPROVE أو REJECT");
        RuleFor(x => x.ApprovedHours).GreaterThan(0).When(x => x.Action == "APPROVE").WithMessage("الساعات المعتمدة مطلوبة عند الموافقة");
    }
}
