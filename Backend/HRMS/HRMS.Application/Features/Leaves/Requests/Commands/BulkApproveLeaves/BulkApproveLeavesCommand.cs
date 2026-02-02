using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.Requests.Commands.BulkApproveLeaves;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// أمر الاعتماد الجماعي لطلبات الإجازة
/// Command to bulk approve leave requests.
/// Performs specific validation and logging for each request.
/// </summary>
public record BulkApproveLeavesCommand : IRequest<Result<BulkApproveResultDto>>
{
    public List<int> RequestIds { get; init; } = new();
    public string? ManagerComment { get; init; }
    public int PerformedByEmployeeId { get; init; } // معرف الموظف الذي قام بالعملية
}

public record BulkApproveResultDto(int TotalProcessed, int SuccessCount, int FailureCount, List<string> Errors);

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

public class BulkApproveLeavesCommandValidator : AbstractValidator<BulkApproveLeavesCommand>
{
    public BulkApproveLeavesCommandValidator()
    {
        RuleFor(x => x.RequestIds)
            .NotEmpty().WithMessage("يجب اختيار طلب واحد على الأقل");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

public class BulkApproveLeavesCommandHandler : IRequestHandler<BulkApproveLeavesCommand, Result<BulkApproveResultDto>>
{
    private readonly IApplicationDbContext _context;

    public BulkApproveLeavesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<BulkApproveResultDto>> Handle(BulkApproveLeavesCommand request, CancellationToken cancellationToken)
    {
        // النتيجة النهائية
        var resultDto = new BulkApproveResultDto(request.RequestIds.Count, 0, 0, new List<string>());
        int successCount = 0;
        int failureCount = 0;

        // جلب جميع الطلبات المطلوبة دفعة واحدة لتحسين الأداء
        var leaves = await _context.LeaveRequests
            .Include(l => l.LeaveType)
            .Where(l => request.RequestIds.Contains(l.RequestId))
            .ToListAsync(cancellationToken);

        // التحقق من وجود جميع الطلبات (اختياري، هنا سنعالج الموجود فقط)
        
        // استخدام استراتيجية "معالجة ما يمكن معالجته" (Process what you can)
        // ولكن مع Transaction لكل عملية ناجحة أو Transaction واحد للكل؟
        // User Standard: ERP usually implies "Batch Process".
        // سنقوم بمعالجة كل طلب على حدة داخل Transaction واحد كبير، إذا فشل واحد لا يوقف الباقي (Partial Success)
        // ولكن لضمان سلامة البيانات، الأفضل هو Transaction واحد للكل إذا كان المطلوب هو "اعتماد الكل"
        // أو Transaction لكل طلب.
        // سنستخدم نهج Loop with Try-Catch per Item إذا أردنا السماح بنجاح البعض وفشل البعض.
        // لكن بما أننا نعدل أرصدة، الأفضل هو Transaction لكل طلب مستقل لضمان عدم ضياع التغييرات الناجحة.

        foreach (var leaveRequestId in request.RequestIds)
        {
            var leaveRequest = leaves.FirstOrDefault(l => l.RequestId == leaveRequestId);
            
            if (leaveRequest == null)
            {
                resultDto.Errors.Add($"الطلب رقم {leaveRequestId} غير موجود");
                failureCount++;
                continue;
            }

            // نبدأ Transaction صغير لكل طلب لضمان استقلالية العمليات
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. التحقق من الحالة
                if (leaveRequest.Status != "PENDING")
                {
                    throw new InvalidOperationException($"الطلب رقم {leaveRequestId} حالته {leaveRequest.Status} ولا يمكن اعتماده");
                }

                // 2. خصم الرصيد
                if (leaveRequest.LeaveType.IsDeductible == 1)
                {
                    var year = (short)leaveRequest.StartDate.Year;
                    var balance = await _context.EmployeeLeaveBalances
                        .FirstOrDefaultAsync(b => 
                            b.EmployeeId == leaveRequest.EmployeeId 
                            && b.LeaveTypeId == leaveRequest.LeaveTypeId 
                            && b.Year == year,
                            cancellationToken);

                    if (balance == null || balance.CurrentBalance < leaveRequest.DaysCount)
                    {
                        throw new InvalidOperationException($"الرصيد غير كافٍ للموظف صاحب الطلب {leaveRequestId}");
                    }

                    balance.CurrentBalance -= (decimal)leaveRequest.DaysCount;
                }

                // 3. تحديث الحالة
                leaveRequest.Status = "MANAGER_APPROVED"; // Approved
                leaveRequest.IsPostedToBalance = (byte)(leaveRequest.LeaveType.IsDeductible == 1 ? 1 : 0);
                
                // 4. تسجيل التاريخ (Audit Trail)
                var history = new LeaveApprovalHistory
                {
                    RequestId = leaveRequest.RequestId,
                    ActionType = "APPROVE", // Approve
                    PerformedByEmployeeId = request.PerformedByEmployeeId,
                    ActionDate = DateTime.UtcNow,
                    Comment = request.ManagerComment,
                    PreviousStatus = "PENDING",
                    NewStatus = "MANAGER_APPROVED"
                };
                _context.LeaveApprovalHistory.Add(history);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                successCount++;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                resultDto.Errors.Add($"فشل اعتماد الطلب {leaveRequestId}: {ex.Message}");
                failureCount++;
            }
        }

        // تحديث النتيجة النهائية
        var finalResult = resultDto with { SuccessCount = successCount, FailureCount = failureCount };
        
        return Result<BulkApproveResultDto>.Success(finalResult, 
            $"تمت المعالجة: {successCount} ناجح، {failureCount} فاشل");
    }
}
