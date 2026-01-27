namespace HRMS.Application.DTOs.Core;

/// <summary>
/// DTO لعرض بيانات الدولة الكاملة
/// </summary>
public class CountryDto
{
    public int CountryId { get; set; }
    public string CountryNameAr { get; set; } = string.Empty;
    public string CountryNameEn { get; set; } = string.Empty;
    public string? CitizenshipNameAr { get; set; }
    public string? CitizenshipNameEn { get; set; }
    public string? IsoCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CitiesCount { get; set; } // عدد المدن التابعة
}
