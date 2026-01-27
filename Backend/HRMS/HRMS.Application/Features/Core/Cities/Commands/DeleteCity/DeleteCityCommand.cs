using MediatR;

namespace HRMS.Application.Features.Core.Cities.Commands.DeleteCity;

public class DeleteCityCommand : IRequest<bool>
{
    public int CityId { get; set; }

    public DeleteCityCommand(int cityId)
    {
        CityId = cityId;
    }
}
