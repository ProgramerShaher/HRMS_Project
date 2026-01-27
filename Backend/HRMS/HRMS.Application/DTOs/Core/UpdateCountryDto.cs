namespace HRMS.Application.DTOs.Core;

/// <summary>
/// DTO لتحديث بيانات الدولة
/// </summary>
public class UpdateCountryDto
{
    public string CountryNameAr { get; set; } = string.Empty;
    public string CountryNameEn { get; set; } = string.Empty;
    public string? CitizenshipNameAr { get; set; }
    public string? IsoCode { get; set; }
    public bool IsActive { get; set; }
}
