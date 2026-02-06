namespace HRMS.Application.DTOs.Recruitment;

/// <summary>
/// DTO لعرض مرشح
/// </summary>
public class CandidateDto
{
    public int CandidateId { get; set; }
    public string FirstNameAr { get; set; } = string.Empty;
    public string? FamilyNameAr { get; set; }
    public string FullNameEn { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int? NationalityId { get; set; }
    public string? NationalityName { get; set; }
    public string? Resume { get; set; }
    public string ApplicationSource { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// DTO لعرض وظيفة شاغرة
/// </summary>
public class JobVacancyDto
{
    public int VacancyId { get; set; }
    public string VacancyTitle { get; set; } = string.Empty;
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public int DeptId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int PositionsCount { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO لعرض طلب توظيف
/// </summary>
public class JobApplicationDto
{
    public int ApplicationId { get; set; }
    public int VacancyId { get; set; }
    public string VacancyTitle { get; set; } = string.Empty;
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
}

/// <summary>
/// DTO لعرض عرض عمل
/// </summary>
public class JobOfferDto
{
    public int OfferId { get; set; }
    public int ApplicationId { get; set; }
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public int VacancyId { get; set; }
    public string VacancyTitle { get; set; } = string.Empty;
    public DateTime OfferDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal? BasicSalary { get; set; }
    public decimal? HousingAllowance { get; set; }
    public decimal? TransportAllowance { get; set; }
    public decimal? TotalPackage { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? JoiningDate { get; set; }
}

/// <summary>
/// DTO لعرض مقابلة
/// </summary>
public class InterviewDto
{
    public int InterviewId { get; set; }
    public int ApplicationId { get; set; }
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public string? InterviewType { get; set; }
    public int? InterviewerId { get; set; }
    public string? InterviewerName { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? Score { get; set; }
    public string? Feedback { get; set; }
}
