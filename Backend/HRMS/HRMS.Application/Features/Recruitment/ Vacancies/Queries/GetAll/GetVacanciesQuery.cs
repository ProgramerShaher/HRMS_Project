//using AutoMapper;
//using HRMS.Application.DTOs.Recruitment;
//using HRMS.Application.Interfaces;
//using HRMS.Core.Utilities;
//using MediatR;
//using Microsoft.EntityFrameworkCore;

//namespace HRMS.Application.Features.Recruitment.Vacancies.Queries.GetAll;

///// <summary>
///// استعلام للحصول على قائمة الوظائف الشاغرة
///// </summary>
//public class GetVacanciesQuery : IRequest<Result<List<JobVacancyDto>>>
//{
//    /// <summary>
//    /// تصفية حسب الحالة (ACTIVE, CLOSED, null للكل)
//    /// </summary>
//    public string? Status { get; set; }
//}

//public class GetVacanciesQueryHandler : IRequestHandler<GetVacanciesQuery, Result<List<JobVacancyDto>>>
//{
//    private readonly IApplicationDbContext _context;
//    private readonly IMapper _mapper;

//    public GetVacanciesQueryHandler(IApplicationDbContext context, IMapper mapper)
//    {
//        _context = context;
//        _mapper = mapper;
//    }

//    public async Task<Result<List<JobVacancyDto>>> Handle(GetVacanciesQuery request, CancellationToken cancellationToken)
//    {
//        var query = _context.JobVacancies
//            .Include(v => v.Job)
//            .Include(v => v.Department)
//            .AsQueryable();

//        // تطبيق التصفية حسب الحالة
//        if (!string.IsNullOrEmpty(request.Status))
//        {
//            query = query.Where(v => v.Status == request.Status);
//        }

//        var vacancies = await query
//            .ToListAsync(cancellationToken);

//        var dtos = _mapper.Map<List<JobVacancyDto>>(vacancies);

//        return Result<List<JobVacancyDto>>.Success(dtos);
//    }
//}
