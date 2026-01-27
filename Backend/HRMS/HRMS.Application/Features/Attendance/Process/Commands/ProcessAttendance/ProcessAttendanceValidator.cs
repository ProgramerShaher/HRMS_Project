using FluentValidation;
using HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;

namespace HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;

public class ProcessAttendanceValidator : AbstractValidator<ProcessAttendanceCommand>
{
    public ProcessAttendanceValidator()
    {
        RuleFor(x => x.TargetDate)
            .NotEmpty().WithMessage("تاريخ المعالجة مطلوب")
            .LessThanOrEqualTo(DateTime.Today).WithMessage("لا يمكن معالجة تواريخ مستقبلية");
    }
}
