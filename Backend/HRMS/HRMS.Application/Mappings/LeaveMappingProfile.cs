using AutoMapper;
using HRMS.Application.DTOs.Core;
using HRMS.Application.DTOs.Leaves;
using HRMS.Core.Entities.Core;
using HRMS.Core.Entities.Leaves;

namespace HRMS.Application.Mappings;

public class LeaveMappingProfile : Profile
{
    public LeaveMappingProfile()
    {
        CreateMap<LeaveType, LeaveTypeDto>().ReverseMap();
        CreateMap<PublicHoliday, PublicHolidayDto>().ReverseMap();
        CreateMap<SystemSetting, SystemSettingDto>().ReverseMap();
        CreateMap<LeaveRequest, LeaveRequestDto>()
            .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.LeaveNameAr)); // Ensure LeaveTypeName is mapped
        CreateMap<LeaveRequestDto, LeaveRequest>();
        
        CreateMap<EmployeeLeaveBalance, LeaveBalanceDto>()
            .ForMember(dest => dest.LeaveTypeName, opt => opt.MapFrom(src => src.LeaveType.LeaveNameAr))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstNameAr + " " + src.Employee.LastNameAr));
    }
}
