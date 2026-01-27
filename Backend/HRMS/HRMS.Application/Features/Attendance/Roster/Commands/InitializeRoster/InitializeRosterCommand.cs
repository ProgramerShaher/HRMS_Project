using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.Features.Attendance.Roster.Commands.InitializeRoster;

// 1. Command
public record InitializeRosterCommand : IRequest<bool>
{
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ShiftId { get; set; }
}

// 2. Validator
public class InitializeRosterValidator : AbstractValidator<InitializeRosterCommand>
{
    private readonly IApplicationDbContext _context;

    public InitializeRosterValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("معرف الموظف مطلوب");

        RuleFor(x => x.ShiftId)
            .GreaterThan(0).WithMessage("معرف المناوبة مطلوب");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاريخ البدء مطلوب");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("تاريخ الانتهاء مطلوب")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("تاريخ الانتهاء يجب أن يكون بعد تاريخ البدء");
    }
}

// 3. Handler
public class InitializeRosterHandler : IRequestHandler<InitializeRosterCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public InitializeRosterHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(InitializeRosterCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Employee & Shift existence
        var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);
        if (!employeeExists) throw new FluentValidation.ValidationException("الموظف غير موجود");

        var shiftExists = await _context.ShiftTypes.AnyAsync(s => s.ShiftId == request.ShiftId, cancellationToken);
        if (!shiftExists) throw new FluentValidation.ValidationException("نوع المناوبة غير موجود");

        // 2. Prepare Records
        var rostersToAdd = new List<EmployeeRoster>();
        var existingDates = await _context.EmployeeRosters
            .Where(r => r.EmployeeId == request.EmployeeId && r.RosterDate >= request.StartDate && r.RosterDate <= request.EndDate)
            .Select(r => r.RosterDate)
            .ToListAsync(cancellationToken);

        var existingDatesSet = new HashSet<DateTime>(existingDates);

        for (var date = request.StartDate.Date; date <= request.EndDate.Date; date = date.AddDays(1))
        {
            if (existingDatesSet.Contains(date)) continue; // Skip duplicates

            var isFriday = date.DayOfWeek == DayOfWeek.Friday;
            
            var roster = new EmployeeRoster
            {
                EmployeeId = request.EmployeeId,
                RosterDate = date,
                ShiftId = isFriday ? null : request.ShiftId,
                IsOffDay = (byte)(isFriday ? 1 : 0)
            };

            rostersToAdd.Add(roster);
        }

        if (rostersToAdd.Any())
        {
            await _context.EmployeeRosters.AddRangeAsync(rostersToAdd, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}
