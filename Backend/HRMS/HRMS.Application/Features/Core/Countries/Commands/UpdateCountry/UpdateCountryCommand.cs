using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Countries.Commands.UpdateCountry;

/// <summary>
/// أمر تحديث بيانات دولة
/// </summary>
public class UpdateCountryCommand : IRequest<int>
{
    public int CountryId { get; set; }
    public string CountryNameAr { get; set; } = string.Empty;
    public string CountryNameEn { get; set; } = string.Empty;
    public string? CitizenshipNameAr { get; set; }
    public string? CitizenshipNameEn { get; set; }
    public string? IsoCode { get; set; }
    public bool IsActive { get; set; }
}
