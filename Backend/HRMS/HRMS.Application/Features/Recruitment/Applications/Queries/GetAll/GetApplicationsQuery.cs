using AutoMapper;
using HRMS.Application.DTOs.Recruitment;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Applications.Queries.GetAll;

/// <summary>
/// استعلام للحصول على قائمة طلبات التوظيف
/// </summary>
public class GetApplicationsQuery : IRequest<Result<List<JobApplicationDto>>>
{
    /// <summary>
    /// تصفية حسب الوظيفة الشاغرة
    /// </summary>
    public int? VacancyId { get; set; }

    /// <summary>
    /// تصفية حسب المرشح
    /// </summary>
    public int? CandidateId { get; set; }

    /// <summary>
    /// تصفية حسب الحالة
    /// </summary>
    public string? Status { get; set; }
}

public class GetApplicationsQueryHandler : IRequestHandler<GetApplicationsQuery, Result<List<JobApplicationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetApplicationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<JobApplicationDto>>> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.JobApplications
            .Include(a => a.Candidate)
            .Include(a => a.Vacancy)
                .ThenInclude(v => v.Job)
            .AsQueryable();

        // تطبيق الفلاتر
        if (request.VacancyId.HasValue)
        {
            query = query.Where(a => a.VacancyId == request.VacancyId.Value);
        }

        if (request.CandidateId.HasValue)
        {
            query = query.Where(a => a.CandidateId == request.CandidateId.Value);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(a => a.Status == request.Status);
        }

        var applications = await query
            .OrderByDescending(a => a.ApplicationDate)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<JobApplicationDto>>(applications);

        return Result<List<JobApplicationDto>>.Success(dtos);
    }
}
