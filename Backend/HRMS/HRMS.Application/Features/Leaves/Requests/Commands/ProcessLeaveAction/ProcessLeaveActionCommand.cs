using FluentValidation;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Exceptions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;

namespace HRMS.Application.Features.Leaves.Requests.Commands.ProcessLeaveAction;

// 1. Command
/// <summary>
/// أمر معالجة طلب إجازة (موافقة/رفض)
/// </summary>
public record ProcessLeaveActionCommand(LeaveActionDto Data) : IRequest<bool>;

// 2. Validator
public class ProcessLeaveActionValidator : AbstractValidator<ProcessLeaveActionCommand>
{
    public ProcessLeaveActionValidator()
    {
        RuleFor(x => x.Data.RequestId).GreaterThan(0).WithMessage("معرف الطلب غير صحيح");
        RuleFor(x => x.Data.ManagerId).GreaterThan(0).WithMessage("معرف المدير مطلوب");
        RuleFor(x => x.Data.Action).NotEmpty().WithMessage("نوع الإجراء مطلوب")
            .Must(a => a == "Approve" || a == "Reject").WithMessage("الإجراء يجب أن يكون Approve أو Reject");
        
        RuleFor(x => x.Data.Comment).MaximumLength(200).WithMessage("التعليق طويل جداً");
    }
}

// 3. Handler
public class ProcessLeaveActionCommandHandler : IRequestHandler<ProcessLeaveActionCommand, bool>
{
    private readonly IApplicationDbContext _context;
    
    public ProcessLeaveActionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ProcessLeaveActionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Data.Action == "Approve")
            {
                // استدعاء بروسيجر الموافقة (الذي أصلح مشكلة الروستر)
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC [HR_LEAVES].[sp_ApproveLeaveRequest] @RequestId = {0}, @ManagerId = {1}",
                    new object[] { request.Data.RequestId, request.Data.ManagerId },
                    cancellationToken);
            }
            else if (request.Data.Action == "Reject")
            {
                // استدعاء بروسيجر الرفض الجديد (للتوحيد والاحترافية)
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC [HR_LEAVES].[sp_RejectLeaveRequest] @RequestId = {0}, @ManagerId = {1}, @Comment = {2}",
                    new object[] { request.Data.RequestId, request.Data.ManagerId, request.Data.Comment ?? "" },
                    cancellationToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("خطأ في تنفيذ الإجراء: " + ex.Message);
        }
    }
    //public async Task<bool> Handle(ProcessLeaveActionCommand request, CancellationToken cancellationToken)
    //{
    //    // Start Atomic Transaction
    //    using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    //    try
    //    {
    //        var action = request.Data.Action; // Approve or Reject

    //        // 1. Fetch Request with detailed info
    //        var leaveRequest = await _context.LeaveRequests
    //            .Include(r => r.LeaveType)
    //            .FirstOrDefaultAsync(r => r.RequestId == request.Data.RequestId, cancellationToken);

    //        if (leaveRequest == null)
    //            throw new NotFoundException("Leave Request", request.Data.RequestId);

    //        if (leaveRequest.Status != "PENDING")
    //            throw new FluentValidation.ValidationException($"لا يمكن تعديل طلب بحالة {leaveRequest.Status}");

    //        // 2. Process Action
    //        if (action == "Reject")
    //        {
    //            leaveRequest.Status = "REJECTED";
    //            leaveRequest.RejectionReason = request.Data.Comment;
    //            leaveRequest.CreatedBy = request.Data.ManagerId.ToString(); // Assuming ManagerId is int, storing as string or int depending on entity
    //            leaveRequest.CreatedAt = DateTime.Now;
    //        }
    //        else if (action == "Approve")
    //        {
    //            // 3. Balance Deduction (Critical Section)
    //            if (leaveRequest.LeaveType.IsDeductible)
    //            {
    //                var year = (short)leaveRequest.StartDate.Year;

    //                // Re-fetch balance to ensure latest data (prevent race conditions)
    //                var balance = await _context.LeaveBalances
    //                    .FirstOrDefaultAsync(b => b.EmployeeId == leaveRequest.EmployeeId && 
    //                                              b.LeaveTypeId == leaveRequest.LeaveTypeId && 
    //                                              b.Year == year, cancellationToken);

    //                if (balance == null)
    //                     throw new FluentValidation.ValidationException("لا يوجد رصيد إجازات معرف لهذا الموظف لهذه السنة.");

    //                if (balance.CurrentBalance < leaveRequest.DaysCount)
    //                    throw new FluentValidation.ValidationException($"رصيد الموظف ({balance.CurrentBalance}) غير كافٍ لخصم ({leaveRequest.DaysCount}) أيام.");

    //                // A. Deduct
    //                balance.CurrentBalance -= leaveRequest.DaysCount;
    //                leaveRequest.IsPostedToBalance = 1;

    //                // B. Audit Trail (Leave Transaction)
    //                var transactionLog = new LeaveTransaction
    //                {
    //                    EmployeeId = leaveRequest.EmployeeId,
    //                    LeaveTypeId = leaveRequest.LeaveTypeId,
    //                    TransactionType = "DEDUCTION",
    //                    Days = -leaveRequest.DaysCount, // Negative for deduction
    //                    TransactionDate = DateTime.Now,
    //                    ReferenceId = leaveRequest.RequestId,
    //                    Notes = $"Approved Leave Request #{leaveRequest.RequestId} by Manager {request.Data.ManagerId}"
    //                };

    //                _context.LeaveTransactions.Add(transactionLog);
    //            }

    //            // 4. Operational Sync (Roster Updates) - Using Raw SQL for Performance & Date Precision
    //            // Fix: Use CAST to ensure we compare only dates, ignoring time parts
    //            // 4. Operational Sync (Roster Updates) - الحل النهائي والآمن
    //            // نستخدم ExecuteSqlInterpolatedAsync للتعامل الصحيح مع الـ CancellationToken والبارامترات
    //            await _context.Database.ExecuteSqlInterpolatedAsync($@"
    //UPDATE [HR_ATTENDANCE].[EMPLOYEE_ROSTERS]
    //SET [STATUS] = 'ON_LEAVE', 
    //    [IS_OFF_DAY] = 1
    //WHERE [EMPLOYEE_ID] = {leaveRequest.EmployeeId} 
    //AND CAST([ROSTER_DATE] AS DATE) BETWEEN CAST({leaveRequest.StartDate} AS DATE) AND CAST({leaveRequest.EndDate} AS DATE)",
    //                cancellationToken); // هنا يتم تمريره كمعامل خارجي للميثود وليس كجزء من الـ SQL
    //            // Update Request Status
    //            leaveRequest.Status = "APPROVED";
    //            leaveRequest.CreatedBy = request.Data.ManagerId.ToString();
    //            leaveRequest.CreatedAt = DateTime.Now;
    //            if (!string.IsNullOrEmpty(request.Data.Comment))
    //            {
    //                 leaveRequest.RejectionReason = request.Data.Comment; // Using same field for comments
    //            }
    //        }

    //        // 5. Commit Changes
    //        await _context.SaveChangesAsync(cancellationToken);
    //        await transaction.CommitAsync(cancellationToken);

    //        return true;
    //    }
    //    catch (Exception)
    //    {
    //        // Rollback on any error
    //        await transaction.RollbackAsync(cancellationToken);
    //        throw; // Re-throw to let global exception handler manage the response
    //    }
    //}
}
