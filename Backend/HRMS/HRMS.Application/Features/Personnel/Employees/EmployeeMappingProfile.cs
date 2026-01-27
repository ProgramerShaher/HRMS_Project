using AutoMapper;
using HRMS.Core.Entities.Personnel;
using HRMS.Application.DTOs.Personnel;

namespace HRMS.Application.Features.Personnel.Employees;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        // Create -> Sub-Entities
        CreateMap<CreateEmployeeDto, Employee>();
        CreateMap<CreateEmployeeDto, EmployeeCompensation>();

        // Employee -> Profile Dto
        CreateMap<Employee, EmployeeProfileDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DeptNameAr))
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.JobTitleAr));

        CreateMap<EmployeeCompensation, EmployeeCompensationDto>();
        CreateMap<EmployeeDocument, EmployeeDocumentDto>()
            .ForMember(dest => dest.DocumentTypeName, opt => opt.MapFrom(src => src.DocumentType.DocumentTypeNameAr));
    }
}
