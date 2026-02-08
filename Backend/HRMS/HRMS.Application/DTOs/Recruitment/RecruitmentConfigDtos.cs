namespace HRMS.Application.DTOs.Recruitment;

/// <summary>
/// DTO للدرجة الوظيفية
/// </summary>
public class JobGradeDto
{
    public int JobGradeId { get; set; }
    public string GradeNameAr { get; set; } = string.Empty;
    public string GradeNameEn { get; set; } = string.Empty;
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
}

/// <summary>
/// DTO عام للبيانات المرجعية (Lookup)
/// </summary>
public class LookupDto
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
}
