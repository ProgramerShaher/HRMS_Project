using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Core;

namespace HRMS.Application.Features.Core.JobGrades.Commands.CreateJobGrade;

/// <summary>
/// معالج أمر إنشاء درجة وظيفية جديدة
/// </summary>
public class CreateJobGradeCommandHandler : IRequestHandler<CreateJobGradeCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateJobGradeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateJobGradeCommand request, CancellationToken cancellationToken)
    {
        var jobGrade = new JobGrade
        {
            GradeCode = request.GradeCode,
            GradeNameAr = request.GradeNameAr,
            GradeNameEn = request.GradeNameEn,
            GradeLevel = request.GradeLevel,
            MinSalary = request.MinSalary,
            MaxSalary = request.MaxSalary,
            BenefitsConfig = request.BenefitsConfig,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.JobGrades.Add(jobGrade);
        await _context.SaveChangesAsync(cancellationToken);

        return jobGrade.JobGradeId;
    }
}
