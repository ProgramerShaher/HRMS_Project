using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.BankAccounts;

public class UpdateBankAccountCommand : IRequest<Result<bool>>
{
    public int AccountId { get; set; }
    public int EmployeeId { get; set; }
    public int BankId { get; set; }
    public string Iban { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class UpdateBankAccountCommandHandler : IRequestHandler<UpdateBankAccountCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateBankAccountCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateBankAccountCommand request, CancellationToken cancellationToken)
    {
        var bankAccount = await _context.EmployeeBankAccounts
            .FirstOrDefaultAsync(b => b.AccountId == request.AccountId && b.EmployeeId == request.EmployeeId, cancellationToken);

        if (bankAccount == null)
            return Result<bool>.Failure("Bank Account not found");

        if (request.IsPrimary)
        {
            var existingPrimary = await _context.EmployeeBankAccounts
                .Where(b => b.EmployeeId == request.EmployeeId && b.IsPrimary == 1 && b.AccountId != request.AccountId)
                .ToListAsync(cancellationToken);
            
            foreach (var bank in existingPrimary)
            {
                bank.IsPrimary = 0;
            }
        }

        bankAccount.BankId = request.BankId;
        bankAccount.Iban = request.Iban;
        bankAccount.AccountNumber = request.AccountNumber;
        bankAccount.IsPrimary = request.IsPrimary ? (byte)1 : (byte)0;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Bank Account updated successfully");
    }
}
