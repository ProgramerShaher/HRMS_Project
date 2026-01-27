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

// ═══════════════════════════════════════════════════════════
// 2. APPLY OVERTIME (CREATE)
// ═══════════════════════════════════════════════════════════
public record ApplyOvertimeCommand(int EmployeeId, DateTime WorkDate, decimal HoursRequested, string Reason) : IRequest<Result<int>>;

public class ApplyOvertimeValidator : AbstractValidator<ApplyOvertimeCommand>
{
    public ApplyOvertimeValidator()
    {
        RuleFor(p => p.EmployeeId).GreaterThan(0);
        RuleFor(p => p.WorkDate).NotEmpty().WithMessage("تاريخ العمل الإضافي مطلوب");
        RuleFor(p => p.HoursRequested).GreaterThan(0).WithMessage("يجب أن تكون الساعات أكبر من صفر");
        RuleFor(p => p.Reason).NotEmpty().WithMessage("السبب مطلوب");
    }
}

public class ApplyOvertimeHandler : IRequestHandler<ApplyOvertimeCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public ApplyOvertimeHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(ApplyOvertimeCommand request, CancellationToken cancellationToken)
    {
        // Payroll Lock Check
        if (await _context.PayrollRuns.AnyAsync(p => p.Year == request.WorkDate.Year && p.Month == request.WorkDate.Month && (p.Status == "APPROVED" || p.Status == "PAID"), cancellationToken))
            return Result<int>.Failure("الشهر المالي مغلق");

        var otRequest = new OvertimeRequest
        {
            EmployeeId = request.EmployeeId,
            WorkDate = request.WorkDate,
            HoursRequested = request.HoursRequested,
            Reason = request.Reason,
            Status = "PENDING"
        };

        _context.OvertimeRequests.Add(otRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(otRequest.OtRequestId, "تم تقديم طلب العمل الإضافي بنجاح");
    }
}

// ═══════════════════════════════════════════════════════════
// 3. UPDATE REQUEST (PENDING ONLY)
// ═══════════════════════════════════════════════════════════
public record UpdateOvertimeCommand(int RequestId, decimal HoursRequested, string Reason) : IRequest<Result<bool>>;

public class UpdateOvertimeHandler : IRequestHandler<UpdateOvertimeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateOvertimeHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateOvertimeCommand request, CancellationToken cancellationToken)
    {
        var otRequest = await _context.OvertimeRequests.FindAsync(new object[] { request.RequestId }, cancellationToken);
        if (otRequest == null) return Result<bool>.Failure("الطلب غير موجود");

        if (otRequest.Status != "PENDING") return Result<bool>.Failure("لا يمكن تعديل طلب غير معلق");

        // Payroll Lock
        if (await _context.PayrollRuns.AnyAsync(p => p.Year == otRequest.WorkDate.Year && p.Month == otRequest.WorkDate.Month && (p.Status == "APPROVED" || p.Status == "PAID"), cancellationToken))
            return Result<bool>.Failure("الشهر المالي مغلق");

        otRequest.HoursRequested = request.HoursRequested;
        otRequest.Reason = request.Reason;
        
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, "تم تعديل الطلب بنجاح");
    }
}

// ═══════════════════════════════════════════════════════════
// 4. CANCEL REQUEST (DELETE / CANCEL)
// ═══════════════════════════════════════════════════════════
public record CancelOvertimeCommand(int RequestId) : IRequest<Result<bool>>;

public class CancelOvertimeHandler : IRequestHandler<CancelOvertimeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public CancelOvertimeHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(CancelOvertimeCommand request, CancellationToken cancellationToken)
    {
        var otRequest = await _context.OvertimeRequests.FindAsync(new object[] { request.RequestId }, cancellationToken);
        if (otRequest == null) return Result<bool>.Failure("الطلب غير موجود");
        
        // Allowed to Cancel Pending and Approved (if payroll not locked)
        // Payroll Lock
        if (await _context.PayrollRuns.AnyAsync(p => p.Year == otRequest.WorkDate.Year && p.Month == otRequest.WorkDate.Month && (p.Status == "APPROVED" || p.Status == "PAID"), cancellationToken))
            return Result<bool>.Failure("الشهر المالي مغلق");

        var wasApproved = otRequest.Status == "APPROVED";

        otRequest.Status = "CANCELLED";
        await _context.SaveChangesAsync(cancellationToken);

        if (wasApproved)
        {
            // Reprocess to clear OT Minutes
            await _mediator.Send(new ProcessAttendanceCommand(otRequest.WorkDate), cancellationToken);
        }

        return Result<bool>.Success(true, "تم إلغاء الطلب بنجاح");
    }
}

// ═══════════════════════════════════════════════════════════
// 5. ACTION REQUEST (APPROVE/REJECT)
// ═══════════════════════════════════════════════════════════
public record ActionOvertimeCommand(int RequestId, int ManagerId, string Action, decimal? ApprovedHours, string Comment) : IRequest<Result<bool>>;

public class ActionOvertimeValidator : AbstractValidator<ActionOvertimeCommand>
{
    public ActionOvertimeValidator()
    {
        RuleFor(p => p.RequestId).GreaterThan(0);
        RuleFor(p => p.ManagerId).GreaterThan(0);
        RuleFor(p => p.Action).Must(a => a == "Approve" || a == "Reject").WithMessage("الإجراء يجب أن يكون Approve أو Reject");
        
        RuleFor(p => p).Must((cmd) => 
        {
            if (cmd.Action == "Approve") return cmd.ApprovedHours.HasValue && cmd.ApprovedHours > 0;
            return true;
        }).WithMessage("يجب تحديد الساعات المعتمدة عند الموافقة");
    }
}

public class ActionOvertimeHandler : IRequestHandler<ActionOvertimeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public ActionOvertimeHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(ActionOvertimeCommand request, CancellationToken cancellationToken)
    {
        var otRequest = await _context.OvertimeRequests
            .FirstOrDefaultAsync(r => r.OtRequestId == request.RequestId, cancellationToken);

        if (otRequest == null) return Result<bool>.Failure("الطلب غير موجود");
        
        // Payroll Lock
        if (await _context.PayrollRuns.AnyAsync(p => p.Year == otRequest.WorkDate.Year && p.Month == otRequest.WorkDate.Month && (p.Status == "APPROVED" || p.Status == "PAID"), cancellationToken))
            return Result<bool>.Failure("الشهر المالي مغلق");

        if (request.Action == "Reject")
        {
            otRequest.Status = "REJECTED";
            otRequest.ApprovedBy = request.ManagerId;
        }
        else if (request.Action == "Approve")
        {
            otRequest.Status = "APPROVED";
            otRequest.ApprovedBy = request.ManagerId;
            otRequest.ApprovedHours = request.ApprovedHours;
        }

        await _context.SaveChangesAsync(cancellationToken);

        // If Approved, Process Attendance
        if (request.Action == "Approve")
        {
            await _mediator.Send(new ProcessAttendanceCommand(otRequest.WorkDate), cancellationToken);
        }

        return Result<bool>.Success(true, "تم تحديث حالة الطلب بنجاح");
    }
}
