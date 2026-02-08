using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Loans.Services;

/// <summary>
/// خدمة توليد الأقساط التلقائية للقروض
/// Installment Generation Service
/// </summary>
public class GenerateInstallmentsService
{
    private readonly IApplicationDbContext _context;

    public GenerateInstallmentsService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// توليد جدول الأقساط للقرض
    /// Generate installment schedule for a loan
    /// </summary>
    public async Task<Result<bool>> GenerateInstallmentsAsync(int loanId, CancellationToken cancellationToken = default)
    {
        // جلب بيانات القرض
        var loan = await _context.Loans
            .Include(l => l.Installments)
            .FirstOrDefaultAsync(l => l.LoanId == loanId, cancellationToken);

        if (loan == null)
            return Result<bool>.Failure("القرض غير موجود");

        // حذف الأقساط القديمة إن وجدت (في حالة إعادة التوليد)
        if (loan.Installments.Any())
        {
            _context.LoanInstallments.RemoveRange(loan.Installments);
        }

        // حساب قيمة القسط الشهري
        decimal monthlyInstallment = Math.Round(loan.LoanAmount / loan.InstallmentCount, 2);

        // تعديل القسط الأخير لضمان المطابقة التامة للمبلغ (بسبب التقريب)
        decimal lastInstallmentAmount = loan.LoanAmount - (monthlyInstallment * (loan.InstallmentCount - 1));

        // توليد الأقساط
        var installments = new List<LoanInstallment>();
        DateTime currentDueDate = loan.ApprovalDate ?? DateTime.Today;

        for (short i = 1; i <= loan.InstallmentCount; i++)
        {
            // حساب تاريخ الاستحقاق (شهرياً)
            currentDueDate = currentDueDate.AddMonths(1);

            var installment = new LoanInstallment
            {
                LoanId = loanId,
                InstallmentNumber = i,
                DueDate = currentDueDate,
                Amount = i == loan.InstallmentCount ? lastInstallmentAmount : monthlyInstallment,
                Status = "UNPAID",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = loan.CreatedBy
            };

            installments.Add(installment);
        }

        // إضافة الأقساط إلى قاعدة البيانات
        await _context.LoanInstallments.AddRangeAsync(installments, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, $"تم توليد {loan.InstallmentCount} قسط بنجاح");
    }
}
