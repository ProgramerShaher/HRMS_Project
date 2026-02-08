using AutoMapper;
using HRMS.Application.DTOs.Performance;
using HRMS.Core.Entities.Performance;

namespace HRMS.Application.Mappings;

/// <summary>
/// AutoMapper Profile للـ Performance Module
/// </summary>
public class PerformanceMappingProfile : Profile
{
    public PerformanceMappingProfile()
    {
        // ═══════════════════════════════════════════════════════════
        // VIOLATIONS MAPPINGS
        // ═══════════════════════════════════════════════════════════
        
        CreateMap<EmployeeViolation, EmployeeViolationDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => 
                src.Employee != null ? src.Employee.FullNameAr : string.Empty))
            .ForMember(dest => dest.ViolationTypeNameAr, opt => opt.MapFrom(src => 
                src.ViolationType != null ? src.ViolationType.ViolationNameAr : string.Empty))
            .ForMember(dest => dest.ActionNameAr, opt => opt.MapFrom(src => 
                src.Action != null ? src.Action.ActionNameAr : string.Empty))
            .ForMember(dest => dest.DeductionDays, opt => opt.MapFrom(src => 
                src.Action != null ? src.Action.DeductionDays : 0))
            .ForMember(dest => dest.IsExecuted, opt => opt.MapFrom(src => src.IsExecuted == 1));

        // ═══════════════════════════════════════════════════════════
        // APPRAISALS MAPPINGS
        // ═══════════════════════════════════════════════════════════
        
        CreateMap<EmployeeAppraisal, EmployeeAppraisalDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => 
                src.Employee != null ? src.Employee.FullNameAr : string.Empty))
            .ForMember(dest => dest.CycleName, opt => opt.MapFrom(src => 
                src.Cycle != null ? src.Cycle.CycleNameAr : string.Empty))
            .ForMember(dest => dest.EvaluatorName, opt => opt.MapFrom(src => 
                src.Evaluator != null ? src.Evaluator.FullNameAr : string.Empty))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));

        CreateMap<AppraisalDetail, AppraisalDetailDto>()
            .ForMember(dest => dest.KpiName, opt => opt.MapFrom(src => 
                src.Kpi != null ? src.Kpi.KpiNameAr : string.Empty))
            .ForMember(dest => dest.Weight, opt => opt.Ignore()); // لا يوجد Weight في KPI entity

        // ═══════════════════════════════════════════════════════════
        // MASTER DATA MAPPINGS - WITH DEFAULT VALUES (NO NULLS)
        // ═══════════════════════════════════════════════════════════
        
        // ViolationType Mapping
        CreateMap<ViolationType, ViolationTypeDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.SeverityLevel, opt => opt.MapFrom(src => src.SeverityLevel));
        
        // DisciplinaryAction Mapping
        CreateMap<DisciplinaryAction, DisciplinaryActionDto>()
            .ForMember(dest => dest.DeductionDays, opt => opt.MapFrom(src => src.DeductionDays))
            .ForMember(dest => dest.IsTermination, opt => opt.MapFrom(src => src.IsTermination == 1));
        
        // KPI Mapping - Direct mapping since DTO matches Entity
        CreateMap<KpiLibrary, KpiDto>()
            .ForMember(dest => dest.KpiDescription, opt => opt.MapFrom(src => src.KpiDescription ?? ""))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category ?? ""))
            .ForMember(dest => dest.MeasurementUnit, opt => opt.MapFrom(src => src.MeasurementUnit ?? ""));

        // AppraisalCycle Mapping
        CreateMap<AppraisalCycle, AppraisalCycleDto>()
            .ForMember(dest => dest.CycleName, opt => opt.MapFrom(src => src.CycleNameAr))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 
                src.IsActive == 1 ? "ACTIVE" : "INACTIVE")); // تحويل IsActive إلى Status
    }
}
