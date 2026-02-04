using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Dependents;

public class GetEmployeeDependentsQuery : IRequest<Result<List<DependentDto>>>
{
    public int EmployeeId { get; set; }
}

public class GetEmployeeDependentsQueryHandler : IRequestHandler<GetEmployeeDependentsQuery, Result<List<DependentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeDependentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<DependentDto>>> Handle(GetEmployeeDependentsQuery request, CancellationToken cancellationToken)
    {
        var dependents = await _context.Dependents
            .Where(d => d.EmployeeId == request.EmployeeId)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<DependentDto>>(dependents);
        return Result<List<DependentDto>>.Success(dtos);
    }
}
