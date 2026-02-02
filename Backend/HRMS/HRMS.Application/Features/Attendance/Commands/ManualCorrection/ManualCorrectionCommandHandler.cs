using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using HRMS.Core.Entities.Attendance;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace HRMS.Application.Features.Attendance.Commands.ManualCorrection;

public class ManualCorrectionCommandHandler : IRequestHandler<ManualCorrectionCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public ManualCorrectionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(ManualCorrectionCommand request, CancellationToken cancellationToken)
    {
        var record = await _context.DailyAttendances
            .Include(d => d.Employee)
            .FirstOrDefaultAsync(d => d.RecordId == request.DailyAttendanceId, cancellationToken);

        if (record == null)
            return Result<bool>.Failure("سجل الحضور غير موجود", 404);

        string oldValue = string.Empty;
        string fieldName = request.CorrectionType;

        // تطبيق التغيير حسب النوع
        switch (request.CorrectionType)
        {
            case "InTime":
                oldValue = record.ActualInTime?.ToString("o") ?? "null";
                if (DateTime.TryParse(request.NewValue, out var newInDate))
                {
                    record.ActualInTime = newInDate;
                    // إعادة حساب التأخير (بسيط هنا، المفروض بناءً على السياسة)
                    // TODO: Recalculate LateMinutes based on Shift
                }
                else return Result<bool>.Failure("تنسيق التاريخ غير صحيح");
                break;

            case "OutTime":
                oldValue = record.ActualOutTime?.ToString("o") ?? "null";
                if (DateTime.TryParse(request.NewValue, out var newOutDate))
                {
                    record.ActualOutTime = newOutDate;
                    // TODO: Recalculate OT/EarlyLeave
                }
                else return Result<bool>.Failure("تنسيق التاريخ غير صحيح");
                break;

            case "Status":
                oldValue = record.Status ?? "null";
                record.Status = request.NewValue;
                break;

            default:
                return Result<bool>.Failure("نوع التصحيح غير مدعوم");
        }

        // تسجيل التصحيح في Audit
        var correction = new AttendanceCorrection
        {
            EmployeeId = record.EmployeeId,
            AttendanceDate = record.AttendanceDate,
            DailyAttendanceId = record.RecordId,
            FieldName = fieldName,
            OldValue = oldValue,
            NewValue = request.NewValue,
            AuditNote = request.AuditNote,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Admin" // TODO: Get from CurrentUserService
        };

        _context.AttendanceCorrections.Add(correction);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم تصحيح السجل بنجاح");
    }
}
