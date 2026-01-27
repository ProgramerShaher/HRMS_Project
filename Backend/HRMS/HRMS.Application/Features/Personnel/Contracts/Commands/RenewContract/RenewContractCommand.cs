using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Contracts.Commands.RenewContract;

public record RenewContractCommand(RenewContractDto Data) : IRequest<int>;

public class RenewContractCommandHandler : IRequestHandler<RenewContractCommand, int>
{
    private readonly IApplicationDbContext _context;

    public RenewContractCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(RenewContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _context.Contracts
            .FirstOrDefaultAsync(c => c.ContractId == request.Data.ContractId, cancellationToken);

        if (contract == null)
            throw new KeyNotFoundException($"Contract {request.Data.ContractId} not found");

        // Validate Logic
        if (request.Data.NewStartDate <= contract.EndDate)
        {
            // Optional: throw exception or allow overlap depending on business rules.
            // Assuming strictly sequential:
            // throw new InvalidOperationException("New start date must be after current end date.");
        }

        var renewal = new ContractRenewal
        {
            ContractId = request.Data.ContractId,
            OldEndDate = contract.EndDate ?? DateTime.MinValue, // Handle null gracefully if logic permits
            NewStartDate = request.Data.NewStartDate,
            NewEndDate = request.Data.NewEndDate,
            Notes = request.Data.Notes,
            RenewalDate = DateTime.Now
        };

        _context.ContractRenewals.Add(renewal);

        // Update Contract
        contract.EndDate = request.Data.NewEndDate;

        await _context.SaveChangesAsync(cancellationToken);

        return renewal.RenewalId;
    }
}
