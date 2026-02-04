using FluentValidation;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Addresses;

public class AddAddressCommand : IRequest<Result<int>>
{
    public int EmployeeId { get; set; }
    public string BuildingNumber { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string DistrictName { get; set; } = string.Empty;
    public int? CityId { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string AddressType { get; set; } = "National";
}

public class AddAddressCommandHandler : IRequestHandler<AddAddressCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddAddressCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        var address = new EmployeeAddress
        {
            EmployeeId = request.EmployeeId,
            BuildingNo = request.BuildingNumber,
            Street = request.StreetName,
            District = request.DistrictName,
            CityId = request.CityId,
            PostalCode = request.ZipCode,
            AddressType = request.AddressType
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(address.AddressId, "Address added successfully");
    }
}

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        // RuleFor(x => x.CityId).GreaterThan(0); // Optional?
    }
}
