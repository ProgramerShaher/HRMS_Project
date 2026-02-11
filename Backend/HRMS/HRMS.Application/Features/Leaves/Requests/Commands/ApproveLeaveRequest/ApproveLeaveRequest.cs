using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Core;

namespace HRMS.Application.Features.Leaves.Requests.Commands.ApproveLeaveRequest;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Command to approve a leave request.
/// Uses database transaction to atomically update status and deduct balance.
/// </summary>
public record ApproveLeaveRequestCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف طلب الإجازة
    /// Request ID
    /// </summary>
    public int RequestId { get; init; }

    /// <summary>
    /// تعليق المدير (اختياري)
    /// Manager comment (optional)
    /// </summary>
    public string? ManagerComment { get; init; }

    /// <summary>
    /// معرف المدير المعتمد
    /// Approving manager ID
    /// </summary>
    public int? ApprovedBy { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Validator for ApproveLeaveRequestCommand.
/// Ensures valid request ID is provided.
/// </summary>
public class ApproveLeaveRequestCommandValidator : AbstractValidator<ApproveLeaveRequestCommand>
{
    public ApproveLeaveRequestCommandValidator()
    {
        // التحقق من معرف الطلب
        // Validate request ID
        RuleFor(x => x.RequestId)
            .GreaterThan(0).WithMessage("معرف الطلب غير صحيح");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for approving a leave request.
/// Uses transaction to ensure atomic balance deduction and status update.
/// </summary>
public class ApproveLeaveRequestCommandHandler : IRequestHandler<ApproveLeaveRequestCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public ApproveLeaveRequestCommandHandler(IApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Result<bool>> Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        // ... (Transaction Start) ...
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // ... (Workflow Logic) ...
            var workflowApproval = await _context.WorkflowApprovals
                .FirstOrDefaultAsync(w => 
                    w.RequestId == request.RequestId 
                    && w.RequestType == "LEAVE" 
                    && w.Status == "PENDING", 
                    cancellationToken);
            
            if (workflowApproval == null)
            {
               // Handle missing workflow
            }

            if (workflowApproval != null)
            {
                workflowApproval.Status = "APPROVED";
                workflowApproval.ApprovalDate = DateTime.UtcNow;
                workflowApproval.Comments = request.ManagerComment;
            }

            bool isFinalLevel = true; 

            if (isFinalLevel)
            {
                var leaveRequest = await _context.LeaveRequests
                    .Include(lr => lr.LeaveType)
                    .Include(lr => lr.Employee) // Include Employee to get UserId for notification
                    .FirstOrDefaultAsync(lr => lr.RequestId == request.RequestId && lr.IsDeleted == 0, 
                        cancellationToken);

                if (leaveRequest == null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<bool>.Failure("طلب الإجازة غير موجود", 404);
                }

                // ... (Deduction Logic) ...
                if (leaveRequest.LeaveType.IsDeductible == 1)
                {
                    var year = (short)leaveRequest.StartDate.Year;
                    var balance = await _context.EmployeeLeaveBalances
                        .FirstOrDefaultAsync(b => 
                            b.EmployeeId == leaveRequest.EmployeeId 
                            && b.LeaveTypeId == leaveRequest.LeaveTypeId 
                            && b.Year == year
                            && b.IsDeleted == 0, 
                            cancellationToken);

                    if (balance == null || balance.CurrentBalance < leaveRequest.DaysCount)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<bool>.Failure("الرصيد غير كافٍ لإتمام العملية", 400);
                    }

                    balance.CurrentBalance -= (short)leaveRequest.DaysCount;
                }

                leaveRequest.Status = "MANAGER_APPROVED"; 
                if (!string.IsNullOrWhiteSpace(request.ManagerComment))
                {
                    leaveRequest.RejectionReason = request.ManagerComment.Trim();
                }

                // ═══════════════════════════════════════════════════════════════════════════
                // الخطوة 6: إرسال تنبيه للموظف
                // Step 6: Send Notification to Employee
                // ═══════════════════════════════════════════════════════════════════════════
                 if (leaveRequest.Employee != null && !string.IsNullOrEmpty(leaveRequest.Employee.UserId))
                {
                    await _notificationService.SendAsync(
                        userId: leaveRequest.Employee.UserId,
                        title: "تم اعتماد طلب الإجازة",
                        message: $"تمت الموافقة على طلب الإجازة ({leaveRequest.LeaveType.LeaveNameAr}) من قبل المدير.",
                        type: "Success",
                        referenceType: "LeaveRequest",
                        referenceId: leaveRequest.RequestId.ToString()
                    );
                }
            }
            
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<bool>.Success(true, "تم اعتماد الطلب بنجاح");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<bool>.Failure($"حدث خطأ أثناء اعتماد الطلب: {ex.Message}", 500);
        }
    }
}
