using AutoMapper;
using HRMS.Application.Features.Personnel.Employees.DTOs;
using HRMS.Application.DTOs.Personnel;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Application.Mappings
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            // ----------------------------------------------------------------------------------
            // 1. Employee Core
            // ----------------------------------------------------------------------------------
            CreateMap<HRMS.Application.DTOs.Personnel.CreateEmployeeDto, Employee>()
                .ForMember(dest => dest.NationalityId, opt => opt.MapFrom(src => src.NationalityId));
            
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.NationalityName, opt => opt.MapFrom(src => src.Country != null ? src.Country.CountryNameAr : ""))
                .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job != null ? src.Job.JobTitleAr : ""))
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.DeptNameAr : ""));

            // Compensation
            CreateMap<HRMS.Application.DTOs.Personnel.CreateEmployeeDto, EmployeeCompensation>();
            CreateMap<EmployeeCompensation, EmployeeCompensationDto>();

            // ----------------------------------------------------------------------------------
            // 2. Qualifications
            // ----------------------------------------------------------------------------------
            CreateMap<EmployeeQualificationDto, EmployeeQualification>()
                // Mapping: degreeType -> DegreeType
                // Mapping: majorAr -> MajorAr 
                // (Already PascalCase in DTO mostly, but explicit just in case if input was camel)
                .ForMember(dest => dest.DegreeType, opt => opt.MapFrom(src => src.DegreeType))
                .ForMember(dest => dest.MajorAr, opt => opt.MapFrom(src => src.MajorAr))
                .ReverseMap();

            // ----------------------------------------------------------------------------------
            // 3. Experiences
            // ----------------------------------------------------------------------------------
            CreateMap<EmployeeExperienceDto, EmployeeExperience>()
                .ReverseMap();

            // ----------------------------------------------------------------------------------
            // 4. Emergency Contacts
            // ----------------------------------------------------------------------------------
            CreateMap<EmergencyContactDto, EmergencyContact>()
                .ReverseMap();

            // ----------------------------------------------------------------------------------
            // 5. Contracts & Renewals
            // ----------------------------------------------------------------------------------
            CreateMap<CreateContractDto, Contract>();
            CreateMap<Contract, ContractDto>();
            CreateMap<ContractRenewalDto, ContractRenewal>().ReverseMap();

            // ----------------------------------------------------------------------------------
            // 6. Certifications (Explicit Mapping)
            // ----------------------------------------------------------------------------------
            // Map certificationName -> CertNameAr, issuingOrganization -> IssuingAuthority, credentialId -> CertNumber
            CreateMap<EmployeeCertificationDto, EmployeeCertification>()
                .ForMember(dest => dest.CertNameAr, opt => opt.MapFrom(src => src.CertificationName))
                .ForMember(dest => dest.IssuingAuthority, opt => opt.MapFrom(src => src.IssuingOrganization))
                .ForMember(dest => dest.CertNumber, opt => opt.MapFrom(src => src.CredentialId))
                .ReverseMap()
                // Reverse Mapping for Read
                .ForMember(dest => dest.CertificationName, opt => opt.MapFrom(src => src.CertNameAr))
                .ForMember(dest => dest.IssuingOrganization, opt => opt.MapFrom(src => src.IssuingAuthority))
                .ForMember(dest => dest.CredentialId, opt => opt.MapFrom(src => src.CertNumber));

            // ----------------------------------------------------------------------------------
            // 7. Bank Accounts
            // ----------------------------------------------------------------------------------
            CreateMap<EmployeeBankAccountDto, EmployeeBankAccount>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.EmployeeBankAccountId))
                .ForMember(dest => dest.BankId, opt => opt.MapFrom(src => src.BankId))
                .ForMember(dest => dest.Iban, opt => opt.MapFrom(src => src.IbanNumber))
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountHolderName)) // Mapping Name to Number per instructions
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(src => src.IsPrimary ? (byte)1 : (byte)0))
                .ReverseMap()
                .ForMember(dest => dest.EmployeeBankAccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.IbanNumber, opt => opt.MapFrom(src => src.Iban))
                .ForMember(dest => dest.AccountHolderName, opt => opt.MapFrom(src => src.AccountNumber))
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(src => src.IsPrimary == 1));

            // ----------------------------------------------------------------------------------
            // 8. Dependents
            // ----------------------------------------------------------------------------------
            // Map fullNameAr -> NameAr, nationalId -> NationalId
            CreateMap<DependentDto, Dependent>()
                .ForMember(dest => dest.NameAr, opt => opt.MapFrom(src => src.FullNameAr))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId))
                .ReverseMap()
                .ForMember(dest => dest.FullNameAr, opt => opt.MapFrom(src => src.NameAr))
                .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId));

            // ----------------------------------------------------------------------------------
            // 9. Addresses
            // ----------------------------------------------------------------------------------
            // Map buildingNumber -> BuildingNo, zipCode -> PostalCode, addressType -> AddressType
            CreateMap<EmployeeAddressDto, EmployeeAddress>()
                .ForMember(dest => dest.BuildingNo, opt => opt.MapFrom(src => src.BuildingNumber))
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.ZipCode))
                .ForMember(dest => dest.AddressType, opt => opt.MapFrom(src => src.AddressType))
                .ReverseMap()
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.BuildingNo))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.PostalCode))
                .ForMember(dest => dest.AddressType, opt => opt.MapFrom(src => src.AddressType));

            // ----------------------------------------------------------------------------------
            // 10. Documents
            // ----------------------------------------------------------------------------------
            // Map documentTypeName -> DocumentTypeName (Logic needed or property match?), documentNumber -> DocumentNumber
            CreateMap<EmployeeDocumentDto, EmployeeDocument>()
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DocumentNumber))
                // Note: DocumentTypeName is usually a lookup fetch, assuming DTO has it for display
                // For creation, we usually use DocumentTypeId. 
                .ReverseMap()
                .ForMember(dest => dest.DocumentTypeName, opt => opt.MapFrom(src => src.DocumentType != null ? src.DocumentType.DocumentTypeNameAr : ""));
        }
    }
}
