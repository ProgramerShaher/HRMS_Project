using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Utilities;
using MediatR;
using HRMS.Core.Entities.Core;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.Requests.Commands.CreateLeaveRequest;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Command to create a new leave request.
/// Validates balance availability and checks for overlapping requests.
/// </summary>
public record CreateLeaveRequestCommand : IRequest<Result<int>>
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
    /// تاريخ البداية
    /// Start date
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// تاريخ النهاية
    /// End date
    /// </summary>
    public DateTime EndDate { get; init; }

    /// <summary>
    /// سبب الإجازة
    /// Leave reason
    /// </summary>
    public string Reason { get; init; } = string.Empty;

    /// <summary>
    /// مسار المرفق (اختياري)
    /// Attachment path (optional)
    /// </summary>
    public string? AttachmentPath { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Validator for CreateLeaveRequestCommand.
/// Ensures all required fields are valid and dates are logical.
/// </summary>
public class CreateLeaveRequestCommandValidator : AbstractValidator<CreateLeaveRequestCommand>
{
    public CreateLeaveRequestCommandValidator()
    {
        // التحقق من معرف الموظف
        // Validate employee ID
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("معرف الموظف غير صحيح");

        // التحقق من معرف نوع الإجازة
        // Validate leave type ID
        RuleFor(x => x.LeaveTypeId)
            .GreaterThan(0).WithMessage("معرف نوع الإجازة غير صحيح");

        // التحقق من تاريخ البداية
        // Validate start date
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاريخ البداية مطلوب")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("تاريخ البداية يجب أن يكون اليوم أو في المستقبل");

        // التحقق من تاريخ النهاية
        // Validate end date
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("تاريخ النهاية مطلوب")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("تاريخ النهاية يجب أن يكون بعد أو يساوي تاريخ البداية");

        // التحقق من سبب الإجازة
        // Validate reason
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("سبب الإجازة مطلوب")
            .MinimumLength(10).WithMessage("سبب الإجازة يجب أن يكون واضحاً (10 أحرف على الأقل)")
            .MaximumLength(500).WithMessage("سبب الإجازة لا يمكن أن يتجاوز 500 حرف");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for creating a new leave request.
/// Validates balance, checks for overlaps, and creates the request.
/// </summary>
public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public CreateLeaveRequestCommandHandler(IApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Result<int>> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // Begin Transaction
        // ═══════════════════════════════════════════════════════════════════════════
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // ═══════════════════════════════════════════════════════════════════════════
            // الخطوة 1: التحقق من وجود الموظف وجلب المدير المباشر
            // Step 1: Verify employee exists and fetch ManagerId
            // ═══════════════════════════════════════════════════════════════════════════

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId && e.IsDeleted == 0, cancellationToken);

            if (employee == null)
            {
                return Result<int>.Failure("الموظف غير موجود", 404);
            }

            // ═══════════════════════════════════════════════════════════════════════════
            // الخطوة 2: التحقق من نوع الإجازة
            // Step 2: Verify leave type exists
            // ═══════════════════════════════════════════════════════════════════════════

            var leaveType = await _context.LeaveTypes
                .FirstOrDefaultAsync(lt => lt.LeaveTypeId == request.LeaveTypeId && lt.IsDeleted == 0, cancellationToken);

            if (leaveType == null)
            {
                return Result<int>.Failure("نوع الإجازة غير موجود", 404);
            }

            // ═══════════════════════════════════════════════════════════════════════════
            // الخطوة 3: حساب عدد الأيام
            // Step 3: Calculate number of days
            // ═══════════════════════════════════════════════════════════════════════════

            var daysCount = (request.EndDate - request.StartDate).Days + 1;

            if (daysCount <= 0)
            {
                return Result<int>.Failure("عدد الأيام يجب أن يكون أكبر من صفر", 400);
            }

            // ═══════════════════════════════════════════════════════════════════════════
            // الخطوة 4: التحقق من الرصيد (إذا كان نوع الإجازة يخصم من الرصيد)
            // Step 4: Check balance (if leave type is deductible)
            // ═══════════════════════════════════════════════════════════════════════════

            if (leaveType.IsDeductible == 1)
            {
                var year = (short)request.StartDate.Year;
                var balance = await _context.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(b => 
                        b.EmployeeId == request.EmployeeId 
                        && b.LeaveTypeId == request.LeaveTypeId 
                        && b.Year == year
                        && b.IsDeleted == 0, 
                        cancellationToken);

                if (balance == null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<int>.Failure(
                        $"لا يوجد رصيد إجازة للموظف من نوع '{leaveType.LeaveNameAr}' للسنة الحالية", 
                        404);
                }

                if (balance.CurrentBalance < daysCount)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<int>.Failure(
                        $"الرصيد غير كافٍ. الرصيد الحالي: {balance.CurrentBalance} يوم، المطلوب: {daysCount} يوم", 
                        400);
                }
            }

            // ═══════════════════════════════════════════════════════════════════════════
            // الخطوة 5: التحقق من عدم وجود تداخل
            // Step 5: Check for overlapping requests
            // ═══════════════════════════════════════════════════════════════════════════

            var hasOverlap = await _context.LeaveRequests
                .AnyAsync(lr => 
                    lr.EmployeeId == request.EmployeeId
                    && lr.IsDeleted == 0
                    && (lr.Status == "PENDING" || lr.Status == "MANAGER_APPROVED" || lr.Status == "HR_APPROVED")
                    && ((request.StartDate >= lr.StartDate && request.StartDate <= lr.EndDate)
                        || (request.EndDate >= lr.StartDate && request.EndDate <= lr.EndDate)
                        || (request.StartDate <= lr.StartDate && request.EndDate >= lr.EndDate)),
                    cancellationToken);

            if (hasOverlap)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<int>.Failure(
                    "يوجد تداخل مع طلب إجازة آخر معتمد أو معلق في نفس الفترة", 
                    400);
            }

            // ═══════════════════════════════════════════════════════════════════════════
            // الخطوة 6: إنشاء طلب الإجازة
            // Step 6: Create leave request
            // ═══════════════════════════════════════════════════════════════════════════

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = request.EmployeeId,
                LeaveTypeId = request.LeaveTypeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                DaysCount = daysCount,
                Reason = request.Reason.Trim(),
                AttachmentPath = request.AttachmentPath,
                Status = "PENDING"
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync(cancellationToken); // Save to generate RequestId

            // ═══════════════════════════════════════════════════════════════════════════
            // الخطوة 7: تكامل سير العمل (Workflow Integration)
            // Step 7: Create Workflow Approval Record
            // ═══════════════════════════════════════════════════════════════════════════

            if (employee.ManagerId.HasValue)
            {
                var workflowApproval = new HRMS.Core.Entities.Core.WorkflowApproval
                {
                    RequestType = "LEAVE",
                    RequestId = leaveRequest.RequestId,
                    ApproverLevel = 1, // Initial Level
                    ApproverId = employee.ManagerId.Value,
                    Status = "PENDING",
                    ApprovalDate = null,
                    Comments = null,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.EmployeeId.ToString() // Or System
                };
                
                _context.WorkflowApprovals.Add(workflowApproval);
                await _context.SaveChangesAsync(cancellationToken);

                // ═══════════════════════════════════════════════════════════════════════════
                // الخطوة 8: إرسال تنبيه للمدير المباشر
                // Step 8: Send Notification to Manager
                // ═══════════════════════════════════════════════════════════════════════════
                if (employee.Manager != null && !string.IsNullOrEmpty(employee.Manager.UserId))
                {
                    await _notificationService.SendAsync(
                        userId: employee.Manager.UserId,
                        title: "طلب إجازة جديد",
                        message: $"قام الموظف {employee.FullNameAr} بتقديم طلب إجازة {leaveType.LeaveNameAr} لمدة {daysCount} يوم.",
                        type: "Info",
                        referenceType: "LeaveRequest",
                        referenceId: leaveRequest.RequestId.ToString()
                    );
                }
            }
            else
            {
                // If no manager...
            }

            await transaction.CommitAsync(cancellationToken);

            return Result<int>.Success(
                leaveRequest.RequestId, 
                $"تم تقديم طلب الإجازة بنجاح. عدد الأيام: {daysCount}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<int>.Failure($"حدث خطأ أثناء تقديم الطلب: {ex.Message}", 500);
        }
    }
}
