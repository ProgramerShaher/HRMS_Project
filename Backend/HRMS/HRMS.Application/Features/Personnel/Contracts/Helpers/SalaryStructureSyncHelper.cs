using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Payroll;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Contracts.Helpers;

/// <summary>
/// خدمة مساعدة لمزامنة عناصر الراتب مع هيكل رواتب الموظفين
/// </summary>
/// <remarks>
/// تضمن هذه الخدمة أن جميع مكونات العقد (الراتب الأساسي والبدلات) 
/// يتم تحديثها تلقائياً في جدول EMPLOYEE_SALARY_STRUCTURE
/// </remarks>
public static class SalaryStructureSyncHelper
{
    /// <summary>
    /// مزامنة عنصر راتب واحد لموظف معين
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات</param>
    /// <param name="employeeId">معرف الموظف</param>
    /// <param name="elementKeyword">الكلمة المفتاحية للبحث عن العنصر (مثل: "سكن", "مواصلات")</param>
    /// <param name="amount">المبلغ المراد تعيينه</param>
    /// <param name="isBasicSalary">هل هذا عنصر الراتب الأساسي؟</param>
    /// <param name="elementNameAr">اسم العنصر بالعربية (للإنشاء التلقائي)</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    public static async Task SyncSalaryElementAsync(
        IApplicationDbContext context,
        int employeeId,
        string elementKeyword,
        decimal amount,
        bool isBasicSalary = false,
        string? elementNameAr = null,
        CancellationToken cancellationToken = default)
    {
        // Skip if amount is zero or negative
        if (amount <= 0)
            return;

        // ═══════════════════════════════════════════════════════════
        // STEP 1: Find or Create the Salary Element
        // ═══════════════════════════════════════════════════════════
        SalaryElement? salaryElement;

        if (isBasicSalary)
        {
            // For Basic Salary, find by IsBasic flag
            salaryElement = await context.SalaryElements
                .FirstOrDefaultAsync(e => e.IsBasic == 1, cancellationToken);
        }
        else
        {
            // For allowances, find by name keyword (Arabic or English)
            salaryElement = await context.SalaryElements
                .FirstOrDefaultAsync(
                    e => e.ElementNameAr.Contains(elementKeyword),
                    cancellationToken
                );
        }

        // ✅ Auto-create missing element if not found
        if (salaryElement == null && !string.IsNullOrEmpty(elementNameAr))
        {
            salaryElement = new SalaryElement
            {
                ElementNameAr = elementNameAr,
                ElementType = "EARNING",
                IsTaxable = 0,
                IsGosiBase = 0,
                IsRecurring = 1,
                IsBasic = (byte)(isBasicSalary ? 1 : 0)
            };
            context.SalaryElements.Add(salaryElement);
            await context.SaveChangesAsync(cancellationToken); // Save to get ElementId
        }

        // If still not found, skip
        if (salaryElement == null)
            return;

        // ═══════════════════════════════════════════════════════════
        // STEP 2: Upsert Employee Salary Structure
        // ═══════════════════════════════════════════════════════════
        var existingStructure = await context.SalaryStructures
            .FirstOrDefaultAsync(
                s => s.EmployeeId == employeeId && s.ElementId == salaryElement.ElementId,
                cancellationToken
            );

        if (existingStructure == null)
        {
            // INSERT: Create new salary structure entry
            var newStructure = new EmployeeSalaryStructure
            {
                EmployeeId = employeeId,
                ElementId = salaryElement.ElementId,
                Amount = amount,
                Percentage = 0,
                IsActive = 1
            };
            context.SalaryStructures.Add(newStructure);
        }
        else
        {
            // UPDATE: Modify existing entry
            existingStructure.Amount = amount;
            existingStructure.IsActive = 1;
        }
    }

    /// <summary>
    /// مزامنة جميع مكونات العقد مع هيكل الراتب
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات</param>
    /// <param name="employeeId">معرف الموظف</param>
    /// <param name="basicSalary">الراتب الأساسي</param>
    /// <param name="housingAllowance">بدل السكن</param>
    /// <param name="transportAllowance">بدل المواصلات</param>
    /// <param name="medicalAllowance">البدل الطبي</param>
    /// <param name="otherAllowances">البدلات الأخرى</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    public static async Task SyncAllContractComponentsAsync(
        IApplicationDbContext context,
        int employeeId,
        decimal basicSalary,
        decimal housingAllowance,
        decimal transportAllowance,
        decimal medicalAllowance,
        decimal otherAllowances,
        CancellationToken cancellationToken = default)
    {
        // Sync Basic Salary (using IsBasic flag)
        await SyncSalaryElementAsync(
            context, 
            employeeId, 
            string.Empty, // Not used for basic salary
            basicSalary, 
            isBasicSalary: true,
            elementNameAr: "الراتب الأساسي",
            cancellationToken
        );

        // Sync Housing Allowance
        await SyncSalaryElementAsync(
            context, 
            employeeId, 
            "سكن", // Arabic keyword for Housing
            housingAllowance, 
            isBasicSalary: false,
            elementNameAr: "بدل سكن",
            cancellationToken
        );

        // Sync Transport Allowance
        await SyncSalaryElementAsync(
            context, 
            employeeId, 
            "مواصلات", // Arabic keyword for Transport
            transportAllowance, 
            isBasicSalary: false,
            elementNameAr: "بدل مواصلات",
            cancellationToken
        );

        // Sync Medical Allowance
        await SyncSalaryElementAsync(
            context, 
            employeeId, 
            "طبي", // Arabic keyword for Medical
            medicalAllowance, 
            isBasicSalary: false,
            elementNameAr: "بدل طبي",
            cancellationToken
        );

        // Sync Other Allowances
        await SyncSalaryElementAsync(
            context, 
            employeeId, 
            "أخرى", // Arabic keyword for Other
            otherAllowances, 
            isBasicSalary: false,
            elementNameAr: "بدلات أخرى",
            cancellationToken
        );
    }
}
