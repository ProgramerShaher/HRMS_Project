using HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Punch.RegisterPunch;

/// <summary>
/// Handler for registering employee punch.
/// Records punch in raw logs and triggers attendance processing for the day.
/// </summary>
public class RegisterPunchCommandHandler : IRequestHandler<RegisterPunchCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public RegisterPunchCommandHandler(
        IApplicationDbContext context, 
        IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<long>> Handle(
        RegisterPunchCommand request, 
        CancellationToken cancellationToken)
    {
        var localPunchTime = request.PunchTime.AddHours(3);
        var today = localPunchTime.Date;
        
        var startUtc = today.AddHours(-3);
        var endUtc = today.AddDays(1).AddHours(-3);

        // 1. Double Check-In Validation
        if (request.PunchType == "IN")
        {
            var existingIn = await _context.RawPunchLogs
                .AnyAsync(p => p.EmployeeId == request.EmployeeId 
                            && p.PunchType == "IN" 
                            && p.PunchTime >= startUtc 
                            && p.PunchTime < endUtc, cancellationToken);
            
            if (existingIn)
            {
                return Result<long>.Failure("الموظف مسجل دخول بالفعل لهذا اليوم");
            }
        }

        // 2. Early Check-Out Validation
        if (request.PunchType == "OUT")
        {
            // First, make sure they actually checked IN
            var hasCheckedIn = await _context.RawPunchLogs
                .AnyAsync(p => p.EmployeeId == request.EmployeeId 
                            && p.PunchType == "IN" 
                            && p.PunchTime >= startUtc 
                            && p.PunchTime < endUtc, cancellationToken);

            if (!hasCheckedIn)
            {
                return Result<long>.Failure("عذراً، يجب تسجيل الدخول أولاً");
            }

            var roster = await _context.EmployeeRosters
                .Include(r => r.ShiftType)
                .FirstOrDefaultAsync(r => r.EmployeeId == request.EmployeeId 
                                       && r.RosterDate == today 
                                       && r.IsDeleted == 0, cancellationToken);

            if (roster != null && roster.ShiftType != null)
            {
                // Shift endTime parsing (e.g., "16:00")
                var shiftEndSpan = TimeSpan.Parse(roster.ShiftType.EndTime); 
                var shiftEndTime = today.Add(shiftEndSpan);

                if (localPunchTime < shiftEndTime)
                {
                    // Check if they have an approved EarlyExit permission
                    var hasPermission = await _context.PermissionRequests
                        .AnyAsync(pr => pr.EmployeeId == request.EmployeeId
                                     && pr.PermissionDate.Date == today
                                     && pr.PermissionType == "EarlyExit"
                                     && pr.Status == "APPROVED", cancellationToken);

                    if (!hasPermission)
                    {
                        return Result<long>.Failure("عذراً، لا يمكنك تسجيل الانصراف قبل انتهاء الدوام بدون إذن خروج معتمد");
                    }
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        // المرحلة 1: تسجيل البصمة في السجل الخام
        // Phase 1: Log punch in raw punch logs
        // ═══════════════════════════════════════════════════════════
        
        var log = new RawPunchLog
        {
            EmployeeId = request.EmployeeId,
            PunchTime = request.PunchTime,
            PunchType = request.PunchType,
            DeviceId = request.DeviceId ?? "API",
            IsProcessed = 0
        };

        _context.RawPunchLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════
        // المرحلة 2: تشغيل معالجة الحضور تلقائياً
        // Phase 2: Trigger attendance processing automatically
        // ═══════════════════════════════════════════════════════════
        
        var processCommand = new ProcessAttendanceCommand(today);
        await _mediator.Send(processCommand, cancellationToken);

        return Result<long>.Success(log.LogId, "تم تسجيل البصمة بنجاح");
    }
}
