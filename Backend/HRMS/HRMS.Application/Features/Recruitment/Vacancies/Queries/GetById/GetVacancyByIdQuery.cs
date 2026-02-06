using AutoMapper;
using FluentValidation;
using HRMS.Application.DTOs.Recruitment;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Vacancies.Queries.GetById;

/// <summary>
/// استعلام للحصول على تفاصيل وظيفة شاغرة
/// </summary>
public class GetVacancyByIdQuery : IRequest<Result<JobVacancyDto>>
{
    public int VacancyId { get; set; }
}

public class GetVacancyByIdQueryHandler : IRequestHandler<GetVacancyByIdQuery, Result<JobVacancyDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetVacancyByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<JobVacancyDto>> Handle(GetVacancyByIdQuery request, CancellationToken cancellationToken)
    {
        var vacancy = await _context.JobVacancies
            .Include(v => v.Job)
            .Include(v => v.Department)
            .FirstOrDefaultAsync(v => v.VacancyId == request.VacancyId, cancellationToken);

        if (vacancy == null)
            return Result<JobVacancyDto>.Failure("الوظيفة الشاغرة غير موجودة");

        var dto = _mapper.Map<JobVacancyDto>(vacancy);

        return Result<JobVacancyDto>.Success(dto);
    }
}

public class GetVacancyByIdQueryValidator : AbstractValidator<GetVacancyByIdQuery>
{
    public GetVacancyByIdQueryValidator()
    {
        RuleFor(x => x.VacancyId)
            .GreaterThan(0).WithMessage("معرف الوظيفة الشاغرة مطلوب");
    }
}
