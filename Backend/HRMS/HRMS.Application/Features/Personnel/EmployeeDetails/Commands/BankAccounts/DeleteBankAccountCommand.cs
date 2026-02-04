using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.BankAccounts;

public class DeleteBankAccountCommand : IRequest<Result<bool>>
{
    public int EmployeeId { get; set; }
    public int AccountId { get; set; }
}

public class DeleteBankAccountCommandHandler : IRequestHandler<DeleteBankAccountCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteBankAccountCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteBankAccountCommand request, CancellationToken cancellationToken)
    {
        var bankAccount = await _context.EmployeeBankAccounts
            .FirstOrDefaultAsync(b => b.AccountId == request.AccountId && b.EmployeeId == request.EmployeeId, cancellationToken);

        if (bankAccount == null)
            return Result<bool>.Failure("Bank Account not found");

        _context.EmployeeBankAccounts.Remove(bankAccount);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Bank Account deleted successfully");
    }
}
