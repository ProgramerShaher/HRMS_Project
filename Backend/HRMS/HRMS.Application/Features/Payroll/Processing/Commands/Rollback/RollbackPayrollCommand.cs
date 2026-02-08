using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Processing.Commands.Rollback;

/// <summary>
/// Rollback Payroll Run Command
/// </summary>
public class RollbackPayrollCommand : IRequest<Result<bool>>
{
    public int RunId { get; set; }
}

/// <summary>
/// Rollback Payroll Command Handler
/// </summary>
public class RollbackPayrollCommandHandler : IRequestHandler<RollbackPayrollCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RollbackPayrollCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(RollbackPayrollCommand request, CancellationToken cancellationToken)
    {
        // جلب مسير الرواتب
        var payrollRun = await _context.PayrollRuns
            .Include(pr => pr.Payslips)
                .ThenInclude(p => p.Details)
            .FirstOrDefaultAsync(pr => pr.RunId == request.RunId, cancellationToken);

        if (payrollRun == null)
            return Result<bool>.Failure("مسير الرواتب غير موجود");

        // Business Rule: لا يمكن التراجع عن مسير رواتب تم ترحيله للحسابات
        if (payrollRun.Status == "POSTED")
            return Result<bool>.Failure("لا يمكن التراجع عن مسير رواتب تم ترحيله إلى دليل الحسابات");

        // استخدام معاملة ذرية لضمان تنفيذ جميع العمليات أو إلغائها بالكامل
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. جلب جميع الأقساط المرتبطة بهذا المسير
            var linkedInstallments = await _context.LoanInstallments
                .Where(i => i.PaidInPayrollRun == request.RunId)
                .ToListAsync(cancellationToken);

            // 2. إعادة فتح الأقساط (تحويلها لحالة غير مدفوعة)
            foreach (var installment in linkedInstallments)
            {
                installment.Status = "UNPAID";
                installment.PaidDate = null;
                installment.PaidInPayrollRun = null;
                installment.IsPaid = 0; // للتوافق مع الحقل القديم
                installment.UpdatedBy = _currentUserService.UserId;
                installment.UpdatedAt = DateTime.UtcNow;
            }

            // 3. حذف تفاصيل قسائم الرواتب
            var payslipDetailIds = payrollRun.Payslips
                .SelectMany(p => p.Details.Select(d => d.DetailId))
                .ToList();

            if (payslipDetailIds.Any())
            {
                var detailsToDelete = await _context.PayslipDetails
					.Where(d => payslipDetailIds.Contains(d.DetailId))
                    .ToListAsync(cancellationToken);

                _context.PayslipDetails.RemoveRange(detailsToDelete);
            }

            // 4. حذف قسائم الرواتب
            var payslipIds = payrollRun.Payslips.Select(p => p.PayslipId).ToList();

            if (payslipIds.Any())
            {
                var payslipsToDelete = await _context.Payslips
                    .Where(p => payslipIds.Contains(p.PayslipId))
                    .ToListAsync(cancellationToken);

                _context.Payslips.RemoveRange(payslipsToDelete);
            }

            // 5. حذف مسير الرواتب نفسه
            _context.PayrollRuns.Remove(payrollRun);

            // حفظ التغييرات
            await _context.SaveChangesAsync(cancellationToken);

            // تأكيد المعاملة
            await transaction.CommitAsync(cancellationToken);

            return Result<bool>.Success(true, 
                $"تم التراجع عن مسير الرواتب بنجاح. تم حذف {payslipIds.Count} قسيمة راتب وإعادة فتح {linkedInstallments.Count} قسط");
        }
        catch (Exception ex)
        {
            // إلغاء المعاملة في حالة حدوث خطأ
            await transaction.RollbackAsync(cancellationToken);
            return Result<bool>.Failure($"فشل التراجع عن مسير الرواتب: {ex.Message}");
        }
    }
}
