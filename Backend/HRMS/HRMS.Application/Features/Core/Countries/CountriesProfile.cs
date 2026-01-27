using AutoMapper;
using HRMS.Core.Entities.Core;
using HRMS.Application.DTOs.Core;
using HRMS.Application.Features.Core.Countries.Commands.CreateCountry;
using HRMS.Application.Features.Core.Countries.Commands.UpdateCountry;

namespace HRMS.Application.Features.Core.Countries;

public class CountriesProfile : Profile
{
    public CountriesProfile()
    {
        // Entity -> DTOs
        CreateMap<Country, CountryDto>();
        CreateMap<Country, CountryListDto>();

        // Commands -> Entity
        CreateMap<CreateCountryCommand, Country>()
            .ForMember(dest => dest.CountryId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        CreateMap<UpdateCountryCommand, Country>()
            .ForMember(dest => dest.CountryId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}
