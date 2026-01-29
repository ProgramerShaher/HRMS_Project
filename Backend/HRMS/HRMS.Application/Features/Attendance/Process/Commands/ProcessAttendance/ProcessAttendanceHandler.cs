using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;

public class ProcessAttendanceHandler : IRequestHandler<ProcessAttendanceCommand, int>
{
    private readonly IApplicationDbContext _context;

    public ProcessAttendanceHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(ProcessAttendanceCommand request, CancellationToken cancellationToken)
    {
        // 1. استخراج التاريخ من الطلب
        var targetDate = request.TargetDate.Date;

        try
        {
            // 2. استدعاء الـ Stored Procedure مباشرة من قاعدة البيانات
            // ملاحظة: قمنا بتمرير التاريخ كـ Parameter الأول، ورقم المستخدم (1) كـ Parameter الثاني للـ Audit
            // نستخدم ExecuteSqlRawAsync لأننا نقوم بعملية (Insert/Delete) ولا ننتظر Return Data
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC [HR_ATTENDANCE].[SP_PROCESS_ATTENDANCE] @TARGET_DATE = {0}, @USER_ID = {1}",
                new object[] { targetDate, 1 },
                cancellationToken
            );

            // 3. نرجع 1 كدلالة على نجاح العملية
            return 1;
        }
        catch (Exception ex)
        {
            // 4. في حالة حدوث خطأ في قاعدة البيانات، يتم رمي استثناء لتوضيح السبب
            throw new Exception($"خطأ أثناء تشغيل محرك المعالجة: {ex.Message}");
        }
    }
}