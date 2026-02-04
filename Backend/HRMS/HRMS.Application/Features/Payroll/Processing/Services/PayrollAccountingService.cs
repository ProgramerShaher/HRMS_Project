using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Accounting;
using HRMS.Core.Entities.Payroll;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HRMS.Application.Features.Payroll.Processing.Services;

/// <summary>
/// خدمة ترحيل الرواتب إلى دليل الحسابات
/// Payroll to General Ledger Posting Service
/// </summary>
public class PayrollAccountingService
{
    private readonly IApplicationDbContext _context;

    public PayrollAccountingService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// ترحيل مسير الرواتب إلى دليل الحسابات
    /// Post Payroll Run to General Ledger
    /// </summary>
    /// <param name="runId">معرف مسير الرواتب</param>
    /// <returns>معرف قيد اليومية المُنشأ</returns>
    public async Task<long> PostPayrollToGLAsync(int runId)
    {
        // 1. التحقق من وجود المسير وحالته
        var payrollRun = await _context.PayrollRuns
            .Include(r => r.Payslips)
            .FirstOrDefaultAsync(r => r.RunId == runId);

        if (payrollRun == null)
            throw new Exception($"Payroll Run {runId} not found.");

        if (payrollRun.Status != "APPROVED")
            throw new Exception($"Payroll Run must be APPROVED before posting. Current status: {payrollRun.Status}");

        if (payrollRun.Status == "POSTED")
            throw new Exception($"Payroll Run {runId} is already posted to GL.");

        // 2. حساب الإجماليات من قسائم الرواتب
        var totalGross = payrollRun.Payslips.Sum(p => 
            (p.BasicSalary ?? 0) + (p.TotalAllowances ?? 0) + (p.OvertimeEarnings));

        var totalNet = payrollRun.Payslips.Sum(p => p.NetSalary ?? 0);

        var totalDeductions = payrollRun.Payslips.Sum(p => p.TotalDeductions ?? 0);

        // 3. التحقق من التوازن (إجمالي الإجمالي = صافي + استقطاعات)
        if (Math.Abs(totalGross - (totalNet + totalDeductions)) > 0.01m)
        {
            throw new Exception($"Payroll totals are not balanced. Gross: {totalGross}, Net: {totalNet}, Deductions: {totalDeductions}");
        }

        // 4. جلب الحسابات من دليل الحسابات
        var salariesExpenseAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountCode == "5100" && a.IsActive);

        var salariesPayableAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountCode == "2100" && a.IsActive);

        var penaltiesIncomeAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountCode == "4200" && a.IsActive);

        if (salariesExpenseAccount == null || salariesPayableAccount == null || penaltiesIncomeAccount == null)
        {
            throw new Exception("Required GL accounts not found. Please configure accounts: 5100 (Salaries Expense), 2100 (Salaries Payable), 4200 (Penalties/Other Income)");
        }

        // 5. بدء Transaction لضمان الذرية (Atomicity)
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 6. إنشاء قيد اليومية
            var journalEntry = new JournalEntry
            {
                EntryDate = DateTime.Now,
                Description = $"Payroll Posting for Month {payrollRun.Month:D2}/{payrollRun.Year}",
                SourceModule = "PAYROLL",
                SourceReferenceId = runId,
                Status = "POSTED",
                TotalDebit = totalGross,
                TotalCredit = totalGross, // يجب أن يكون متوازن
                PostedDate = DateTime.Now
            };

            _context.JournalEntries.Add(journalEntry);
            await _context.SaveChangesAsync();

            // 7. إضافة سطور القيد
            var lines = new List<JournalEntryLine>
            {
                // سطر 1: مدين - مصروف الرواتب
                new JournalEntryLine
                {
                    JournalEntryId = journalEntry.JournalEntryId,
                    AccountId = salariesExpenseAccount.AccountId,
                    DebitAmount = totalGross,
                    CreditAmount = 0,
                    Description = $"Salaries Expense for {payrollRun.Month:D2}/{payrollRun.Year}",
                    LineNumber = 1
                },
                // سطر 2: دائن - رواتب مستحقة
                new JournalEntryLine
                {
                    JournalEntryId = journalEntry.JournalEntryId,
                    AccountId = salariesPayableAccount.AccountId,
                    DebitAmount = 0,
                    CreditAmount = totalNet,
                    Description = $"Salaries Payable for {payrollRun.Month:D2}/{payrollRun.Year}",
                    LineNumber = 2
                },
                // سطر 3: دائن - استقطاعات/غرامات
                new JournalEntryLine
                {
                    JournalEntryId = journalEntry.JournalEntryId,
                    AccountId = penaltiesIncomeAccount.AccountId,
                    DebitAmount = 0,
                    CreditAmount = totalDeductions,
                    Description = $"Deductions/Penalties for {payrollRun.Month:D2}/{payrollRun.Year}",
                    LineNumber = 3
                }
            };

            foreach (var line in lines)
            {
                _context.JournalEntryLines.Add(line);
            }

            await _context.SaveChangesAsync();

            // 8. تحديث حالة مسير الرواتب
            payrollRun.Status = "POSTED";
            await _context.SaveChangesAsync();

            // 9. Commit Transaction
            await transaction.CommitAsync();

            return journalEntry.JournalEntryId;
        }
        catch (Exception ex)
        {
            // 10. Rollback في حالة الخطأ
            await transaction.RollbackAsync();
            throw new Exception($"Failed to post payroll to GL: {ex.Message}", ex);
        }
    }
}
