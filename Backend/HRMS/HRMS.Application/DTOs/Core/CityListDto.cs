namespace HRMS.Application.DTOs.Core;

public class CityListDto
{
    public int CityId { get; set; }
    public string CityNameAr { get; set; } = string.Empty;
    public string CityNameEn { get; set; } = string.Empty;
    public string CountryNameAr { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
