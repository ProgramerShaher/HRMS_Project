using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Addresses;

public class UpdateAddressCommand : IRequest<Result<bool>>
{
    public int AddressId { get; set; }
    public int EmployeeId { get; set; }
    public string BuildingNumber { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string DistrictName { get; set; } = string.Empty;
    public int? CityId { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string AddressType { get; set; } = "National";
}

public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateAddressCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.AddressId == request.AddressId && a.EmployeeId == request.EmployeeId, cancellationToken);

        if (address == null)
            return Result<bool>.Failure("Address not found");

        address.BuildingNo = request.BuildingNumber;
        address.Street = request.StreetName;
        address.District = request.DistrictName;
        address.CityId = request.CityId;
        address.PostalCode = request.ZipCode;
        address.AddressType = request.AddressType;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Address updated successfully");
    }
}
