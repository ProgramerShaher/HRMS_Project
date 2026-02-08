using AutoMapper;
using HRMS.Application.DTOs.Payroll;
using HRMS.Application.DTOs.Payroll.Configuration;
using HRMS.Core.Entities.Payroll;

namespace HRMS.Application.Mappings;

public class PayrollConfigurationProfile : Profile
{
    public PayrollConfigurationProfile()
    {
        CreateMap<SalaryElement, SalaryElementDto>()
            .ForMember(dest => dest.IsTaxable, opt => opt.MapFrom(src => src.IsTaxable == 1))
            .ForMember(dest => dest.IsGosiBase, opt => opt.MapFrom(src => src.IsGosiBase == 1))
            .ForMember(dest => dest.IsRecurring, opt => opt.MapFrom(src => src.IsRecurring == 1))
            .ForMember(dest => dest.IsBasic, opt => opt.MapFrom(src => src.IsBasic == 1));

        CreateMap<SalaryElementDto, SalaryElement>()
            .ForMember(dest => dest.IsTaxable, opt => opt.MapFrom(src => src.IsTaxable ? (byte)1 : (byte)0))
            .ForMember(dest => dest.IsGosiBase, opt => opt.MapFrom(src => src.IsGosiBase ? (byte)1 : (byte)0))
            .ForMember(dest => dest.IsRecurring, opt => opt.MapFrom(src => src.IsRecurring ? (byte)1 : (byte)0))
            .ForMember(dest => dest.IsBasic, opt => opt.MapFrom(src => src.IsBasic ? (byte)1 : (byte)0));

        // Loan Mappings
        CreateMap<Loan, LoanDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullNameAr : null))
            .ForMember(dest => dest.ApprovedByName, opt => opt.Ignore())
            .ForMember(dest => dest.Installments, opt => opt.MapFrom(src => src.Installments))
            .ForMember(dest => dest.RemainingAmount, opt => opt.MapFrom(src => 
                src.Installments != null ? src.Installments.Where(i => i.Status == "UNPAID").Sum(i => i.Amount) : 0))
            .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(src => 
                src.Installments != null ? src.Installments.Where(i => i.Status == "PAID").Sum(i => i.Amount) : 0));

        // LoanInstallment Mappings
        CreateMap<LoanInstallment, LoanInstallmentDto>();
    }
}
