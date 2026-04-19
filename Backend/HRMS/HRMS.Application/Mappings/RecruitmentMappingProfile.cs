using AutoMapper;
using HRMS.Application.DTOs.Recruitment;
using HRMS.Core.Entities.Recruitment;

namespace HRMS.Application.Mappings;

/// <summary>
/// AutoMapper Profile للـ Recruitment Module
/// </summary>
public class RecruitmentMappingProfile : Profile
{
    public RecruitmentMappingProfile()
    {
        // ═══════════════════════════════════════════════════════════
        // CANDIDATES MAPPINGS
        // ═══════════════════════════════════════════════════════════
        
        CreateMap<Candidate, CandidateDto>()
            .ForMember(dest => dest.NationalityName, opt => opt.MapFrom(src => 
                src.Nationality != null ? src.Nationality.CountryNameAr : null))
            .ForMember(dest => dest.Resume, opt => opt.MapFrom(src => src.CvFilePath));

        // ═══════════════════════════════════════════════════════════
        // VACANCIES MAPPINGS
        // ═══════════════════════════════════════════════════════════
        
        CreateMap<JobVacancy, JobVacancyDto>()
            .ForMember(dest => dest.VacancyTitle, opt => opt.MapFrom(src => 
                src.Job != null ? src.Job.JobTitleAr : string.Empty))
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => 
                src.Job != null ? src.Job.JobTitleAr : string.Empty))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => 
                src.Department !=  null ? src.Department.DeptNameAr : string.Empty))
            .ForMember(dest => dest.PositionsCount, opt => opt.MapFrom(src => src.RequiredCount))
            .ForMember(dest => dest.PostedDate, opt => opt.MapFrom(src => src.PublishDate ?? DateTime.Now))
            // ✅ نطاق الراتب من الدرجة الوظيفية الافتراضية
            .ForMember(dest => dest.MinSalary, opt => opt.MapFrom(src => 
                src.Job != null && src.Job.DefaultGrade != null 
                    ? src.Job.DefaultGrade.MinSalary 
                    : (decimal?)null))
            .ForMember(dest => dest.MaxSalary, opt => opt.MapFrom(src => 
                src.Job != null && src.Job.DefaultGrade != null 
                    ? src.Job.DefaultGrade.MaxSalary 
                    : (decimal?)null))
            // ✅ وصف الوظيفة من JobDescription
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => 
                src.JobDescription ?? ""));

        // ═══════════════════════════════════════════════════════════
        // APPLICATIONS MAPPINGS
        // ═══════════════════════════════════════════════════════════
        
        CreateMap<JobApplication, JobApplicationDto>()
            .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.AppId))
            .ForMember(dest => dest.VacancyTitle, opt => opt.MapFrom(src => 
                src.Vacancy != null ? src.Vacancy.Job.JobTitleAr : string.Empty))
            .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => 
                src.Candidate != null ? src.Candidate.FullNameEn : string.Empty));

        // ═══════════════════════════════════════════════════════════
        // JOB OFFERS MAPPINGS
        // ═══════════════════════════════════════════════════════════
        
        CreateMap<JobOffer, JobOfferDto>()
            .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.AppId))
            .ForMember(dest => dest.CandidateId,   opt => opt.MapFrom(src =>
                src.Application != null ? src.Application.CandidateId : 0))
            .ForMember(dest => dest.VacancyId,     opt => opt.MapFrom(src =>
                src.Application != null ? src.Application.VacancyId : 0))
            .ForMember(dest => dest.CandidateName, opt => opt.MapFrom(src => 
                src.Application != null && src.Application.Candidate != null 
                    ? src.Application.Candidate.FullNameEn 
                    : string.Empty))
            .ForMember(dest => dest.VacancyTitle, opt => opt.MapFrom(src => 
                src.Application != null && src.Application.Vacancy != null 
                    ? src.Application.Vacancy.Job.JobTitleAr 
                    : string.Empty))
            .ForMember(dest => dest.TotalPackage, opt => opt.MapFrom(src => 
                (src.BasicSalary ?? 0) + (src.HousingAllowance ?? 0) + (src.TransportAllowance ?? 0)
                + (src.MedicalAllowance ?? 0) + (src.OtherAllowances ?? 0)));

        // ═══════════════════════════════════════════════════════════
        // INTERVIEWS MAPPINGS
        // ═══════════════════════════════════════════════════════════
        
        CreateMap<Interview, InterviewDto>()
            .ForMember(dest => dest.ApplicationId,  opt => opt.MapFrom(src => src.AppId))
            .ForMember(dest => dest.CandidateId,    opt => opt.MapFrom(src =>
                src.Application != null ? src.Application.CandidateId : 0))
            .ForMember(dest => dest.InterviewDate,  opt => opt.MapFrom(src => src.ScheduledTime))
            .ForMember(dest => dest.Status,         opt => opt.MapFrom(src => src.Status ?? "SCHEDULED"))
            .ForMember(dest => dest.Score,          opt => opt.MapFrom(src => (decimal?)src.Rating))
            .ForMember(dest => dest.Feedback,       opt => opt.MapFrom(src => src.ResultNotes))
            .ForMember(dest => dest.CandidateName,  opt => opt.MapFrom(src => 
                src.Application != null && src.Application.Candidate != null 
                    ? src.Application.Candidate.FullNameEn 
                    : string.Empty))
            .ForMember(dest => dest.InterviewerName, opt => opt.MapFrom(src => 
                src.Interviewer != null ? src.Interviewer.FullNameAr : null));
    }
}
