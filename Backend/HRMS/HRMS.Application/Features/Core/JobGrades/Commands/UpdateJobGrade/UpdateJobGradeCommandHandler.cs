using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;

namespace HRMS.Application.Features.Core.JobGrades.Commands.UpdateJobGrade;

public class UpdateJobGradeCommandHandler : IRequestHandler<UpdateJobGradeCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateJobGradeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateJobGradeCommand request, CancellationToken cancellationToken)
    {
        var jobGrade = await _context.JobGrades
            .FirstOrDefaultAsync(g => g.JobGradeId == request.JobGradeId, cancellationToken);

        if (jobGrade == null)
            throw new KeyNotFoundException($"الدرجة الوظيفية برقم {request.JobGradeId} غير موجودة");

        jobGrade.GradeCode = request.GradeCode;
        jobGrade.GradeNameAr = request.GradeNameAr;
        jobGrade.GradeNameEn = request.GradeNameEn;
        jobGrade.GradeLevel = request.GradeLevel;
        jobGrade.MinSalary = request.MinSalary;
        jobGrade.MaxSalary = request.MaxSalary;
        jobGrade.BenefitsConfig = request.BenefitsConfig;
        jobGrade.Description = request.Description;
        jobGrade.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return jobGrade.JobGradeId;
    }
}
