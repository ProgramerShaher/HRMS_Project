using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Performance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;
using System.Linq;

namespace HRMS.Application.Features.Performance.Appraisals.Commands.InitiateAppraisal;

public class InitiateAppraisalCommand : IRequest<Result<int>>
{
    public int EmployeeId { get; set; }
    public int CycleId { get; set; }
    public int? EvaluatorId { get; set; }
}

public class InitiateAppraisalCommandValidator : AbstractValidator<InitiateAppraisalCommand>
{
    public InitiateAppraisalCommandValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0).WithMessage("الموظف مطلوب");
        RuleFor(x => x.CycleId).GreaterThan(0).WithMessage("الدورة مطلوبة");
    }
}

public class InitiateAppraisalCommandHandler : IRequestHandler<InitiateAppraisalCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public InitiateAppraisalCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(InitiateAppraisalCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // التحقق من عدم وجود تقييم مسبق
            var existing = await _context.EmployeeAppraisals
                .AnyAsync(a => a.EmployeeId == request.EmployeeId && a.CycleId == request.CycleId && a.IsDeleted == 0, cancellationToken);

            if (existing)
                return Result<int>.Failure("تم إنشاء تقييم لهذا الموظف في هذه الدورة مسبقاً.");

            // جلب الدورة للحصول على تواريخها
            var cycle = await _context.AppraisalCycles
                .FirstOrDefaultAsync(c => c.CycleId == request.CycleId && c.IsDeleted == 0, cancellationToken);
            if (cycle == null) return Result<int>.Failure("دورة التقييم غير موجودة.");

            // تحديد المُقيّم
            int evaluatorId = request.EvaluatorId ?? 0;
            if (evaluatorId == 0)
            {
                var emp = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken);
                if (emp == null) return Result<int>.Failure("الموظف غير موجود");
                if (emp.ManagerId.HasValue) evaluatorId = emp.ManagerId.Value;
                else return Result<int>.Failure("لم يتم تعيين مدير مباشر للموظف، يرجى تحديد المُقيّم يدوياً.");
            }

            // إنشاء التقييم الرئيسي
            var appraisal = new EmployeeAppraisal
            {
                EmployeeId = request.EmployeeId,
                CycleId = request.CycleId,
                EvaluatorId = evaluatorId,
                Status = "SELF_EVALUATION",
                AppraisalDate = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.EmployeeAppraisals.Add(appraisal);
            await _context.SaveChangesAsync(cancellationToken);

            // ✅ FIX #3A: جلب بيانات الحضور خلال فترة الدورة
            var attendanceSummary = await _context.DailyAttendances
                .Where(a => a.EmployeeId == request.EmployeeId
                         && a.AttendanceDate >= cycle.StartDate
                         && a.AttendanceDate <= cycle.EndDate
                         && a.IsDeleted == 0)
                .GroupBy(a => a.EmployeeId)
                .Select(g => new
                {
                    AbsentDays = g.Count(a => a.Status == "ABSENT"),
                    LateDays = g.Count(a => a.LateMinutes > 0),
                    TotalLateMinutes = g.Sum(a => (int)a.LateMinutes),
                    OvertimeMinutes = g.Sum(a => (int)a.OvertimeMinutes),
                    TotalWorkingDays = g.Count()
                })
                .FirstOrDefaultAsync(cancellationToken);

            // ✅ FIX #3B: جلب عدد المخالفات المعتمدة خلال فترة الدورة
            var violationCount = await _context.EmployeeViolations
                .CountAsync(v => v.EmployeeId == request.EmployeeId
                              && v.ViolationDate >= cycle.StartDate
                              && v.ViolationDate <= cycle.EndDate
                              && v.Status == "APPROVED"
                              && v.IsDeleted == 0, cancellationToken);

            // حساب نقاط الحضور المبدئية (تنازلي: كل يوم غياب = -5 نقاط من 100)
            decimal attendanceBaseScore = 100;
            if (attendanceSummary != null && attendanceSummary.TotalWorkingDays > 0)
            {
                int totalDays = attendanceSummary.TotalWorkingDays;
                attendanceBaseScore -= (attendanceSummary.AbsentDays * 5m);
                attendanceBaseScore -= (attendanceSummary.LateDays * 1m);
                attendanceBaseScore = Math.Max(0, Math.Min(100, attendanceBaseScore));
            }

            // حساب نقاط المخالفات (كل مخالفة = -10 نقاط)
            decimal violationsBaseScore = Math.Max(0, 100 - (violationCount * 10m));

            // جلب الـ KPIs لإنشاء تفاصيل التقييم
            var kpis = await _context.KpiLibraries
                .Where(k => k.IsDeleted == 0)
                .ToListAsync(cancellationToken);

            // ✅ FIX: ملء ActualValue بناءً على نوع الـ KPI (category)
            var details = kpis.Select(k =>
            {
                decimal actualValue = 0;
                decimal targetValue = 100;

                // تعبئة القيم الفعلية بناءً على تصنيف الـ KPI
                var category = k.Category?.ToUpperInvariant() ?? "";
                if (category.Contains("ATTEND") || category.Contains("حضور"))
                {
                    actualValue = attendanceBaseScore;
                }
                else if (category.Contains("DISCIPLIN") || category.Contains("مخالف") || category.Contains("انضباط"))
                {
                    actualValue = violationsBaseScore;
                }
                else if (category.Contains("OVERTIME") || category.Contains("إضافي"))
                {
                    // ساعات إضافي > 10 ساعة = 100 نقطة
                    var otHours = (attendanceSummary?.OvertimeMinutes ?? 0) / 60.0m;
                    actualValue = Math.Min(100, otHours * 10m);
                }
                // باقي الـ KPIs تبقى 0 (يُدخلها المدير يدوياً)

                return new AppraisalDetail
                {
                    AppraisalId = appraisal.AppraisalId,
                    KpiId = k.KpiId,
                    TargetValue = targetValue,
                    ActualValue = actualValue,
                    EmployeeScore = 0,   // يُدخله الموظف
                    ManagerScore = 0,    // يُدخله المدير
                    FinalScore = 0,      // يُدخله الـ HR
                    CreatedBy = _currentUserService.UserId,
                    CreatedAt = DateTime.UtcNow
                };
            }).ToList();

            _context.AppraisalDetails.AddRange(details);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return Result<int>.Success(appraisal.AppraisalId,
                $"تم تهيئة التقييم بنجاح. غياب: {attendanceSummary?.AbsentDays ?? 0} يوم | مخالفات: {violationCount}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<int>.Failure($"خطأ داخلي: {ex.Message}");
        }
    }
}
