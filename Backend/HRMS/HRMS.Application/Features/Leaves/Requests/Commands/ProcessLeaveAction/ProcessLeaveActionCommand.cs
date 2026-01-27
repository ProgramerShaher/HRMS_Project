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
        var action = request.Data.Action; // Approve or Reject
        
        // 1. Fetch Request
        var leaveRequest = await _context.LeaveRequests
            .Include(r => r.LeaveType)
            .FirstOrDefaultAsync(r => r.RequestId == request.Data.RequestId, cancellationToken);

        if (leaveRequest == null)
            throw new NotFoundException("Leave Request", request.Data.RequestId);

        if (leaveRequest.Status != "PENDING")
            throw new FluentValidation.ValidationException($"لا يمكن تعديل طلب بحالة {leaveRequest.Status}");

        // 2. Process Action
        if (action == "Reject")
        {
            leaveRequest.Status = "REJECTED";
            leaveRequest.RejectionReason = request.Data.Comment;
        }
        else if (action == "Approve")
        {
            // 3. Deduct Balance (If applicable)
            if (leaveRequest.LeaveType.IsDeductible)
            {
                var year = (short)leaveRequest.StartDate.Year;
                var balance = await _context.LeaveBalances
                    .FirstOrDefaultAsync(b => b.EmployeeId == leaveRequest.EmployeeId && 
                                              b.LeaveTypeId == leaveRequest.LeaveTypeId && 
                                              b.Year == year, cancellationToken);
                
                if (balance == null || balance.CurrentBalance < leaveRequest.DaysCount)
                    throw new FluentValidation.ValidationException("رصيد الموظف لم يعد كافياً لإتمام الموافقة.");

                // Deduct
                balance.CurrentBalance -= leaveRequest.DaysCount;
                leaveRequest.IsPostedToBalance = 1;

                // 4. Create Transaction Log
                var transaction = new LeaveTransaction
                {
                    EmployeeId = leaveRequest.EmployeeId,
                    LeaveTypeId = leaveRequest.LeaveTypeId,
                    TransactionType = "DEDUCTION",
                    Days = -leaveRequest.DaysCount, // Negative for deduction
                    TransactionDate = DateTime.Now,
                    ReferenceId = leaveRequest.RequestId,
                    Notes = $"Approved Leave Request #{leaveRequest.RequestId}"
                };

                _context.LeaveTransactions.Add(transaction);
            }

            leaveRequest.Status = "APPROVED";
            // leaveRequest.ApprovedBy = User... (if we had that field)
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
