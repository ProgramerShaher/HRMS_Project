namespace HRMS.Application.DTOs.Core;

/// <summary>
/// DTO لعرض بيانات المدينة الكاملة
/// </summary>
public class CityDto
{
    public int CityId { get; set; }
    public int CountryId { get; set; }
    public string CityNameAr { get; set; } = string.Empty;
    public string CityNameEn { get; set; } = string.Empty;
    public string CountryNameAr { get; set; } = string.Empty;
    public string CountryNameEn { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
