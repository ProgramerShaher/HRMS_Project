using HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities;
using MediatR;

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
        // ═══════════════════════════════════════════════════════════
        // المرحلة 1: تسجيل البصمة في السجل الخام
        // Phase 1: Log punch in raw punch logs
        // ═══════════════════════════════════════════════════════════
        
        // تسجيل البصمة في جدول السجلات الخام
        // هذا الجدول يحتفظ بجميع البصمات قبل المعالجة
        // Record punch in raw logs table
        // This table keeps all punches before processing
        var log = new RawPunchLog
        {
            EmployeeId = request.EmployeeId,
            PunchTime = request.PunchTime,
            PunchType = request.PunchType,
            DeviceId = request.DeviceId ?? "API", // إذا لم يتم تحديد جهاز، نستخدم "API"
            IsProcessed = 0 // سيتم معالجتها بواسطة ProcessAttendance
        };

        _context.RawPunchLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════
        // المرحلة 2: تشغيل معالجة الحضور تلقائياً
        // Phase 2: Trigger attendance processing automatically
        // ═══════════════════════════════════════════════════════════
        
        // معالجة الحضور لليوم الذي تمت فيه البصمة
        // Process attendance for the day of the punch
        // ملاحظة: بالنسبة للمناوبات العابرة لمنتصف الليل، معالج ProcessAttendance
        // يجلب البصمات من اليوم الحالي واليوم التالي تلقائياً
        // Note: For cross-day shifts, ProcessAttendance handler
        // automatically fetches punches from current day and next day
        var processCommand = new ProcessAttendanceCommand(request.PunchTime.Date);
        await _mediator.Send(processCommand, cancellationToken);

        return Result<long>.Success(log.LogId, "تم تسجيل البصمة ومعالجة الحضور بنجاح");
    }
}
