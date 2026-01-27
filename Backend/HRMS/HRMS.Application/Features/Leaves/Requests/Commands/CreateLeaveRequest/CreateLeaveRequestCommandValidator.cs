using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.Requests.Commands.CreateLeaveRequest;

public class CreateLeaveRequestCommandValidator : AbstractValidator<CreateLeaveRequestCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateLeaveRequestCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(p => p.EmployeeId)
            .GreaterThan(0).WithMessage("معرف الموظف مطلوب.");

        RuleFor(p => p.LeaveTypeId)
            .GreaterThan(0).WithMessage("نوع الإجازة مطلوب.");

        RuleFor(p => p.StartDate)
            .NotEmpty().WithMessage("تاريخ البدء مطلوب.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("لا يمكن طلب إجازة في تاريخ سابق.");

        RuleFor(p => p.EndDate)
            .NotEmpty().WithMessage("تاريخ الانتهاء مطلوب.")
            .GreaterThanOrEqualTo(p => p.StartDate).WithMessage("تاريخ الانتهاء يجب أن يكون بعد تاريخ البدء.");

        RuleFor(p => p.Reason)
            .NotEmpty().WithMessage("سبب الإجازة مطلوب.")
            .MaximumLength(500).WithMessage("السبب يجب ألا يتجاوز 500 حرف.");
    }
}
