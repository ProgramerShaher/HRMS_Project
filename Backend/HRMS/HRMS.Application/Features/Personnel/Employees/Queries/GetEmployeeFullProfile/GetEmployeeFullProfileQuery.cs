using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Features.Personnel.Employees.DTOs;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeFullProfile;

public record GetEmployeeFullProfileQuery(int EmployeeId) : IRequest<DetailedEmployeeProfileDto>;

public class GetEmployeeFullProfileQueryHandler : IRequestHandler<GetEmployeeFullProfileQuery, DetailedEmployeeProfileDto>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeFullProfileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DetailedEmployeeProfileDto> Handle(GetEmployeeFullProfileQuery request, CancellationToken cancellationToken)
    {
        // Fetch Aggregate Root with All Sub-Entities efficiently
        var employee = await _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.Job)
            .Include(e => e.Country) // For Nationality Name
            .Include(e => e.Compensation)
            .Include(e => e.Documents)
                .ThenInclude(d => d.DocumentType)
            .Include(e => e.Qualifications)
                .ThenInclude(q => q.Country)
            .Include(e => e.Experiences)
            .Include(e => e.EmergencyContacts)
            .Include(e => e.Contracts)
            .Include(e => e.Certifications)
            .Include(e => e.BankAccounts)
            .Include(e => e.Dependents)
            .Include(e => e.Addresses)
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new Exceptions.NotFoundException($"Employee {request.EmployeeId} not found");

        // ✅ Explicit Mapping with correct property names
        var profile = new DetailedEmployeeProfileDto
        {
            CoreProfile = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeNumber = employee.EmployeeNumber,
                FirstNameAr = employee.FirstNameAr,
                SecondNameAr = employee.SecondNameAr,
                ThirdNameAr = employee.ThirdNameAr,
                LastNameAr = employee.LastNameAr,
                FullNameEn = employee.FullNameEn,
                FullNameAr = employee.FullNameAr, // Computed property
                Email = employee.Email,
                Mobile = employee.Mobile, // ✅ Fixed: Entity uses Mobile (not MobileNumber)
                NationalId = employee.NationalId,
                BirthDate = employee.BirthDate, // ✅ Fixed: Entity uses BirthDate (not DateOfBirth)
                Gender = employee.Gender,
                MaritalStatus = employee.MaritalStatus,
                NationalityId = employee.NationalityId,
                NationalityName = employee.Country?.CountryNameAr,
                JobId = employee.JobId,
                JobTitle = employee.Job?.JobTitleAr, // ✅ Fixed: DTO uses JobTitle (not JobTitleAr)
                DepartmentId = employee.DepartmentId, // ✅ Fixed: Entity uses DepartmentId (not DeptId)
                DepartmentName = employee.Department?.DeptNameAr,
                HireDate = employee.HireDate,
                LicenseNumber = employee.LicenseNumber,
                LicenseExpiryDate = employee.LicenseExpiryDate,
                Specialty = employee.Specialty,
                ManagerId = employee.ManagerId,
                UserId = employee.UserId,
                ProfilePicturePath = employee.ProfilePicturePath,
                IsActive = employee.IsActive,
                TerminationDate = employee.TerminationDate
            },

            Compensation = employee.Compensation != null ? new EmployeeCompensationDto
            {
                BasicSalary = employee.Compensation.BasicSalary,
                HousingAllowance = employee.Compensation.HousingAllowance,
                TransportAllowance = employee.Compensation.TransportAllowance,
                MedicalAllowance = employee.Compensation.MedicalAllowance,
                OtherAllowances = employee.Compensation.OtherAllowances // ✅ Fixed: Include OtherAllowances
                // ✅ TotalSalary is computed property in DTO
            } : null,

            // ✅ FIX: Explicit CertificationId = c.CertId mapping
            Certifications = employee.Certifications.Select(c => new EmployeeCertificationDto
            {
                CertificationId = c.CertId, // ✅ Explicit mapping
                CertificationName = c.CertNameAr,
                IssuingOrganization = c.IssuingAuthority,
                IssueDate = c.IssueDate,
                ExpiryDate = c.ExpiryDate,
                CredentialId = c.CertNumber,
                CredentialUrl = c.AttachmentPath
            }).ToList(),

            // ✅ FIX: Explicit QualificationId = q.QualificationId mapping
            Qualifications = employee.Qualifications.Select(q => new EmployeeQualificationDto
            {
                QualificationId = q.QualificationId, // ✅ Explicit mapping
                DegreeType = q.DegreeType,
                MajorAr = q.MajorAr,
                UniversityAr = q.UniversityAr,
                CountryId = q.CountryId,
                GraduationYear = q.GraduationYear,
                Grade = q.Grade
            }).ToList(),

            // ✅ FIX: Explicit mapping for Experiences
            Experiences = employee.Experiences.Select(ex => new EmployeeExperienceDto
            {
                ExperienceId = ex.ExperienceId, // ✅ Explicit mapping
                CompanyNameAr = ex.CompanyNameAr,
                JobTitleAr = ex.JobTitleAr,
                StartDate = ex.StartDate,
                EndDate = ex.EndDate,
                IsCurrent = ex.IsCurrent,
                Responsibilities = ex.Responsibilities,
                ReasonForLeaving = ex.ReasonForLeaving
            }).ToList(),

            // ✅ FIX: Explicit mapping for EmergencyContacts
            EmergencyContacts = employee.EmergencyContacts.Select(ec => new EmergencyContactDto
            {
                ContactId = ec.ContactId, // ✅ Explicit mapping
                ContactNameAr = ec.ContactNameAr,
                Relationship = ec.Relationship,
                PhonePrimary = ec.PhonePrimary, // ✅ Fixed: Entity uses PhonePrimary
                PhoneSecondary = ec.PhoneSecondary, // ✅ Fixed: Entity uses PhoneSecondary
                IsPrimary = ec.IsPrimary
            }).ToList(),

            // ✅ FIX: Contract Logic - Identify Active Contract (ordered by StartDate descending)
            Contracts = employee.Contracts
                .OrderByDescending(c => c.StartDate)
                .Select(c => new ContractDto
                {
                    ContractId = c.ContractId, // ✅ Explicit mapping
                    EmployeeId = c.EmployeeId,
                    ContractType = c.ContractType,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    IsRenewable = c.IsRenewable,
                    BasicSalary = c.BasicSalary,
                    HousingAllowance = c.HousingAllowance,
                    TransportAllowance = c.TransportAllowance,
                    OtherAllowances = c.OtherAllowances,
                    VacationDays = c.VacationDays,
                    WorkingHoursDaily = c.WorkingHoursDaily,
                    ContractStatus = c.ContractStatus
                }).ToList(),

            // ✅ FIX: Explicit mapping for BankAccounts
            BankAccounts = employee.BankAccounts.Select(b => new EmployeeBankAccountDto
            {
                EmployeeBankAccountId = b.AccountId, // ✅ Fixed: Entity uses AccountId
                BankId = b.BankId,
                AccountHolderName = b.AccountNumber, // ✅ Using AccountNumber as holder name
                IbanNumber = b.Iban ?? string.Empty,
                IsPrimary = b.IsPrimary == 1
            }).ToList(),

            // ✅ FIX: Explicit mapping for Dependents
            Dependents = employee.Dependents.Select(d => new DependentDto
            {
                DependentId = d.DependentId, // ✅ Explicit mapping
                FullNameAr = d.NameAr, // ✅ Fixed: Entity uses NameAr
                FullNameEn = null, // Entity has no FullNameEn
                Relationship = d.Relationship,
                BirthDate = d.BirthDate ?? DateTime.MinValue, // ✅ Fixed: Entity uses BirthDate
                NationalId = d.NationalId,
                Gender = null // Entity has no Gender property
            }).ToList(),

            // ✅ FIX: Explicit mapping for Addresses
            Addresses = employee.Addresses.Select(a => new EmployeeAddressDto
            {
                AddressId = a.AddressId, // ✅ Explicit mapping
                AddressType = a.AddressType ?? "Home",
                CityId = a.CityId,
                Street = a.Street, // ✅ Fixed: Entity uses Street
                BuildingNumber = a.BuildingNo, // ✅ Fixed: Entity uses BuildingNo (not BuildingNumber)
                ZipCode = a.PostalCode,
                AdditionalDetails = null // Entity has no AdditionalDetails
            }).ToList(),

            // ✅ FIX: Explicit mapping for Documents
            Documents = employee.Documents.Select(doc => new EmployeeDocumentDto
            {
                DocumentId = doc.DocumentId, // ✅ Explicit mapping
                DocumentTypeId = doc.DocumentTypeId,
                DocumentTypeName = doc.DocumentType?.DocumentTypeNameAr, // ✅ Fixed: Entity uses DocumentTypeNameAr
                DocumentNumber = doc.DocumentNumber,
                FilePath = doc.FilePath,
                FileName = doc.FileName,
                ExpiryDate = doc.ExpiryDate
            }).ToList()
        };

        return profile;
    }
}
