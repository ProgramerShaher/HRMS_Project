namespace HRMS.Application.DTOs.Core;

/// <summary>
/// DTO لعرض قائمة الدول (مبسط للأداء)
/// </summary>
public class CountryListDto
{
    public int CountryId { get; set; }
    public string? CitizenshipNameAr { get; set; }
    public string? CitizenshipNameEn { get; set; }
    public string CountryNameAr { get; set; } = string.Empty;
    public string CountryNameEn { get; set; } = string.Empty;
    public string? IsoCode { get; set; }
    public bool IsActive { get; set; }
    public int CitiesCount { get; set; }
}
