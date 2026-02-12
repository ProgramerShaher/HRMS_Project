using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Exceptions;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.Balances.Commands.InitializeBalances;

/// <summary>
/// Handler for bulk leave balance initialization.
/// Implements dynamic entitlement based on leave configuration with duplicate prevention and proration support.
/// </summary>
public class InitializeBalancesCommandHandler
    : IRequestHandler<InitializeBalancesCommand, Result<InitializeBalancesResultDto>>
{
    private readonly IApplicationDbContext _context;

    public InitializeBalancesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<InitializeBalancesResultDto>> Handle(
        InitializeBalancesCommand request, 
        CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // المرحلة 1: جلب إعدادات نوع الإجازة
        // Phase 1: Fetch leave type configuration
        // ═══════════════════════════════════════════════════════════

        var leaveType = await _context.LeaveTypes
            .AsNoTracking()
            .Where(lt => lt.LeaveTypeId == request.LeaveTypeId && lt.IsDeleted == 0)
            .FirstOrDefaultAsync(cancellationToken);

        if (leaveType == null)
            throw new NotFoundException(
                $"نوع الإجازة برقم {request.LeaveTypeId} غير موجود");

        // ═══════════════════════════════════════════════════════════
        // المرحلة 2: تحديد عدد الأيام المستحقة
        // Phase 2: Determine entitlement days
        // ═══════════════════════════════════════════════════════════

        // استخدام الأيام المخصصة من الطلب أو الأيام الافتراضية من نوع الإجازة
        // Use custom days from request or default days from leave type
        decimal standardDaysGranted = request.CustomDays ?? leaveType.DefaultDays;

        // ═══════════════════════════════════════════════════════════
        // المرحلة 3: تصفية الموظفين المستهدفين
        // Phase 3: Filter target employees
        // ═══════════════════════════════════════════════════════════

        // جلب الموظفين النشطين فقط
        // Fetch only active employees
        var employeesQuery = _context.Employees
            .AsNoTracking()
            .Where(e => e.IsDeleted == 0);

        // تطبيق فلتر القسم إذا تم تحديده
        // Apply department filter if specified
        if (request.DepartmentId.HasValue)
        {
            // تصفية الموظفين حسب القسم المحدد
            // Filter employees by specified department
            employeesQuery = employeesQuery.Where(e => e.DepartmentId == request.DepartmentId.Value);
        }

        var targetEmployees = await employeesQuery
            .Select(e => new
            {
                e.EmployeeId,
                // Fetch name parts to construct full name client-side using standard logic if needed, 
                // but since we can't use [NotMapped] in LINQ, we map to parts or use the parts in memory.
                // However, since we are returning an anonymous object, let's just fetch the whole Employee needed or parts.
                e.FirstNameAr, e.SecondNameAr, e.ThirdNameAr, e.LastNameAr,
                e.HireDate
            })
            .ToListAsync(cancellationToken);
            
            // Map to a stricter structure if needed, or just use the parts in loop.
            // We will modify the usage in the loop.

        // ═══════════════════════════════════════════════════════════
        // المرحلة 4: التحقق من الأرصدة الموجودة مسبقاً
        // Phase 4: Check for existing balances (duplicate prevention)
        // ═══════════════════════════════════════════════════════════

        // جلب معرفات الموظفين الذين لديهم رصيد بالفعل لهذا النوع والسنة
        // Fetch employee IDs who already have balance for this type/year
        var existingBalanceEmployeeIds = await _context.LeaveBalances
            .Where(b => b.LeaveTypeId == request.LeaveTypeId 
                     && b.Year == request.Year
                     && b.IsDeleted == 0)
            .Select(b => b.EmployeeId)
            .ToListAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════
        // المرحلة 5: إنشاء الأرصدة الجديدة
        // Phase 5: Create new balances
        // ═══════════════════════════════════════════════════════════

        var result = new InitializeBalancesResultDto
        {
            LeaveTypeName = leaveType.LeaveNameAr,
            StandardDaysGranted = standardDaysGranted,
            TotalEmployeesProcessed = targetEmployees.Count()
        };

        var newBalances = new List<EmployeeLeaveBalance>();

        foreach (var employee in targetEmployees)
        {
            // Construct FullNameAr client-side (Single Source of Logic replicated or helper used)
            var fullNameAr = string.Join(" ", new[] { employee.FirstNameAr, employee.SecondNameAr, employee.ThirdNameAr, employee.LastNameAr }
                                    .Where(s => !string.IsNullOrWhiteSpace(s)));

            // تخطي الموظفين الذين لديهم رصيد بالفعل
            // Skip employees who already have a balance
            if (existingBalanceEmployeeIds.Contains(employee.EmployeeId))
            {
                result.BalancesSkipped++;
                result.Warnings.Add($"تم تخطي الموظف {fullNameAr} - يوجد رصيد مسبقاً");
                continue;
            }

            // حساب الأيام النهائية (مع التناسب إذا كان مفعلاً)
            // Calculate final days (with proration if enabled)
            decimal finalDays = standardDaysGranted;

            if (request.EnableProration && employee.HireDate.Year == request.Year)
            {
                // حساب التناسب للموظفين المعينين خلال السنة المستهدفة
                // Calculate proration for employees hired during target year
                
                // حساب عدد الأشهر المتبقية من السنة (بما في ذلك شهر التعيين)
                // Calculate remaining months in the year (including hire month)
                int monthsWorked = 12 - employee.HireDate.Month + 1;
                
                // حساب الأيام المتناسبة: (الأيام الكاملة / 12 شهر) × عدد الأشهر المعمولة
                // Calculate prorated days: (Full days / 12 months) × Months worked
                finalDays = Math.Round((standardDaysGranted / 12) * monthsWorked, 2);
                
                result.Warnings.Add(
                    $"تم تطبيق التناسب للموظف {fullNameAr}: " +
                    $"{finalDays} أيام بدلاً من {standardDaysGranted} (عمل {monthsWorked} شهر)");
            }

            // إنشاء رصيد جديد للموظف
            // Create new balance for employee
            var balance = new EmployeeLeaveBalance
            {
                EmployeeId = employee.EmployeeId,
                LeaveTypeId = request.LeaveTypeId,
                Year = request.Year,
                CurrentBalance = finalDays
            };

            newBalances.Add(balance);
            result.BalancesCreated++;
        }

        // ═══════════════════════════════════════════════════════════
        // المرحلة 6: حفظ الأرصدة في قاعدة البيانات
        // Phase 6: Save balances to database
        // ═══════════════════════════════════════════════════════════

        if (newBalances.Any())
        {
            _context.LeaveBalances.AddRange(newBalances);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // ═══════════════════════════════════════════════════════════
        // المرحلة 7: إرجاع النتيجة
        // Phase 7: Return result
        // ═══════════════════════════════════════════════════════════

        var successMessage = $"تم تهيئة {result.BalancesCreated} رصيد إجازة بنجاح. " +
                           $"تم تخطي {result.BalancesSkipped} موظف (لديهم رصيد مسبقاً).";

        return Result<InitializeBalancesResultDto>.Success(result, successMessage);
    }
}
