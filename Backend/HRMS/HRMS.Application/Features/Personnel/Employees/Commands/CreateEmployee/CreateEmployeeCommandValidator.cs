using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateEmployeeCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Data.FirstNameAr).NotEmpty().WithMessage("الاسم الأول مطلوب");
        RuleFor(x => x.Data.LastNameAr).NotEmpty().WithMessage("اسم العائلة مطلوب");
        RuleFor(x => x.Data.Email).EmailAddress().WithMessage("البريد الإلكتروني غير صحيح");
        
        RuleFor(x => x.Data.DepartmentId)
            .GreaterThan(0).WithMessage("يجب اختيار القسم");
        
        RuleFor(x => x.Data.JobId)
            .GreaterThan(0).WithMessage("يجب اختيار الوظيفة");

        RuleFor(x => x.Data.BasicSalary)
             .GreaterThanOrEqualTo(0).WithMessage("الراتب الأساسي لا يمكن أن يكون سالب");

        // --- New Professional Validations ---
        RuleFor(x => x.Data.NationalId)
            .NotEmpty().WithMessage("رقم الهوية مطلوب")
            .Length(10).WithMessage("رقم الهوية يجب أن يكون 10 أرقام"); // افتراض قياسي

        RuleFor(x => x.Data.LicenseExpiryDate)
            .GreaterThan(DateTime.Today)
            .When(x => x.Data.LicenseExpiryDate.HasValue)
            .WithMessage("تاريخ انتهاء الترخيص يجب أن يكون في المستقبل");

        RuleFor(x => x.Data.BankId)
            .GreaterThan(0)
            .When(x => x.Data.BankId.HasValue)
            .WithMessage("بيانات البنك غير صحيحة");

        // --- Sub-Entities Validation ---
        RuleForEach(x => x.Data.Qualifications).SetValidator(new EmployeeQualificationValidator());
        RuleForEach(x => x.Data.Experiences).SetValidator(new EmployeeExperienceValidator());
        RuleForEach(x => x.Data.EmergencyContacts).SetValidator(new EmergencyContactValidator());
    }

    }

