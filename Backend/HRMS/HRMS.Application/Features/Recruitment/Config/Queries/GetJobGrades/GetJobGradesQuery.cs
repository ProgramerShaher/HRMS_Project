using HRMS.Application.DTOs.Recruitment;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Config.Queries.GetJobGrades;

/// <summary>
/// استعلام للحصول على جميع الدرجات الوظيفية
/// </summary>
public class GetJobGradesQuery : IRequest<Result<List<JobGradeDto>>>
{
}

/// <summary>
/// معالج استعلام الدرجات الوظيفية
/// </summary>
public class GetJobGradesQueryHandler : IRequestHandler<GetJobGradesQuery, Result<List<JobGradeDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetJobGradesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<JobGradeDto>>> Handle(GetJobGradesQuery request, CancellationToken cancellationToken)
    {
        var jobGrades = await _context.JobGrades
            .Where(g => g.IsDeleted == 0)
            .OrderBy(g => g.MinSalary)
            .Select(g => new JobGradeDto
            {
                JobGradeId = g.JobGradeId,
                GradeNameAr = g.GradeNameAr,
                GradeNameEn = g.GradeNameEn ?? "",
                MinSalary = g.MinSalary,
                MaxSalary = g.MaxSalary
            })
            .ToListAsync(cancellationToken);

        return Result<List<JobGradeDto>>.Success(jobGrades ?? new List<JobGradeDto>());
    }
}
