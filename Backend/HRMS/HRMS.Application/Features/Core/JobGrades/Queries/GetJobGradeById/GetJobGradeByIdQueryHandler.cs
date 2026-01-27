using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.JobGrades.Queries.GetJobGradeById;

public class GetJobGradeByIdQueryHandler : IRequestHandler<GetJobGradeByIdQuery, JobGradeDto?>
{
    private readonly IApplicationDbContext _context;

    public GetJobGradeByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<JobGradeDto?> Handle(GetJobGradeByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.JobGrades
            .Where(g => g.JobGradeId == request.JobGradeId && g.IsDeleted == 0)
            .Select(g => new JobGradeDto
            {
                JobGradeId = g.JobGradeId,
                GradeCode = g.GradeCode,
                GradeNameAr = g.GradeNameAr,
                GradeNameEn = g.GradeNameEn,
                GradeLevel = g.GradeLevel,
                MinSalary = g.MinSalary,
                MaxSalary = g.MaxSalary,
                BenefitsConfig = g.BenefitsConfig,
                Description = g.Description,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
