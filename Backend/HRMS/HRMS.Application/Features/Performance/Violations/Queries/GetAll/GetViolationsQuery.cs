using AutoMapper;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Violations.Queries.GetAll;

public class GetViolationsQuery : IRequest<Result<List<EmployeeViolationDto>>>
{
    public int? EmployeeId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class GetViolationsQueryHandler : IRequestHandler<GetViolationsQuery, Result<List<EmployeeViolationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetViolationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<EmployeeViolationDto>>> Handle(GetViolationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.EmployeeViolations
            .Include(v => v.ViolationType)
            .Include(v => v.Action)
            .Include(v => v.Employee)
            .Where(v => v.IsDeleted == 0)
            .AsQueryable();

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(v => v.EmployeeId == request.EmployeeId);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(v => v.Status == request.Status);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(v => v.ViolationDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(v => v.ViolationDate <= request.ToDate.Value);
        }

        var violations = await query
            .OrderByDescending(v => v.ViolationDate)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<EmployeeViolationDto>>(violations);

        return Result<List<EmployeeViolationDto>>.Success(dtos);
    }
}
