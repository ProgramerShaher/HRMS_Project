using FluentValidation;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.BankAccounts;

public class AddBankAccountCommand : IRequest<Result<int>>
{
    public int EmployeeId { get; set; }
    public int BankId { get; set; }
    public string Iban { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
}

public class AddBankAccountCommandHandler : IRequestHandler<AddBankAccountCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddBankAccountCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddBankAccountCommand request, CancellationToken cancellationToken)
    {
        // Check if primary exists if this one is primary
        if (request.IsPrimary)
        {
            var existingPrimary = await _context.EmployeeBankAccounts
                .Where(b => b.EmployeeId == request.EmployeeId && b.IsPrimary == 1)
                .ToListAsync(cancellationToken);
            
            foreach (var bank in existingPrimary)
            {
                bank.IsPrimary = 0;
            }
        }

        var bankAccount = new EmployeeBankAccount
        {
            EmployeeId = request.EmployeeId,
            BankId = request.BankId,
            Iban = request.Iban,
            AccountNumber = request.AccountNumber,
            IsPrimary = request.IsPrimary ? (byte)1 : (byte)0
        };

        _context.EmployeeBankAccounts.Add(bankAccount);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(bankAccount.AccountId, "Bank Account added successfully");
    }
}

public class AddBankAccountCommandValidator : AbstractValidator<AddBankAccountCommand>
{
    public AddBankAccountCommandValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.BankId).GreaterThan(0);
        RuleFor(x => x.Iban).NotEmpty();
    }
}
