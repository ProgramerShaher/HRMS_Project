using AutoMapper;
using AutoMapper.QueryableExtensions;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Contracts.Queries.GetEmployeeContracts;

public record GetEmployeeContractsQuery(int EmployeeId) : IRequest<List<ContractDto>>;

public class GetEmployeeContractsQueryHandler : IRequestHandler<GetEmployeeContractsQuery, List<ContractDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeContractsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ContractDto>> Handle(GetEmployeeContractsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Contracts
            .Where(c => c.EmployeeId == request.EmployeeId)
            .ProjectTo<ContractDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
