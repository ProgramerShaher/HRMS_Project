using AutoMapper;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.ViolationTypes.Queries.GetAll;

/// <summary>
/// استعلام للحصول على جميع أنواع المخالفات
/// </summary>
public class GetViolationTypesQuery : IRequest<Result<List<ViolationTypeDto>>>
{
}

/// <summary>
/// معالج الاستعلام
/// </summary>
public class GetViolationTypesQueryHandler : IRequestHandler<GetViolationTypesQuery, Result<List<ViolationTypeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetViolationTypesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<ViolationTypeDto>>> Handle(GetViolationTypesQuery request, CancellationToken cancellationToken)
    {
        var violationTypes = await _context.ViolationTypes
            .OrderBy(v => v.ViolationNameAr)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<ViolationTypeDto>>(violationTypes);

        return Result<List<ViolationTypeDto>>.Success(dtos);
    }
}
