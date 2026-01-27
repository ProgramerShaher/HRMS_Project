using AutoMapper;
using HRMS.Core.Entities.Core;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Departments;

/// <summary>
/// ملف تعريف التحويل (Mapping) لوحدة الأقسام
/// </summary>
public class DepartmentMappingProfile : Profile
{
    public DepartmentMappingProfile()
    {
        // Entity -> DTO
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.ParentDeptNameAr, 
                opt => opt.MapFrom(src => src.ParentDepartment != null ? src.ParentDepartment.DeptNameAr : null));

        // Command -> Entity
        CreateMap<Commands.CreateDepartment.CreateDepartmentCommand, Department>()
            .ForMember(dest => dest.DeptId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => (byte)1));
    }
}
