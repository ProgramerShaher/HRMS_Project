using MediatR;

namespace HRMS.Application.Features.Core.Cities.Commands.UpdateCity;

public class UpdateCityCommand : IRequest<int>
{
    public int CityId { get; set; }
    public int CountryId { get; set; }
    public string CityNameAr { get; set; } = string.Empty;
    public string CityNameEn { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
