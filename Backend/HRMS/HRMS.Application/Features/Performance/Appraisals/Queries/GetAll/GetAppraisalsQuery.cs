using AutoMapper;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Appraisals.Queries.GetAll;

public class GetAppraisalsQuery : IRequest<Result<List<EmployeeAppraisalDto>>>
{
    public int? EmployeeId { get; set; }
    public int? CycleId { get; set; }
}

public class GetAppraisalsQueryHandler : IRequestHandler<GetAppraisalsQuery, Result<List<EmployeeAppraisalDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAppraisalsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<EmployeeAppraisalDto>>> Handle(GetAppraisalsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.EmployeeAppraisals
            .Include(a => a.Employee)
            .Include(a => a.Cycle)
            .Include(a => a.Employee)
            .Where(a => a.IsDeleted == 0)
            .AsQueryable();

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(a => a.EmployeeId == request.EmployeeId);
        }

        if (request.CycleId.HasValue)
        {
            query = query.Where(a => a.CycleId == request.CycleId);
        }

        var appraisals = await query
            .OrderByDescending(a => a.AppraisalDate)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<EmployeeAppraisalDto>>(appraisals);

        return Result<List<EmployeeAppraisalDto>>.Success(dtos);
    }
}
