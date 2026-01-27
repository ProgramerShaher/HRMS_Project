namespace HRMS.Application.DTOs.Core;

public class JobGradeDto
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

public class JobGradeListDto
{
    public int JobGradeId { get; set; }
    public string GradeCode { get; set; } = string.Empty;
    public string GradeNameAr { get; set; } = string.Empty;
    public string GradeNameEn { get; set; } = string.Empty;
    public int GradeLevel { get; set; }
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
}
