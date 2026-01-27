using AutoMapper;
using HRMS.Core.Entities.Core;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Jobs;

public class JobMappingProfile : Profile
{
    public JobMappingProfile()
    {
        // Entity -> DTO
        CreateMap<Job, JobDto>()
            .ForMember(dest => dest.GradeNameAr,
                opt => opt.MapFrom(src => src.DefaultGrade != null ? src.DefaultGrade.GradeNameAr : null));
        // Command -> Entity
        CreateMap<Commands.CreateJob.CreateJobCommand, Job>()
            .ForMember(dest => dest.JobId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}
