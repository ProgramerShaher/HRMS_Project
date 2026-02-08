using AutoMapper;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Violations.Queries.GetEmployeeViolations;

public class GetEmployeeViolationsQuery : IRequest<Result<List<EmployeeViolationDto>>>
{
    public int EmployeeId { get; set; }
}

public class GetEmployeeViolationsQueryHandler : IRequestHandler<GetEmployeeViolationsQuery, Result<List<EmployeeViolationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeViolationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<EmployeeViolationDto>>> Handle(GetEmployeeViolationsQuery request, CancellationToken cancellationToken)
    {
        var violations = await _context.EmployeeViolations
            .Include(v => v.ViolationType)
            .Include(v => v.Action)
            .Where(v => v.EmployeeId == request.EmployeeId && v.IsDeleted == 0)
            .OrderByDescending(v => v.ViolationDate)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<EmployeeViolationDto>>(violations);

        return Result<List<EmployeeViolationDto>>.Success(dtos);
    }
}
