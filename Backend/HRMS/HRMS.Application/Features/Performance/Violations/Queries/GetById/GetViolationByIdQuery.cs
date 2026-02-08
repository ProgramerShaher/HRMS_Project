using AutoMapper;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Violations.Queries.GetById;

public class GetViolationByIdQuery : IRequest<Result<EmployeeViolationDto>>
{
    public int ViolationId { get; set; }

    public GetViolationByIdQuery(int id)
    {
        ViolationId = id;
    }
}

public class GetViolationByIdQueryHandler : IRequestHandler<GetViolationByIdQuery, Result<EmployeeViolationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetViolationByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<EmployeeViolationDto>> Handle(GetViolationByIdQuery request, CancellationToken cancellationToken)
    {
        var violation = await _context.EmployeeViolations
            .Include(v => v.Employee)
            .Include(v => v.ViolationType)
            .Include(v => v.Action)
            .FirstOrDefaultAsync(v => v.ViolationId == request.ViolationId && v.IsDeleted == 0, cancellationToken);

        if (violation == null)
            return Result<EmployeeViolationDto>.Failure("المخالفة غير موجودة");

        var dto = _mapper.Map<EmployeeViolationDto>(violation);

        return Result<EmployeeViolationDto>.Success(dto);
    }
}
