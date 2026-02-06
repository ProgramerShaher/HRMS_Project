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
        // MASTER DATA MAPPINGS
        // ═══════════════════════════════════════════════════════════
        
        CreateMap<ViolationType, ViolationTypeDto>();
        CreateMap<DisciplinaryAction, DisciplinaryActionDto>();
        CreateMap<KpiLibrary, KpiDto>()
            .ForMember(dest => dest.KpiId, opt => opt.MapFrom(src => src.KpiId))
            .ForMember(dest => dest.KpiNameAr, opt => opt.MapFrom(src => src.KpiNameAr))
            .ForMember(dest => dest.KpiNameEn, opt => opt.Ignore()) // لا يوجد في Entity
            .ForMember(dest => dest.DefaultWeight, opt => opt.Ignore()) // لا يوجد في Entity
            .ForMember(dest => dest.MeasurementCriteria, opt => opt.MapFrom(src => src.KpiDescription));

        CreateMap<AppraisalCycle, AppraisalCycleDto>();
    }
}
