using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Cities.Queries.GetCityById;

public class GetCityByIdQuery : IRequest<CityDto?>
{
    public int CityId { get; set; }

    public GetCityByIdQuery(int id)
    {
        CityId = id;
    }
}
