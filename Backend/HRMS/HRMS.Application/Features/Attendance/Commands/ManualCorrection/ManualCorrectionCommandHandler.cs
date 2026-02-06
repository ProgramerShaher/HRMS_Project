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
		// داخل الـ Handler
		switch (request.CorrectionType)
		{
			case "InTime":
				oldValue = record.ActualInTime?.ToString("o") ?? "null";
				// استخدام TimeOnly لضمان عدم تغيير تاريخ اليوم
				if (TimeSpan.TryParse(request.NewValue, out var newInTime))
				{
					// نأخذ تاريخ السجل الأصلي + الوقت الجديد
					record.ActualInTime = record.AttendanceDate.Date.Add(newInTime);

					// 🔥 الإصلاح: تصفير التأخير لأن المدير تدخل يدوياً
					record.LateMinutes = 0;
					record.Status = "PRESENT"; // ضمان أن الحالة حضور
				}
				else return Result<bool>.Failure("تنسيق الوقت غير صحيح (استخدم HH:mm:ss)");
				break;

			case "OutTime":
				oldValue = record.ActualOutTime?.ToString("o") ?? "null";
				if (TimeSpan.TryParse(request.NewValue, out var newOutTime))
				{
					record.ActualOutTime = record.AttendanceDate.Date.Add(newOutTime);

					// 🔥 الإصلاح: تصفير الخروج المبكر
					record.EarlyLeaveMinutes = 0;
					// يمكن هنا إضافة منطق إعادة حساب الإضافي إذا أردت
				}
				else return Result<bool>.Failure("تنسيق الوقت غير صحيح");
				break;

			case "Status":
				oldValue = record.Status ?? "null";
				record.Status = request.NewValue;
				// إذا غير الحالة إلى "إجازة"، صفر كل شيء
				if (request.NewValue == "LEAVE" || request.NewValue == "OFF")
				{
					record.LateMinutes = 0;
					record.EarlyLeaveMinutes = 0;
					record.OvertimeMinutes = 0;
				}
				break;

			default:
				return Result<bool>.Failure("نوع التصحيح غير مدعوم (استخدم InTime, OutTime, Status)");
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
