using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.LeaveTypes.Commands.DeleteLeaveType;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Command to delete a leave type (soft delete).
/// Returns success status wrapped in Result pattern.
/// </summary>
public record DeleteLeaveTypeCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف نوع الإجازة المراد حذفه
    /// Leave type ID to delete
    /// </summary>
    public int LeaveTypeId { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Validator for DeleteLeaveTypeCommand.
/// Ensures valid leave type ID is provided.
/// </summary>
public class DeleteLeaveTypeCommandValidator : AbstractValidator<DeleteLeaveTypeCommand>
{
    public DeleteLeaveTypeCommandValidator()
    {
        // التحقق من المعرف
        // Validate ID
        RuleFor(x => x.LeaveTypeId)
            .GreaterThan(0).WithMessage("معرف نوع الإجازة غير صحيح");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for deleting a leave type.
/// Implements soft delete to maintain data integrity.
/// </summary>
public class DeleteLeaveTypeCommandHandler : IRequestHandler<DeleteLeaveTypeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteLeaveTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: التحقق من وجود نوع الإجازة
        // Step 1: Verify leave type exists
        // ═══════════════════════════════════════════════════════════════════════════

        var leaveType = await _context.LeaveTypes
            .FirstOrDefaultAsync(lt => lt.LeaveTypeId == request.LeaveTypeId && lt.IsDeleted == 0, 
                cancellationToken);

        if (leaveType == null)
        {
            return Result<bool>.Failure("نوع الإجازة غير موجود أو تم حذفه مسبقاً", 404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: التحقق من عدم وجود أرصدة أو طلبات مرتبطة
        // Step 2: Check for related balances or requests
        // ═══════════════════════════════════════════════════════════════════════════

        // نتحقق من وجود أرصدة مرتبطة بهذا النوع
        // Why: لا يمكن حذف نوع إجازة له أرصدة موظفين نشطة
        var hasBalances = await _context.LeaveBalances
            .AnyAsync(lb => lb.LeaveTypeId == request.LeaveTypeId && lb.IsDeleted == 0, 
                cancellationToken);

        if (hasBalances)
        {
            return Result<bool>.Failure(
                "لا يمكن حذف نوع الإجازة لأنه مرتبط بأرصدة موظفين. يرجى حذف الأرصدة أولاً.", 
                400);
        }

        // نتحقق من وجود طلبات إجازة مرتبطة بهذا النوع
        // Why: لا يمكن حذف نوع إجازة له طلبات نشطة
        var hasRequests = await _context.LeaveRequests
            .AnyAsync(lr => lr.LeaveTypeId == request.LeaveTypeId && lr.IsDeleted == 0, 
                cancellationToken);

        if (hasRequests)
        {
            return Result<bool>.Failure(
                "لا يمكن حذف نوع الإجازة لأنه مرتبط بطلبات إجازة. يرجى حذف الطلبات أولاً.", 
                400);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: الحذف المنطقي (Soft Delete)
        // Step 3: Soft delete
        // ═══════════════════════════════════════════════════════════════════════════

        // نستخدم الحذف المنطقي للحفاظ على سجل البيانات
        // Why: للتدقيق والرجوع إلى البيانات التاريخية
        leaveType.IsDeleted = 1;

        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 4: إرجاع النتيجة بنجاح
        // Step 4: Return success result
        // ═══════════════════════════════════════════════════════════════════════════

        return Result<bool>.Success(true, "تم حذف نوع الإجازة بنجاح");
    }
}
