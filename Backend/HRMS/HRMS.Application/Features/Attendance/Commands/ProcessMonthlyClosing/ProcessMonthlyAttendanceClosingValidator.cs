using FluentValidation;

namespace HRMS.Application.Features.Attendance.Commands.ProcessMonthlyClosing;

/// <summary>
/// Fluent validation rules for monthly attendance closing command
/// </summary>
public class ProcessMonthlyAttendanceClosingValidator : AbstractValidator<ProcessMonthlyAttendanceClosingCommand>
{
    public ProcessMonthlyAttendanceClosingValidator()
    {
        // التحقق من صحة السنة
        RuleFor(x => x.Year)
            .GreaterThan((short)2020)
            .WithMessage("السنة يجب أن تكون أكبر من 2020")
            .LessThanOrEqualTo((short)DateTime.Now.Year)
            .WithMessage("لا يمكن إغلاق فترة في المستقبل");

        // التحقق من صحة الشهر
        RuleFor(x => x.Month)
            .GreaterThan((byte)0)
            .WithMessage("الشهر يجب أن يكون بين 1 و 12")
            .LessThanOrEqualTo((byte)12)
            .WithMessage("الشهر يجب أن يكون بين 1 و 12");

        // التحقق من معرف المستخدم
        RuleFor(x => x.ClosedByUserId)
            .GreaterThan(0)
            .WithMessage("معرف المستخدم مطلوب");

        // التحقق من عدم إغلاق فترة مستقبلية
        RuleFor(x => x)
            .Must(cmd => 
            {
                var targetDate = new DateTime(cmd.Year, cmd.Month, 1);
                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                return targetDate <= currentDate;
            })
            .WithMessage("لا يمكن إغلاق فترة لم تنتهي بعد");
    }
}
