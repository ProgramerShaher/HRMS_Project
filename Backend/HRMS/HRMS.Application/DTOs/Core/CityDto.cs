namespace HRMS.Application.DTOs.Core
{
    public class CityDto
    {
        public int CityId { get; set; }
        public int CountryId { get; set; }
        public string? CityNameAr { get; set; }
        public string? CityNameEn { get; set; }
    }

    public class CreateCityDto
    {
        public int CountryId { get; set; }
        public required string CityNameAr { get; set; }
        public required string CityNameEn { get; set; }
    }

    public class UpdateCityDto
    {
        public int CountryId { get; set; }
        public required string CityNameAr { get; set; }
        public required string CityNameEn { get; set; }
    }
}
