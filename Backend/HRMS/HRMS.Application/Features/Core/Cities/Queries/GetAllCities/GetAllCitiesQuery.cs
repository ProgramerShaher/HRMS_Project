using MediatR;
using HRMS.Application.DTOs.Core;
using HRMS.Application.DTOs;

namespace HRMS.Application.Features.Core.Cities.Queries.GetAllCities;

public class GetAllCitiesQuery : IRequest<PagedResult<CityListDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? CountryId { get; set; }
    public bool? IsActive { get; set; }
}
