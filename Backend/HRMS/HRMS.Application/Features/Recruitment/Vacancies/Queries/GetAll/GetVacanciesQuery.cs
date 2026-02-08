using AutoMapper;
using HRMS.Application.DTOs.Recruitment;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Vacancies.Queries.GetAll;

/// <summary>
/// استعلام للحصول على قائمة الوظائف الشاغرة
/// </summary>
public class GetVacanciesQuery : IRequest<Result<List<JobVacancyDto>>>
{
    /// <summary>
    /// تصفية حسب حالة (ACTIVE, CLOSED, null للكل)
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// تصفية حسب القسم
    /// </summary>
    public int? DepartmentId { get; set; }
}

/// <summary>
/// معالج استعلام الوظائف الشاغرة
/// </summary>
public class GetVacanciesQueryHandler : IRequestHandler<GetVacanciesQuery, Result<List<JobVacancyDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetVacanciesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<JobVacancyDto>>> Handle(GetVacanciesQuery request, CancellationToken cancellationToken)
    {
        // جلب الشواغر مع تحميل Job و JobGrade (للحصول على نطاق الراتب)
        var query = _context.JobVacancies
            .Include(v => v.Job)
                .ThenInclude(j => j.DefaultGrade)  // ✅ تحميل JobGrade للحصول على MinSalary/MaxSalary
            .Include(v => v.Department)
            .Where(v => v.IsDeleted == 0);

        // تطبيق التصفية حسب الحالة
        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(v => v.Status == request.Status);
        }

        // تطبيق التصفية حسب القسم
        if (request.DepartmentId.HasValue)
        {
            query = query.Where(v => v.DeptId == request.DepartmentId.Value);
        }

        var vacancies = await query
            .OrderByDescending(v => v.PublishDate)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<JobVacancyDto>>(vacancies);

        return Result<List<JobVacancyDto>>.Success(dtos);
    }
}
