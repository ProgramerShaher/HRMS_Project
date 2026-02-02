using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.Requests.Commands.RejectLeaveRequest;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Command to reject a leave request.
/// Requires mandatory rejection reason from manager.
/// </summary>
public record RejectLeaveRequestCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف طلب الإجازة
    /// Request ID
    /// </summary>
    public int RequestId { get; init; }

    /// <summary>
    /// سبب الرفض (مطلوب)
    /// Rejection reason (required)
    /// </summary>
    public string RejectionReason { get; init; } = string.Empty;

    /// <summary>
    /// معرف المدير الرافض
    /// Rejecting manager ID
    /// </summary>
    public int? RejectedBy { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Validator for RejectLeaveRequestCommand.
/// Ensures valid request ID and mandatory rejection reason.
/// </summary>
public class RejectLeaveRequestCommandValidator : AbstractValidator<RejectLeaveRequestCommand>
{
    public RejectLeaveRequestCommandValidator()
    {
        // التحقق من معرف الطلب
        // Validate request ID
        RuleFor(x => x.RequestId)
            .GreaterThan(0).WithMessage("معرف الطلب غير صحيح");

        // التحقق من سبب الرفض
        // Validate rejection reason
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("سبب الرفض مطلوب")
            .MinimumLength(10).WithMessage("سبب الرفض يجب أن يكون واضحاً (10 أحرف على الأقل)")
            .MaximumLength(500).WithMessage("سبب الرفض لا يمكن أن يتجاوز 500 حرف");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for rejecting a leave request.
/// Updates status to rejected with mandatory reason.
/// </summary>
public class RejectLeaveRequestCommandHandler : IRequestHandler<RejectLeaveRequestCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public RejectLeaveRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(RejectLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: جلب طلب الإجازة
        // Step 1: Get leave request
        // ═══════════════════════════════════════════════════════════════════════════

        var leaveRequest = await _context.LeaveRequests
            .FirstOrDefaultAsync(lr => lr.RequestId == request.RequestId && lr.IsDeleted == 0, 
                cancellationToken);

        if (leaveRequest == null)
        {
            return Result<bool>.Failure("طلب الإجازة غير موجود", 404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: التحقق من حالة الطلب
        // Step 2: Verify request status
        // ═══════════════════════════════════════════════════════════════════════════

        if (leaveRequest.Status != "PENDING") // Not Pending
        {
            return Result<bool>.Failure($"لا يمكن رفض الطلب. الطلب {leaveRequest.Status} مسبقاً", 400);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: تحديث حالة الطلب إلى مرفوض
        // Step 3: Update request status to rejected
        // ═══════════════════════════════════════════════════════════════════════════

        // نحدث الحالة إلى مرفوض مع تسجيل سبب الرفض
        // Why: الموظف يجب أن يعرف لماذا تم رفض طلبه
        leaveRequest.Status = "REJECTED"; // Rejected
        leaveRequest.RejectionReason = request.RejectionReason.Trim();


        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 4: إرجاع النتيجة بنجاح
        // Step 4: Return success result
        // ═══════════════════════════════════════════════════════════════════════════

        return Result<bool>.Success(true, "تم رفض طلب الإجازة بنجاح");
    }
}
