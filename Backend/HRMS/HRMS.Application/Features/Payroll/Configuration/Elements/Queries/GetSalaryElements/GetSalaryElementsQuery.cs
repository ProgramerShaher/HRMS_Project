using AutoMapper;
using HRMS.Application.DTOs.Payroll.Configuration;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Configuration.Elements.Queries.GetSalaryElements;

public class GetSalaryElementsQuery : IRequest<Result<List<SalaryElementDto>>>
{
}

public class GetSalaryElementsQueryHandler : IRequestHandler<GetSalaryElementsQuery, Result<List<SalaryElementDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSalaryElementsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<SalaryElementDto>>> Handle(GetSalaryElementsQuery request, CancellationToken cancellationToken)
    {
        var elements = await _context.SalaryElements
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<SalaryElementDto>>(elements);

        return Result<List<SalaryElementDto>>.Success(dtos);
    }
}
