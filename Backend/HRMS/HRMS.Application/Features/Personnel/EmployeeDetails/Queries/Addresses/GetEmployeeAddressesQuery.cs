using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Addresses;

public class GetEmployeeAddressesQuery : IRequest<Result<List<EmployeeAddressDto>>>
{
    public int EmployeeId { get; set; }
}

public class GetEmployeeAddressesQueryHandler : IRequestHandler<GetEmployeeAddressesQuery, Result<List<EmployeeAddressDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeAddressesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<EmployeeAddressDto>>> Handle(GetEmployeeAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await _context.Addresses
            .Where(a => a.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<EmployeeAddressDto>>(addresses);
        return Result<List<EmployeeAddressDto>>.Success(dtos);
    }
}
