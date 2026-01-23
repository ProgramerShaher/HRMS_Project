using AutoMapper;
using HRMS.Application.DTOs.Core;
using HRMS.Core.Entities.Core;

namespace HRMS.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Country Mappings
            CreateMap<Country, CountryDto>();
            CreateMap<CreateCountryDto, Country>();
            CreateMap<UpdateCountryDto, Country>();

            // City Mappings
            CreateMap<City, CityDto>();
            CreateMap<CreateCityDto, City>();
            CreateMap<UpdateCityDto, City>();

            // Branch Mappings
            CreateMap<Branch, BranchDto>();
            CreateMap<CreateBranchDto, Branch>();
            CreateMap<UpdateBranchDto, Branch>();

            // Department Mappings
            CreateMap<Department, DepartmentDto>();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();

            // Job Mappings
            CreateMap<Job, JobDto>();
            CreateMap<CreateJobDto, Job>();
            CreateMap<UpdateJobDto, Job>();
        }
    }
}
