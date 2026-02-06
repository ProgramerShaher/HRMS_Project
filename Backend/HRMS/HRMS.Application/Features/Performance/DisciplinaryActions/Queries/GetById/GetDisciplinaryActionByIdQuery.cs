using AutoMapper;
using FluentValidation;
using HRMS.Application.DTOs.Performance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.DisciplinaryActions.Queries.GetById;

public class GetDisciplinaryActionByIdQuery : IRequest<Result<DisciplinaryActionDto>>
{
    public int ActionId { get; set; }
}

public class GetDisciplinaryActionByIdQueryHandler : IRequestHandler<GetDisciplinaryActionByIdQuery, Result<DisciplinaryActionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDisciplinaryActionByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<DisciplinaryActionDto>> Handle(GetDisciplinaryActionByIdQuery request, CancellationToken cancellationToken)
    {
        var action = await _context.DisciplinaryActions
            .FirstOrDefaultAsync(a => a.ActionId == request.ActionId, cancellationToken);

        if (action == null)
            return Result<DisciplinaryActionDto>.Failure("الإجراء التأديبي غير موجود");

        var dto = _mapper.Map<DisciplinaryActionDto>(action);

        return Result<DisciplinaryActionDto>.Success(dto);
    }
}

public class GetDisciplinaryActionByIdQueryValidator : AbstractValidator<GetDisciplinaryActionByIdQuery>
{
    public GetDisciplinaryActionByIdQueryValidator()
    {
        RuleFor(x => x.ActionId)
            .GreaterThan(0).WithMessage("معرف الإجراء مطلوب");
    }
}
