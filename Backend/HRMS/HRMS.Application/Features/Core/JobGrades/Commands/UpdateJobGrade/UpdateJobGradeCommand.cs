using MediatR;

namespace HRMS.Application.Features.Core.JobGrades.Commands.UpdateJobGrade;

public class UpdateJobGradeCommand : IRequest<int>
{
    public int JobGradeId { get; set; }
    public string GradeCode { get; set; } = string.Empty;
    public string GradeNameAr { get; set; } = string.Empty;
    public string GradeNameEn { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public string? BenefitsConfig { get; set; }
    public string? Description { get; set; }
}
