using MediatR;

namespace HRMS.Application.Features.Core.Countries.Commands.CreateCountry
{
    public class CreateCountryCommand : IRequest<int>
    {
        public string CountryNameAr { get; set; }
        public string CountryNameEn { get; set; }
        public string CitizenshipNameAr { get; set; }
        public string IsoCode { get; set; }
    }
}
