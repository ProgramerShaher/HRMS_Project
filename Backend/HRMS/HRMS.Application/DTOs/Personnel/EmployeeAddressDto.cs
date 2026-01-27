using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Personnel;

public class EmployeeAddressDto
{
    public int? AddressId { get; set; }

    [Required]
    [MaxLength(20)]
    public string AddressType { get; set; } = "Home"; // Home, Work

    public int? CityId { get; set; } // If linking specific city
    
    [MaxLength(100)]
    public string? Street { get; set; }

    [MaxLength(50)]
    public string? BuildingNumber { get; set; }

    [MaxLength(10)]
    public string? ZipCode { get; set; }
    
    [MaxLength(200)]
    public string? AdditionalDetails { get; set; }
}
