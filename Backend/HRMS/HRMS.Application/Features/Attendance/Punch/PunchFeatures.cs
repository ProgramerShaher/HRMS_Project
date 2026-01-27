using FluentValidation;
using HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Attendance.Punch;

// ═══════════════════════════════════════════════════════════
// 1. REGISTER PUNCH COMMAND
// ═══════════════════════════════════════════════════════════
public record RegisterPunchCommand(int EmployeeId, string PunchType, DateTime PunchTime, string? DeviceId, string? LocationCoordinates) : IRequest<Result<long>>;

public class RegisterPunchValidator : AbstractValidator<RegisterPunchCommand>
{
    public RegisterPunchValidator()
    {
        RuleFor(p => p.EmployeeId).GreaterThan(0);
        RuleFor(p => p.PunchType).Must(t => t == "IN" || t == "OUT").WithMessage("نوع البصمة يجب أن يكون IN أو OUT");
        RuleFor(p => p.PunchTime).NotEmpty();
    }
}

public class RegisterPunchHandler : IRequestHandler<RegisterPunchCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public RegisterPunchHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<long>> Handle(RegisterPunchCommand request, CancellationToken cancellationToken)
    {
        // 1. Log the Punch
        var log = new RawPunchLog
        {
            EmployeeId = request.EmployeeId,
            PunchTime = request.PunchTime,
            PunchType = request.PunchType,
            DeviceId = request.DeviceId ?? "API",
            IsProcessed = 0 // ProcessAttendance will handle this flag if needed (currently ProcessAttendance reads RawLogs irrespective of IsProcessed, usually)
        };

        _context.RawPunchLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. Trigger Process Attendance Logic
        // We trigger it for the day of the punch.
        // Note: For Cross-Day shifts (OUT next day), processing TargetDate (PunchTime.Date) might need logic. 
        // ProcessAttendanceHandler fetches punches for Day and Day+1. 
        // If this is an OUT punch on Day+1 early morning, processing Day might use it.
        // If I process PunchTime.Date, it refreshes that day.
        
        // We will trigger for the Punch Date.
        var processCommand = new ProcessAttendanceCommand(request.PunchTime.Date);
        
        // Fire and forget or Wait? User said "trigger logic". Usually wait to ensure consistency for returns if meaningful.
        // But ProcessAttendance returns count. We can await it.
        await _mediator.Send(processCommand, cancellationToken);
        
        // Optional: If it was a cross-day shift end (next day), user might expect Previous Day to update.
        // Simple heuristic: If PunchTime is early morning (e.g. < 8 AM) AND PunchType is OUT, maybe try processing Yesterday too?
        // For now, sticking to PunchDate to be deterministic as per User Request "target day immediately".
        // (Usually Target Day for a punch IS the Date of the punch).

        return Result<long>.Success(log.LogId, "تم تسجيل البصمة ومعالجة الحضور بنجاح");
    }
}
