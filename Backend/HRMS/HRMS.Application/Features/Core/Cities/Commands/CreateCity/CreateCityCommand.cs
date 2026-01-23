using MediatR;

namespace HRMS.Application.Features.Core.Cities.Commands.CreateCity
{
    public class CreateCityCommand : IRequest<int>
    {
        public string CityNameAr { get; set; }
        public string CityNameEn { get; set; }
        public int CountryId { get; set; }
    }
}
