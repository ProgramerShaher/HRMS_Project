using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.LeaveBalances.Commands.InitializeYearlyBalance;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Command to initialize yearly leave balances for all employees.
/// Creates balance records for all deductible leave types.
/// </summary>
public record InitializeYearlyBalanceCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// السنة المراد تهيئة الأرصدة لها
    /// Year to initialize balances for
    /// </summary>
    public short Year { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Validator for InitializeYearlyBalanceCommand.
/// Ensures valid year is provided.
/// </summary>
public class InitializeYearlyBalanceCommandValidator : AbstractValidator<InitializeYearlyBalanceCommand>
{
    public InitializeYearlyBalanceCommandValidator()
    {
        // التحقق من السنة
        // Validate year
        RuleFor(x => x.Year)
            .InclusiveBetween((short)2000, (short)2100).WithMessage("السنة يجب أن تكون بين 2000 و 2100");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for initializing yearly leave balances.
/// Creates balance records for all active employees and deductible leave types.
/// </summary>
public class InitializeYearlyBalanceCommandHandler : IRequestHandler<InitializeYearlyBalanceCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public InitializeYearlyBalanceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(InitializeYearlyBalanceCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: الحصول على جميع الموظفين النشطين
        // Step 1: Get all active employees
        // ═══════════════════════════════════════════════════════════════════════════

        var employees = await _context.Employees
            .Where(e => e.IsDeleted == 0) // Only active employees
            .Select(e => e.EmployeeId)
            .ToListAsync(cancellationToken);

        if (!employees.Any())
        {
            return Result<bool>.Failure("لا يوجد موظفون نشطون لتهيئة أرصدتهم", 404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: الحصول على أنواع الإجازات القابلة للخصم
        // Step 2: Get all deductible leave types
        // جلب أنواع الإجازات التي تخصم من الرصيد فقط
        // Get deductible leave types only
        var leaveTypes = await _context.LeaveTypes
            .Where(lt => lt.IsDeductible == 1 && lt.IsDeleted == 0)
            .ToListAsync(cancellationToken);

        if (!leaveTypes.Any())
        {
            return Result<bool>.Failure("لا توجد أنواع إجازات قابلة للخصم لتهيئة الأرصدة", 404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: إنشاء الأرصدة للموظفين
        // Step 3: Create balances for employees
        // ═══════════════════════════════════════════════════════════════════════════

        int createdCount = 0;

        foreach (var empId in employees)
        {
            foreach (var type in leaveTypes)
            {
                // نتحقق من عدم وجود رصيد مسبقاً
                // Why: لتجنب التكرار والتضارب
                var exists = await _context.EmployeeLeaveBalances.AnyAsync(b => 
                    b.EmployeeId == empId 
                    && b.LeaveTypeId == type.LeaveTypeId 
                    && b.Year == request.Year
                    && b.IsDeleted == 0, 
                    cancellationToken);

                if (!exists)
                {
                    var balance = new EmployeeLeaveBalance
                    {
                        EmployeeId = empId,
                        LeaveTypeId = type.LeaveTypeId,
                        Year = request.Year,
                        CurrentBalance = type.DefaultDays // نبدأ بالرصيد الافتراضي
                    };
                    _context.EmployeeLeaveBalances.Add(balance);
                    createdCount++;
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 4: حفظ التغييرات
        // Step 4: Save changes
        // ═══════════════════════════════════════════════════════════════════════════

        if (createdCount > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 5: إرجاع النتيجة
        // Step 5: Return result
        // ═══════════════════════════════════════════════════════════════════════════

        var message = createdCount > 0
            ? $"تم تهيئة {createdCount} رصيد إجازة للسنة {request.Year}"
            : $"جميع الأرصدة موجودة مسبقاً للسنة {request.Year}";

        return Result<bool>.Success(true, message);
    }
}
