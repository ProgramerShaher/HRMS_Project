using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Addresses;

public class DeleteAddressCommand : IRequest<Result<bool>>
{
    public int EmployeeId { get; set; }
    public int AddressId { get; set; }
}

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteAddressCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.AddressId == request.AddressId && a.EmployeeId == request.EmployeeId, cancellationToken);

        if (address == null)
            return Result<bool>.Failure("Address not found");

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Address deleted successfully");
    }
}
