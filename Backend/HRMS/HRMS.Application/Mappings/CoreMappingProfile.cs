using AutoMapper;

using HRMS.Application.Features.Core.Countries.Commands.CreateCountry;
using HRMS.Application.Features.Core.Cities.Commands.CreateCity;
using HRMS.Application.Features.Core.Departments.Commands.CreateDepartment;
using HRMS.Application.Features.Core.Jobs.Commands.CreateJob;
using HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee;
using HRMS.Application.Features.Personnel.Employees.DTOs;
using HRMS.Core.Entities.Core;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Application.Mappings
{
    public class CoreMappingProfile : Profile
    {
        public CoreMappingProfile()
        {
            // Core Entities

            CreateMap<CreateCountryCommand, Country>();
            CreateMap<CreateCityCommand, City>();
            CreateMap<CreateDepartmentCommand, Department>();
            CreateMap<CreateJobCommand, Job>();
            
            // Personnel - Employee
            CreateMap<CreateEmployeeCommand, Employee>();
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.NationalityName, opt => opt.MapFrom(src => src.Nationality != null ? src.Nationality.CountryNameAr : ""))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job != null ? src.Job.JobTitleAr : ""))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DeptNameAr : ""));
        }
    }
}
