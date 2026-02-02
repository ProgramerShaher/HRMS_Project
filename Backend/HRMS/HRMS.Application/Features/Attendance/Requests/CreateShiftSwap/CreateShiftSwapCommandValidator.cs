using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Requests.CreateShiftSwap;

/// <summary>
/// Validator for shift swap request creation.
/// Validates employee IDs, date, and roster existence.
/// </summary>
public class CreateShiftSwapCommandValidator : AbstractValidator<CreateShiftSwapCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateShiftSwapCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(p => p.RequesterId).GreaterThan(0);
        
        // لا يمكن تبديل المناوبة مع نفسك
        // Cannot swap shift with yourself
        RuleFor(p => p.TargetEmployeeId)
            .GreaterThan(0)
            .NotEqual(p => p.RequesterId)
            .WithMessage("لا يمكن تبديل المناوبة مع نفسك");
        
        // لا يمكن طلب تبديل لتاريخ سابق
        // Cannot request swap for past date
        RuleFor(p => p.RosterDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("لا يمكن طلب تبديل لتاريخ سابق");
        
        // التحقق من وجود مناوبة للموظف الطالب في التاريخ المحدد
        // Verify requester has a roster on specified date
        RuleFor(p => p)
            .MustAsync(async (cmd, ct) => 
            {
                return await _context.EmployeeRosters
                    .AnyAsync(r => r.EmployeeId == cmd.RequesterId && 
                                   r.RosterDate == cmd.RosterDate && 
                                   r.IsOffDay == 0, ct);
            })
            .WithMessage("لا يوجد مناوبة لك في هذا التاريخ لتبديلها");
    }
}
