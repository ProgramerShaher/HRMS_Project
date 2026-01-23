namespace HRMS.Application.DTOs.Core
{
    public class CountryDto
    {
        public int CountryId { get; set; }
        public string? CountryNameAr { get; set; }
        public string? CountryNameEn { get; set; }
        public string? CitizenshipNameAr { get; set; }
        public string? IsoCode { get; set; }
    }

    public class CreateCountryDto
    {
        public required string CountryNameAr { get; set; }
        public required string CountryNameEn { get; set; }
        public string? CitizenshipNameAr { get; set; }
        public string? IsoCode { get; set; }
    }

    public class UpdateCountryDto
    {
        public required string CountryNameAr { get; set; }
        public required string CountryNameEn { get; set; }
        public string? CitizenshipNameAr { get; set; }
        public string? IsoCode { get; set; }
    }
}
