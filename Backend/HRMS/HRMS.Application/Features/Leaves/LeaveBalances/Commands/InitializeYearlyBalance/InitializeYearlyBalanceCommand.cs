using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.LeaveBalances.Commands.InitializeYearlyBalance;

// 1. Command
/// <summary>
/// أمر تهيئة أرصدة الإجازات السنوية للموظفين
/// </summary>
public record InitializeYearlyBalanceCommand(short Year) : IRequest<bool>;

// 2. Validator
public class InitializeYearlyBalanceValidator : AbstractValidator<InitializeYearlyBalanceCommand>
{
    public InitializeYearlyBalanceValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween((short)2000, (short)2100).WithMessage("السنة يجب أن تكون بين 2000 و 2100");
    }
}

// 3. Handler
public class InitializeYearlyBalanceCommandHandler : IRequestHandler<InitializeYearlyBalanceCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public InitializeYearlyBalanceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(InitializeYearlyBalanceCommand request, CancellationToken cancellationToken)
    {
        // 1. Get all employees
        var employees = await _context.Employees
            .Where(e => e.IsDeleted == (byte)0) // Only active employees
            .Select(e => e.EmployeeId)
            .ToListAsync(cancellationToken);

        // 2. Get all Deductible Leave Types (like Annual)
        var leaveTypes = await _context.LeaveTypes
            .Where(lt => lt.IsDeductible)
            .ToListAsync(cancellationToken);

        if (!leaveTypes.Any()) return false;

        foreach (var empId in employees)
        {
            foreach (var type in leaveTypes)
            {
                // Check if balance already exists
                var exists = await _context.LeaveBalances.AnyAsync(b => 
                    b.EmployeeId == empId && 
                    b.LeaveTypeId == type.LeaveTypeId && 
                    b.Year == request.Year, cancellationToken);

                if (!exists)
                {
                    var balance = new EmployeeLeaveBalance
                    {
                        EmployeeId = empId,
                        LeaveTypeId = type.LeaveTypeId,
                        Year = request.Year,
                        CurrentBalance = type.DefaultDays // Start with default balance
                    };
                    _context.LeaveBalances.Add(balance);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
