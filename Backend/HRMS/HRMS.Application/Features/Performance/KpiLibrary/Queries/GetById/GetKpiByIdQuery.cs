using AutoMapper;
using FluentValidation;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.KpiLibrary.Queries.GetById;

public class GetKpiByIdQuery : IRequest<Result<KpiDto>>
{
    public int KpiId { get; set; }
}

public class GetKpiByIdQueryHandler : IRequestHandler<GetKpiByIdQuery, Result<KpiDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetKpiByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<KpiDto>> Handle(GetKpiByIdQuery request, CancellationToken cancellationToken)
    {
        var kpi = await _context.KpiLibraries
            .FirstOrDefaultAsync(k => k.KpiId == request.KpiId, cancellationToken);

        if (kpi == null)
            return Result<KpiDto>.Failure("مؤشر الأداء غير موجود");

        var dto = _mapper.Map<KpiDto>(kpi);

        return Result<KpiDto>.Success(dto);
    }
}

public class GetKpiByIdQueryValidator : AbstractValidator<GetKpiByIdQuery>
{
    public GetKpiByIdQueryValidator()
    {
        RuleFor(x => x.KpiId)
            .GreaterThan(0).WithMessage("معرف مؤشر الأداء مطلوب");
    }
}
