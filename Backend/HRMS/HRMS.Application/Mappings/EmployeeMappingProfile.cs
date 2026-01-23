using AutoMapper;
using HRMS.Application.Features.Personnel.Employees.DTOs;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Application.Mappings
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            // Entity -> DTO
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.NationalityName, opt => opt.MapFrom(src => src.Nationality != null ? src.Nationality.CountryNameAr : ""))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job != null ? src.Job.JobTitleAr : ""))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DeptNameAr : ""));

            // DTO -> Entity (For Creation)
            CreateMap<CreateEmployeeDto, Employee>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "ACTIVE"));
        }
    }
}
