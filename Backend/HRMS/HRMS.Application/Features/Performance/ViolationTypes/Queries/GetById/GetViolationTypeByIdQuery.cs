using AutoMapper;
using FluentValidation;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.ViolationTypes.Queries.GetById;

/// <summary>
/// استعلام للحصول على نوع مخالفة بمعرفه
/// </summary>
public class GetViolationTypeByIdQuery : IRequest<Result<ViolationTypeDto>>
{
    public int ViolationTypeId { get; set; }
}

/// <summary>
/// معالج الاستعلام
/// </summary>
public class GetViolationTypeByIdQueryHandler : IRequestHandler<GetViolationTypeByIdQuery, Result<ViolationTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetViolationTypeByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<ViolationTypeDto>> Handle(GetViolationTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var violationType = await _context.ViolationTypes
            .FirstOrDefaultAsync(v => v.ViolationTypeId == request.ViolationTypeId, cancellationToken);

        if (violationType == null)
            return Result<ViolationTypeDto>.Failure("نوع المخالفة غير موجود");

        var dto = _mapper.Map<ViolationTypeDto>(violationType);

        return Result<ViolationTypeDto>.Success(dto);
    }
}

/// <summary>
/// قواعد التحقق
/// </summary>
public class GetViolationTypeByIdQueryValidator : AbstractValidator<GetViolationTypeByIdQuery>
{
    public GetViolationTypeByIdQueryValidator()
    {
        RuleFor(x => x.ViolationTypeId)
            .GreaterThan(0).WithMessage("معرف نوع المخالفة مطلوب");
    }
}
