using FluentValidation;
using HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace HRMS.Application.Features.Attendance.Requests;

// ═══════════════════════════════════════════════════════════
// 1. DTO
// ═══════════════════════════════════════════════════════════
public class ShiftSwapRequestDto
{
    public int RequestId { get; set; }
    public int RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public int TargetEmployeeId { get; set; }
    public string TargetEmployeeName { get; set; } = string.Empty;
    public DateTime RosterDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ManagerComment { get; set; }
}

// ═══════════════════════════════════════════════════════════
// 2. CREATE REQUEST
// ═══════════════════════════════════════════════════════════
public record CreateSwapRequestCommand(int RequesterId, int TargetEmployeeId, DateTime RosterDate, string Reason) : IRequest<Result<int>>;

public class CreateSwapRequestValidator : AbstractValidator<CreateSwapRequestCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateSwapRequestValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(p => p.RequesterId).GreaterThan(0);
        RuleFor(p => p.TargetEmployeeId).GreaterThan(0).NotEqual(p => p.RequesterId).WithMessage("لا يمكن تبديل المناوبة مع نفسك");
        RuleFor(p => p.RosterDate).GreaterThanOrEqualTo(DateTime.Today).WithMessage("لا يمكن طلب تبديل لتاريخ سابق");
        
        RuleFor(p => p).MustAsync(async (cmd, ct) => 
        {
            return await _context.EmployeeRosters
                .AnyAsync(r => r.EmployeeId == cmd.RequesterId && r.RosterDate == cmd.RosterDate && r.IsOffDay == 0, ct);
        }).WithMessage("لا يوجد مناوبة لك في هذا التاريخ لتبديلها");
    }
}

public class CreateSwapRequestHandler : IRequestHandler<CreateSwapRequestCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateSwapRequestHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateSwapRequestCommand request, CancellationToken cancellationToken)
    {
        // Payroll Lock Check
        if (await _context.PayrollRuns.AnyAsync(p => p.Year == request.RosterDate.Year && p.Month == request.RosterDate.Month && (p.Status == "APPROVED" || p.Status == "PAID"), cancellationToken))
        {
            return Result<int>.Failure("لا يمكن تقديم طلب لشهر تم إغلاق مسير الرواتب له");
        }

        var targetExists = await _context.Employees.AnyAsync(e => e.EmployeeId == request.TargetEmployeeId, cancellationToken);
        if (!targetExists) return Result<int>.Failure("الموظف البديل غير موجود");

        var swapRequest = new ShiftSwapRequest
        {
            RequesterId = request.RequesterId,
            TargetEmployeeId = request.TargetEmployeeId,
            RosterDate = request.RosterDate,
            Status = "PENDING",
            ManagerComment = request.Reason
        };

        _context.ShiftSwapRequests.Add(swapRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(swapRequest.RequestId, "تم إرسال طلب التبديل بنجاح");
    }
}

// ═══════════════════════════════════════════════════════════
// 3. UPDATE REQUEST (PENDING ONLY)
// ═══════════════════════════════════════════════════════════
public record UpdateSwapRequestCommand(int RequestId, int TargetEmployeeId, string Reason) : IRequest<Result<bool>>;

public class UpdateSwapRequestHandler : IRequestHandler<UpdateSwapRequestCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateSwapRequestHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateSwapRequestCommand request, CancellationToken cancellationToken)
    {
        var swapRequest = await _context.ShiftSwapRequests.FindAsync(new object[] { request.RequestId }, cancellationToken);
        if (swapRequest == null) return Result<bool>.Failure("الطلب غير موجود");
        
        if (swapRequest.Status != "PENDING") return Result<bool>.Failure("لا يمكن تعديل طلب غير معلق");

        // Payroll Lock Check
        if (await _context.PayrollRuns.AnyAsync(p => p.Year == swapRequest.RosterDate.Year && p.Month == swapRequest.RosterDate.Month && (p.Status == "APPROVED" || p.Status == "PAID"), cancellationToken))
           return Result<bool>.Failure("الشهر المالي مغلق");

        swapRequest.TargetEmployeeId = request.TargetEmployeeId;
        swapRequest.ManagerComment = request.Reason;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, "تم تعديل الطلب بنجاح");
    }
}

// ═══════════════════════════════════════════════════════════
// 4. CANCEL REQUEST (DELETE - SOFT TO CANCELLED)
// ═══════════════════════════════════════════════════════════
public record CancelSwapRequestCommand(int RequestId) : IRequest<Result<bool>>;

public class CancelSwapRequestHandler : IRequestHandler<CancelSwapRequestCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CancelSwapRequestHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CancelSwapRequestCommand request, CancellationToken cancellationToken)
    {
        var swapRequest = await _context.ShiftSwapRequests.FindAsync(new object[] { request.RequestId }, cancellationToken);
        if (swapRequest == null) return Result<bool>.Failure("الطلب غير موجود");

        if (swapRequest.Status != "PENDING") return Result<bool>.Failure("لا يمكن إلغاء طلب غير معلق");

        swapRequest.Status = "CANCELLED";
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, "تم إلغاء الطلب بنجاح");
    }
}

// ═══════════════════════════════════════════════════════════
// 5. ACTION REQUEST (APPROVE/REJECT)
// ═══════════════════════════════════════════════════════════
public record ActionSwapRequestCommand(int RequestId, string Action, string Comment) : IRequest<Result<bool>>;

public class ActionSwapRequestValidator : AbstractValidator<ActionSwapRequestCommand>
{
    public ActionSwapRequestValidator()
    {
        RuleFor(p => p.RequestId).GreaterThan(0);
        RuleFor(p => p.Action).Must(a => a == "Approve" || a == "Reject").WithMessage("الإجراء يجب أن يكون Approve أو Reject");
    }
}

public class ActionSwapRequestHandler : IRequestHandler<ActionSwapRequestCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public ActionSwapRequestHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(ActionSwapRequestCommand request, CancellationToken cancellationToken)
    {
        var swapRequest = await _context.ShiftSwapRequests
            .FirstOrDefaultAsync(r => r.RequestId == request.RequestId, cancellationToken);
        
        if (swapRequest == null) return Result<bool>.Failure("الطلب غير موجود");
        if (swapRequest.Status != "PENDING") return Result<bool>.Failure($"لا يمكن تعديل طلب بحالة {swapRequest.Status}");

        // Payroll Lock Check
        if (await _context.PayrollRuns.AnyAsync(p => p.Year == swapRequest.RosterDate.Year && p.Month == swapRequest.RosterDate.Month && (p.Status == "APPROVED" || p.Status == "PAID"), cancellationToken))
           return Result<bool>.Failure("الشهر المالي مغلق");

        if (request.Action == "Reject")
        {
            swapRequest.Status = "REJECTED";
            swapRequest.ManagerComment = request.Comment;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true, "تم رفض الطلب");
        }
        else if (request.Action == "Approve")
        {
            // CRITICAL LOGIC: SWAP SHIFTS
            var rosterA = await _context.EmployeeRosters
                .FirstOrDefaultAsync(r => r.EmployeeId == swapRequest.RequesterId && r.RosterDate == swapRequest.RosterDate, cancellationToken);

            var rosterB = await _context.EmployeeRosters
                .FirstOrDefaultAsync(r => r.EmployeeId == swapRequest.TargetEmployeeId && r.RosterDate == swapRequest.RosterDate, cancellationToken);
            
            if (rosterA == null) return Result<bool>.Failure("لا يوجد جدول للموظف مقدم الطلب في هذا التاريخ");
            if (rosterB == null) return Result<bool>.Failure("لا يوجد جدول للموظف البديل في هذا التاريخ");

            // Swap
            var tempShift = rosterA.ShiftId;
            var tempOff = rosterA.IsOffDay;

            rosterA.ShiftId = rosterB.ShiftId;
            rosterA.IsOffDay = rosterB.IsOffDay;

            rosterB.ShiftId = tempShift;
            rosterB.IsOffDay = tempOff;

            swapRequest.Status = "APPROVED";
            swapRequest.ManagerComment = request.Comment;

            await _context.SaveChangesAsync(cancellationToken);
            
            return Result<bool>.Success(true, "تم تبديل المناوبات بنجاح وتحديث الجدول");
        }

        return Result<bool>.Failure("إجراء غير معروف");
    }
}

// ═══════════════════════════════════════════════════════════
// 6. REVOKE APPROVED SWAP (Revert Changes)
// ═══════════════════════════════════════════════════════════
public record RevokeSwapRequestCommand(int RequestId, string Reason) : IRequest<Result<bool>>;

public class RevokeSwapRequestHandler : IRequestHandler<RevokeSwapRequestCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public RevokeSwapRequestHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(RevokeSwapRequestCommand request, CancellationToken cancellationToken)
    {
        var swapRequest = await _context.ShiftSwapRequests.FindAsync(new object[] { request.RequestId }, cancellationToken);
        if (swapRequest == null) return Result<bool>.Failure("الطلب غير موجود");

        if (swapRequest.Status != "APPROVED") return Result<bool>.Failure("يمكن فقط سحب الطلبات المعتمدة");

        // Payroll Lock Check
        if (await _context.PayrollRuns.AnyAsync(p => p.Year == swapRequest.RosterDate.Year && p.Month == swapRequest.RosterDate.Month && (p.Status == "APPROVED" || p.Status == "PAID"), cancellationToken))
           return Result<bool>.Failure("الشهر المالي مغلق");

        // 1. Revert rosters
        var rosterA = await _context.EmployeeRosters
                .FirstOrDefaultAsync(r => r.EmployeeId == swapRequest.RequesterId && r.RosterDate == swapRequest.RosterDate, cancellationToken);
        var rosterB = await _context.EmployeeRosters
                .FirstOrDefaultAsync(r => r.EmployeeId == swapRequest.TargetEmployeeId && r.RosterDate == swapRequest.RosterDate, cancellationToken);

        if (rosterA != null && rosterB != null)
        {
            // Swap Back
            var tempShift = rosterA.ShiftId;
            var tempOff = rosterA.IsOffDay;

            rosterA.ShiftId = rosterB.ShiftId;
            rosterA.IsOffDay = rosterB.IsOffDay;

            rosterB.ShiftId = tempShift;
            rosterB.IsOffDay = tempOff;
        }

        // 2. Update Status
        swapRequest.Status = "REVOKED";
        swapRequest.ManagerComment = $"{swapRequest.ManagerComment} | Revoked: {request.Reason}";

        await _context.SaveChangesAsync(cancellationToken);

        // 3. Reprocess Attendance
        await _mediator.Send(new ProcessAttendanceCommand(swapRequest.RosterDate), cancellationToken);

        return Result<bool>.Success(true, "تم سحب الموافقة وإعادة المناوبات للأصل");
    }
}
