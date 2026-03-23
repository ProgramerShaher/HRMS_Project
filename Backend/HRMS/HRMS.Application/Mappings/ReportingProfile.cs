using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HRMS.Application.DTOs.Reports.Analytics;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Application.Mappings
{
    public class ReportingProfile : Profile
    {
        public ReportingProfile()
        {
			// 1. Mapping for EmployeeCensusDto
			CreateMap<Employee, EmployeeCensusDto>()
				.ForMember(dest => dest.FullNameAr, opt => opt.MapFrom(src => $"{src.FirstNameAr} {src.LastNameAr}"))
				.ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department != null ? src.Department.DeptNameAr : "-"))
				.ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job != null ? src.Job.JobTitleAr : "-"))
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => (src.IsActive && src.TerminationDate == null) ? "نشط" : "منتهي"))
				.ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.NationalityId != null ? src.NationalityId.ToString() : "-"))
				.ForMember(dest => dest.BasicSalary, opt => opt.MapFrom(src => src.Compensation != null ? src.Compensation.BasicSalary : 0));

			// 2. Mapping for DailyAttendanceDetailsDto
			CreateMap<DailyAttendance, DailyAttendanceDetailsDto>()
				.ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee.FirstNameAr} {src.Employee.LastNameAr}"))
				.ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Employee.Department != null ? src.Employee.Department.DeptNameAr : "-"))
				.ForMember(dest => dest.PlannedShift, opt => opt.MapFrom(src => src.PlannedShift != null ? src.PlannedShift.ShiftNameAr : "-"))
				.ForMember(dest => dest.InTime, opt => opt.MapFrom(src => src.ActualInTime.HasValue ? src.ActualInTime.Value.ToString("HH:mm") : "-"))
				.ForMember(dest => dest.OutTime, opt => opt.MapFrom(src => src.ActualOutTime.HasValue ? src.ActualOutTime.Value.ToString("HH:mm") : "-"));
		}
    }
}
