using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using HRMS.Core.Entities.Leaves; // ADDED
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.Requests.Commands.CancelLeaveRequest;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// أمر إلغاء طلب إجازة
/// Command to cancel a leave request.
/// Handles balance reversal if the request was already posted.
/// </summary>
public record CancelLeaveRequestCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف الطلب
    /// Request ID
    /// </summary>
    public int RequestId { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// مدقق أمر إلغاء طلب الإجازة
/// Validator for CancelLeaveRequestCommand.
/// </summary>
public class CancelLeaveRequestCommandValidator : AbstractValidator<CancelLeaveRequestCommand>
{
    public CancelLeaveRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .GreaterThan(0).WithMessage("معرف الطلب غير صحيح");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// معالج إلغاء طلب الإجازة
/// Handler for canceling a leave request.
/// Reverses balance deduction if applicable.
/// </summary>
public class CancelLeaveRequestCommandHandler : IRequestHandler<CancelLeaveRequestCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CancelLeaveRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CancelLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: بدء Transaction لضمان سلامة البيانات
        // Step 1: Begin Transaction
        // ═══════════════════════════════════════════════════════════════════════════
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // ═══════════════════════════════════════════════════════════════════════════
            // الخطوة 2: جلب الطلب
            // Step 2: Get Leave Request
            // ═══════════════════════════════════════════════════════════════════════════
            var leaveRequest = await _context.LeaveRequests
                .Include(lr => lr.LeaveType)
                .FirstOrDefaultAsync(lr => lr.RequestId == request.RequestId && lr.IsDeleted == 0, cancellationToken);

            if (leaveRequest == null)
            {
                return Result<bool>.Failure("طلب الإجازة غير موجود", 404);
            }

            // ═══════════════════════════════════════════════════════════════════════════
            // الخطوة 3: التحقق من القواعد (Business Rules)
            // Step 3: Check Business Rules
            // ═══════════════════════════════════════════════════════════════════════════
            
            // القاعدة 1: الحالة يجب أن تكون PENDING أو MANAGER_APPROVED
            if (leaveRequest.Status != "PENDING" && leaveRequest.Status != "MANAGER_APPROVED")
            {
                return Result<bool>.Failure($"لا يمكن إلغاء الطلب لأن حالته الحالية هي: {leaveRequest.Status}", 400);
            }

            // القاعدة 2: التحقق من تاريخ البداية (اختياري، لكن مفضل)
            // إذا بدأت الإجازة بالفعل، قد نحتاج سياسة مختلفة (إلغاء جزئي)، لكن للتبسيط سنمنع الإلغاء
            //if (leaveRequest.StartDate < DateTime.Today)
            //{
            //    return Result<bool>.Failure("لا يمكن إلغاء إجازة بدأت بالفعل أو انتهت", 400);
            //}

            // ═══════════════════════════════════════════════════════════════════════════
            // الخطوة 4: استعادة الرصيد (Reversal Transaction)
            // Step 4: Reverse Balance Transaction
            // ═══════════════════════════════════════════════════════════════════════════
            
            EmployeeLeaveBalance? balance = null;

            if (leaveRequest.IsPostedToBalance == 1)
            {
                // البحث عن الرصيد المرتبط
                var year = (short)leaveRequest.StartDate.Year;
                balance = await _context.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(b => 
                        b.EmployeeId == leaveRequest.EmployeeId 
                        && b.LeaveTypeId == leaveRequest.LeaveTypeId 
                        && b.Year == year
                        && b.IsDeleted == 0, 
                        cancellationToken);

                if (balance != null)
                {
                    // استرجاع الأيام إلى الرصيد
                    // Re-add the days back to the balance
                    balance.CurrentBalance += (decimal)leaveRequest.DaysCount;
                }
                // ملاحظة: إذا لم نجد الرصيد، فهذا وضع غريب لطلب IsPostedToBalance=1
                // لكن لن نوقف العملية، سنقوم فقط بتحديث حالة الطلب
            }

            // ═══════════════════════════════════════════════════════════════════════════
            // الخطوة 5: تحديث حالة الطلب
            // Step 5: Update Request Status
            // ═══════════════════════════════════════════════════════════════════════════
            
            leaveRequest.Status = "CANCELLED";
            leaveRequest.IsPostedToBalance = 0; // لم يعد مخصوماً
            leaveRequest.RejectionReason = "Cancelled by user"; // أو أي سبب مناسب

            // تسجيل حركة عكسية في سجل المعاملات (Audit Trail)
            if (balance != null) // If we reversed balance (simplified check since we only care if balance was modified/loaded)
            {
                 var reversalTransaction = new LeaveTransaction
                 {
                     EmployeeId = leaveRequest.EmployeeId,
                     LeaveTypeId = leaveRequest.LeaveTypeId,
                     TransactionType = "CANCELLATION",
                     Days = leaveRequest.DaysCount, // Adding days back
                     TransactionDate = DateTime.Now,
                     Notes = $"Reversal of Request #{leaveRequest.RequestId}",
                     ReferenceId = leaveRequest.RequestId
                 };
                 _context.LeaveTransactions.Add(reversalTransaction);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<bool>.Success(true, "تم إلغاء طلب الإجازة بنجاح واستعادة الرصيد إن وجد");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<bool>.Failure($"حدث خطأ أثناء إلغاء الطلب: {ex.Message}", 500);
        }
    }
}
