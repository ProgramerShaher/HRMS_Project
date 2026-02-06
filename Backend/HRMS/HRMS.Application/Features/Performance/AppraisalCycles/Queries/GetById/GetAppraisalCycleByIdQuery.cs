using AutoMapper;
using FluentValidation;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.AppraisalCycles.Queries.GetById;

public class GetAppraisalCycleByIdQuery : IRequest<Result<AppraisalCycleDto>>
{
    public int CycleId { get; set; }
}

public class GetAppraisalCycleByIdQueryHandler : IRequestHandler<GetAppraisalCycleByIdQuery, Result<AppraisalCycleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAppraisalCycleByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<AppraisalCycleDto>> Handle(GetAppraisalCycleByIdQuery request, CancellationToken cancellationToken)
    {
        var cycle = await _context.AppraisalCycles
            .FirstOrDefaultAsync(c => c.CycleId == request.CycleId, cancellationToken);

        if (cycle == null)
            return Result<AppraisalCycleDto>.Failure("فترة التقييم غير موجودة");

        var dto = _mapper.Map<AppraisalCycleDto>(cycle);

        return Result<AppraisalCycleDto>.Success(dto);
    }
}

public class GetAppraisalCycleByIdQueryValidator : AbstractValidator<GetAppraisalCycleByIdQuery>
{
    public GetAppraisalCycleByIdQueryValidator()
    {
        RuleFor(x => x.CycleId)
            .GreaterThan(0).WithMessage("معرف فترة التقييم مطلوب");
    }
}
