using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace HRMS.Application.Features.Attendance.Roster.Commands.UpdateRosterDay;

// 1. Command
public record UpdateRosterDayCommand : IRequest<Result<bool>>
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public int? ShiftId { get; set; } // Null if Off Day
    public bool IsOffDay { get; set; }
}

// 2. Validator
public class UpdateRosterDayValidator : AbstractValidator<UpdateRosterDayCommand>
{
    public UpdateRosterDayValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.Date).NotEmpty();
        // If not off day, ShiftId must be present
        RuleFor(x => x.ShiftId).GreaterThan(0).When(x => !x.IsOffDay).WithMessage("يجب تحديد المناوبة إذا لم يكن يوم عطلة");
    }
}

// 3. Handler
public class UpdateRosterDayHandler : IRequestHandler<UpdateRosterDayCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateRosterDayHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateRosterDayCommand request, CancellationToken cancellationToken)
    {
        // Find existing record
        var rosterRecord = await _context.EmployeeRosters
            .FirstOrDefaultAsync(r => r.EmployeeId == request.EmployeeId && r.RosterDate.Date == request.Date.Date, cancellationToken);

        if (rosterRecord == null)
        {
            // Create new
            rosterRecord = new EmployeeRoster
            {
                EmployeeId = request.EmployeeId,
                RosterDate = request.Date.Date,
                Status = "Scheduled"
            };
            await _context.EmployeeRosters.AddAsync(rosterRecord, cancellationToken);
        }

        // Update fields
        rosterRecord.IsOffDay = (byte)(request.IsOffDay ? 1 : 0);
        rosterRecord.ShiftId = request.IsOffDay ? null : request.ShiftId;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, "تم تحديث المناوبة بنجاح");
    }
}
