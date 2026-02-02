using FluentValidation;

namespace HRMS.Application.Features.Attendance.Commands.ManualCorrection;

public class ManualCorrectionCommandValidator : AbstractValidator<ManualCorrectionCommand>
{
    public ManualCorrectionCommandValidator()
    {
        RuleFor(x => x.DailyAttendanceId)
            .GreaterThan(0).WithMessage("معرف السجل مطلوب");

        RuleFor(x => x.CorrectionType)
            .Must(type => new[] { "InTime", "OutTime", "Status" }.Contains(type))
            .WithMessage("نوع التصحيح غير صالح. القيم المسموحة: InTime, OutTime, Status");

        RuleFor(x => x.NewValue)
            .NotEmpty().WithMessage("القيمة الجديدة مطلوبة");

        RuleFor(x => x.AuditNote)
            .NotEmpty().WithMessage("ملاحظة التدقيق مطلوبة لتوضيح سبب التعديل");
    }
}
