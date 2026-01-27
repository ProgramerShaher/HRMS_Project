using MediatR;

namespace HRMS.Application.Features.Core.JobGrades.Commands.CreateJobGrade;

/// <summary>
/// أمر إنشاء درجة وظيفية جديدة
/// </summary>
public class CreateJobGradeCommand : IRequest<int>
{
    public string GradeCode { get; set; } = string.Empty;
    public string GradeNameAr { get; set; } = string.Empty;
    public string GradeNameEn { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public string? BenefitsConfig { get; set; }
    public string? Description { get; set; }
}
