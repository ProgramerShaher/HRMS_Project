namespace HRMS.Application.DTOs.Core;

public class UpdateCityDto
{
    public int CountryId { get; set; }
    public string CityNameAr { get; set; } = string.Empty;
    public string CityNameEn { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
