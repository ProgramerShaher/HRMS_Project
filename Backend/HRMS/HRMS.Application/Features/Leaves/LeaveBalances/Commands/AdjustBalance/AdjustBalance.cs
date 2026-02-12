using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.LeaveBalances.Commands.AdjustBalance;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Command to manually adjust an employee's leave balance.
/// Used for corrections, bonuses, or penalties.
/// </summary>
public record AdjustBalanceCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف الموظف
    /// Employee ID
    /// </summary>
    public int EmployeeId { get; init; }

    /// <summary>
    /// معرف نوع الإجازة
    /// Leave type ID
    /// </summary>
    public int LeaveTypeId { get; init; }

    /// <summary>
    /// السنة
    /// Year
    /// </summary>
    public short Year { get; init; }

    /// <summary>
    /// مقدار التعديل (موجب للإضافة، سالب للخصم)
    /// Adjustment amount (positive to add, negative to deduct)
    /// </summary>
    public short AdjustmentDays { get; init; }

    /// <summary>
    /// سبب التعديل (مطلوب)
    /// Adjustment reason (required)
    /// </summary>
    public string Reason { get; init; } = string.Empty;
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Validator for AdjustBalanceCommand.
/// Ensures all required fields are valid.
/// </summary>
public class AdjustBalanceCommandValidator : AbstractValidator<AdjustBalanceCommand>
{
    public AdjustBalanceCommandValidator()
    {
        // التحقق من معرف الموظف
        // Validate employee ID
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("معرف الموظف غير صحيح");

        // التحقق من معرف نوع الإجازة
        // Validate leave type ID
        RuleFor(x => x.LeaveTypeId)
            .GreaterThan(0).WithMessage("معرف نوع الإجازة غير صحيح");

        // التحقق من السنة
        // Validate year
        RuleFor(x => x.Year)
            .InclusiveBetween((short)2000, (short)2100).WithMessage("السنة يجب أن تكون بين 2000 و 2100");

        // التحقق من مقدار التعديل
        // Validate adjustment days
        RuleFor(x => x.AdjustmentDays)
            .NotEqual((short)0).WithMessage("مقدار التعديل لا يمكن أن يكون صفر")
            .GreaterThanOrEqualTo((short)-365).WithMessage("مقدار التعديل لا يمكن أن يكون أقل من -365 يوم")
            .LessThanOrEqualTo((short)365).WithMessage("مقدار التعديل لا يمكن أن يتجاوز 365 يوم");

        // التحقق من سبب التعديل
        // Validate reason
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("سبب التعديل مطلوب")
            .MinimumLength(10).WithMessage("سبب التعديل يجب أن يكون واضحاً (10 أحرف على الأقل)")
            .MaximumLength(200).WithMessage("سبب التعديل لا يمكن أن يتجاوز 200 حرف");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for adjusting employee leave balance.
/// Validates existence and applies adjustment with reason tracking.
/// </summary>
public class AdjustBalanceCommandHandler : IRequestHandler<AdjustBalanceCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public AdjustBalanceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(AdjustBalanceCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: التحقق من وجود الموظف
        // Step 1: Verify employee exists
        // ═══════════════════════════════════════════════════════════════════════════

        var employeeExists = await _context.Employees
            .AnyAsync(e => e.EmployeeId == request.EmployeeId && e.IsDeleted == 0, cancellationToken);

        if (!employeeExists)
        {
            return Result<bool>.Failure("الموظف غير موجود", 404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: التحقق من وجود نوع الإجازة
        // Step 2: Verify leave type exists
        // ═══════════════════════════════════════════════════════════════════════════

        var leaveTypeExists = await _context.LeaveTypes
            .AnyAsync(lt => lt.LeaveTypeId == request.LeaveTypeId && lt.IsDeleted == 0, cancellationToken);

        if (!leaveTypeExists)
        {
            return Result<bool>.Failure("نوع الإجازة غير موجود", 404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: الحصول على الرصيد أو إنشاؤه
        // Step 3: Get or create balance
        // ═══════════════════════════════════════════════════════════════════════════

        var balance = await _context.EmployeeLeaveBalances
            .FirstOrDefaultAsync(b => 
                b.EmployeeId == request.EmployeeId 
                && b.LeaveTypeId == request.LeaveTypeId 
                && b.Year == request.Year
                && b.IsDeleted == 0, 
                cancellationToken);

        if (balance == null)
        {
            return Result<bool>.Failure(
                "رصيد الإجازة غير موجود للموظف في هذه السنة. يرجى تهيئة الأرصدة أولاً.", 
                404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 4: تطبيق التعديل
        // Step 4: Apply adjustment
        // ═══════════════════════════════════════════════════════════════════════════

        var newBalance = balance.CurrentBalance + request.AdjustmentDays;

        // نتحقق من أن الرصيد الجديد لا يكون سالباً
        // Why: لا يمكن أن يكون رصيد الموظف سالباً
        if (newBalance < 0)
        {
            return Result<bool>.Failure(
                $"لا يمكن تطبيق التعديل. الرصيد الحالي ({balance.CurrentBalance}) لا يكفي للخصم ({Math.Abs(request.AdjustmentDays)} يوم)", 
                400);
        }

        balance.CurrentBalance = newBalance;

        // إنشاء حركة تعديل رصيد
        var transactionRecord = new LeaveTransaction
        {
            EmployeeId = request.EmployeeId,
            LeaveTypeId = request.LeaveTypeId,
            TransactionType = "ADJUSTMENT",
            Days = request.AdjustmentDays,
            TransactionDate = DateTime.UtcNow,
            Notes = request.Reason
        };
        _context.LeaveTransactions.Add(transactionRecord);

        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 5: إرجاع النتيجة بنجاح
        // Step 5: Return success result
        // ═══════════════════════════════════════════════════════════════════════════

        var adjustmentType = request.AdjustmentDays > 0 ? "إضافة" : "خصم";
        var message = $"تم {adjustmentType} {Math.Abs(request.AdjustmentDays)} يوم. الرصيد الجديد: {newBalance} يوم. السبب: {request.Reason}";

        return Result<bool>.Success(true, message);
    }
}
