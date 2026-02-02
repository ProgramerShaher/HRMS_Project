using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.LeaveBalances.Commands.InitializeBalances;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// أمر تهيئة أرصدة الإجازات الذكي
/// Smart Leave Balance Initialization Command
/// </summary>
public record InitializeBalancesCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف نوع الإجازة (اختياري - إذا كان null يتم تهيئة جميع الأنواع النشطة)
    /// Leave Type ID (optional - if null, initialize ALL active leave types)
    /// </summary>
    public int? LeaveTypeId { get; init; }

    /// <summary>
    /// السنة المستهدفة للأرصدة
    /// Target year for the balances
    /// </summary>
    public short Year { get; init; }

    /// <summary>
    /// معرف القسم (اختياري - لتصفية الموظفين حسب القسم)
    /// Department ID (optional - to filter employees by department)
    /// </summary>
    public int? DepartmentId { get; init; }

    /// <summary>
    /// تفعيل التوزيع النسبي للموظفين المعينين في منتصف السنة
    /// Enable proration for mid-year hires
    /// </summary>
    public bool EnableProration { get; init; }

    /// <summary>
    /// عدد الأيام المخصص (اختياري - يتجاوز القيمة الافتراضية من نوع الإجازة)
    /// Custom days (optional - overrides default days from leave type)
    /// </summary>
    public short? CustomDays { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// مدقق أمر تهيئة الأرصدة
/// Validator for InitializeBalancesCommand
/// </summary>
public class InitializeBalancesCommandValidator : AbstractValidator<InitializeBalancesCommand>
{
    public InitializeBalancesCommandValidator()
    {
        // التحقق من السنة
        // Validate year
        RuleFor(x => x.Year)
            .InclusiveBetween((short)2000, (short)2100)
            .WithMessage("السنة يجب أن تكون بين 2000 و 2100");

        // التحقق من معرف نوع الإجازة (إذا تم توفيره)
        // Validate leave type ID (if provided)
        RuleFor(x => x.LeaveTypeId)
            .GreaterThan(0)
            .When(x => x.LeaveTypeId.HasValue)
            .WithMessage("معرف نوع الإجازة غير صحيح");

        // التحقق من معرف القسم (إذا تم توفيره)
        // Validate department ID (if provided)
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .When(x => x.DepartmentId.HasValue)
            .WithMessage("معرف القسم غير صحيح");

        // التحقق من عدد الأيام المخصص (إذا تم توفيره)
        // Validate custom days (if provided)
        RuleFor(x => x.CustomDays)
            .GreaterThan((short)0)
            .LessThanOrEqualTo((short)365)
            .When(x => x.CustomDays.HasValue)
            .WithMessage("عدد الأيام المخصص يجب أن يكون بين 1 و 365");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// معالج أمر تهيئة الأرصدة الذكي
/// Handler for Smart Leave Balance Initialization
/// </summary>
public class InitializeBalancesCommandHandler : IRequestHandler<InitializeBalancesCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public InitializeBalancesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(InitializeBalancesCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: جلب أنواع الإجازات المستهدفة
        // Step 1: Fetch target leave types
        // ═══════════════════════════════════════════════════════════════════════════

        var leaveTypesQuery = _context.LeaveTypes
            .Where(lt => lt.IsDeleted == 0 && lt.IsDeductible == 1);

        // إذا تم تحديد نوع إجازة معين، نجلبه فقط
        // If a specific leave type is specified, fetch only that one
        if (request.LeaveTypeId.HasValue)
        {
            leaveTypesQuery = leaveTypesQuery.Where(lt => lt.LeaveTypeId == request.LeaveTypeId.Value);
        }

        var leaveTypes = await leaveTypesQuery.ToListAsync(cancellationToken);

        if (!leaveTypes.Any())
        {
            return Result<bool>.Failure(
                request.LeaveTypeId.HasValue
                    ? "نوع الإجازة المحدد غير موجود أو غير قابل للخصم"
                    : "لا توجد أنواع إجازات نشطة قابلة للخصم",
                404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: جلب الموظفين المستهدفين
        // Step 2: Fetch target employees
        // ═══════════════════════════════════════════════════════════════════════════

        var employeesQuery = _context.Employees
            .Where(e => e.IsDeleted == 0);

        // تصفية حسب القسم إذا تم تحديده
        // Filter by department if specified
        if (request.DepartmentId.HasValue)
        {
            employeesQuery = employeesQuery.Where(e => e.DepartmentId == request.DepartmentId.Value);
        }

        var employees = await employeesQuery
            .Select(e => new
            {
                e.EmployeeId,
                e.HireDate
            })
            .ToListAsync(cancellationToken);

        if (!employees.Any())
        {
            return Result<bool>.Failure(
                request.DepartmentId.HasValue
                    ? "لا يوجد موظفون نشطون في القسم المحدد"
                    : "لا يوجد موظفون نشطون",
                404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: تهيئة الأرصدة مع تطبيق منطق التوزيع النسبي
        // Step 3: Initialize balances with proration logic
        // ═══════════════════════════════════════════════════════════════════════════

        int createdCount = 0;
        int updatedCount = 0;
        int skippedCount = 0;

        foreach (var employee in employees)
        {
            foreach (var leaveType in leaveTypes)
            {
                // حساب الرصيد الأولي
                // Calculate initial balance
                decimal initialBalance = request.CustomDays ?? leaveType.DefaultDays;

                // تطبيق التوزيع النسبي إذا كان مفعلاً
                // Apply proration if enabled
                if (request.EnableProration)
                {
                    var hireDate = employee.HireDate;

                    // التحقق من أن تاريخ التعيين في السنة المستهدفة
                    // Check if hire date is within the target year
                    if (hireDate.Year == request.Year)
                    {
                        // حساب عدد الأشهر المتبقية من السنة (شامل شهر التعيين)
                        // Calculate remaining months in the year (including hire month)
                        int remainingMonths = 12 - hireDate.Month + 1;

                        // تطبيق معادلة التوزيع النسبي
                        // Apply proration formula: Balance = (DefaultDays / 12) * RemainingMonths
                        initialBalance = Math.Round((initialBalance / 12m) * remainingMonths, 2);
                    }
                }

                // ═══════════════════════════════════════════════════════════════════════════
                // الخطوة 4: استراتيجية Upsert (تحديث أو إدراج)
                // Step 4: Upsert Strategy (Update or Insert)
                // ═══════════════════════════════════════════════════════════════════════════

                // البحث عن رصيد موجود
                // Check if balance record exists
                var existingBalance = await _context.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(b =>
                        b.EmployeeId == employee.EmployeeId
                        && b.LeaveTypeId == leaveType.LeaveTypeId
                        && b.Year == request.Year
                        && b.IsDeleted == 0,
                        cancellationToken);

                if (existingBalance != null)
                {
                    // تحديث الرصيد الموجود
                    // Update existing balance
                    existingBalance.CurrentBalance = (short)initialBalance;
                    updatedCount++;
                }
                else
                {
                    // إنشاء رصيد جديد
                    // Create new balance
                    var newBalance = new EmployeeLeaveBalance
                    {
                        EmployeeId = employee.EmployeeId,
                        LeaveTypeId = leaveType.LeaveTypeId,
                        Year = request.Year,
                        CurrentBalance = (short)initialBalance
                    };

                    _context.EmployeeLeaveBalances.Add(newBalance);
                    createdCount++;
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 5: حفظ التغييرات
        // Step 5: Save changes
        // ═══════════════════════════════════════════════════════════════════════════

        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 6: إرجاع النتيجة بنجاح
        // Step 6: Return success result
        // ═══════════════════════════════════════════════════════════════════════════

        var prorationNote = request.EnableProration
            ? " (مع تطبيق التوزيع النسبي للمعينين في منتصف السنة)"
            : "";

        var message = $"تمت تهيئة الأرصدة بنجاح للسنة {request.Year}{prorationNote}. " +
                      $"تم إنشاء {createdCount} رصيد جديد، وتحديث {updatedCount} رصيد موجود.";

        return Result<bool>.Success(true, message);
    }
}
