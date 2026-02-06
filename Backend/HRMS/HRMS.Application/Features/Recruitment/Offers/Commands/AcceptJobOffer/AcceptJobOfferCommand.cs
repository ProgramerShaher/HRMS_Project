using FluentValidation;
using HRMS.Application.Features.Personnel.Contracts.Helpers;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Offers.Commands.AcceptJobOffer;

/// <summary>
/// أمر قبول عرض توظيف - دورة التوظيف الكاملة (Full Hiring Cycle)
/// </summary>
/// <remarks>
/// **التكامل ERP الكامل (The Full ERP Magic)**:
/// 1. تحديث حالة العرض إلى "ACCEPTED"
/// 2. التوظيف التلقائي (Auto-Hire):
///    - إنشاء موظف جديد من بيانات المرشح
///    - نسخ البيانات الشخصية (Name, Email, Phone, Nationality)
///    - دعم البيانات المفقودة عبر الـ Command
/// 3. إعداد الهيكل المالي الكامل (Complete Financial Setup):
///    - استخدام SalaryStructureSyncHelper لمزامنة جميع البدلات
///    - Basic Salary, Housing, Transport, Medical, Other Allowances
/// 4. كل العمليات في Transaction واحدة
/// </remarks>
public class AcceptJobOfferCommand : IRequest<Result<int>>
{
    /// <summary>
    /// معرف العرض الوظيفي
    /// </summary>
    public int OfferId { get; set; }

    /// <summary>
    /// تاريخ الالتحاق الفعلي
    /// </summary>
    public DateTime? JoiningDate { get; set; }

    /// <summary>
    /// رقم وظيفي للموظف الجديد (اختياري - سيتم توليده تلقائياً)
    /// </summary>
    public string? EmployeeNumber { get; set; }

    // ═══════════════════════════════════════════════════════════
    // ✅ NEW: Missing Employee Data Fields
    // ═══════════════════════════════════════════════════════════
    
    /// <summary>
    /// رقم الهوية الوطنية (مطلوب إذا لم يكن موجوداً في بيانات المرشح)
    /// </summary>
    public string? NationalId { get; set; }

    /// <summary>
    /// رقم الجوال (مطلوب إذا لم يكن موجوداً في بيانات المرشح)
    /// </summary>
    public string? Mobile { get; set; }

    /// <summary>
    /// تاريخ الميلاد
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// الجنس (M/F)
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// الحالة الاجتماعية
    /// </summary>
    public string? MaritalStatus { get; set; }
}

/// <summary>
/// التحقق من صحة البيانات
/// </summary>
public class AcceptJobOfferCommandValidator : AbstractValidator<AcceptJobOfferCommand>
{
    public AcceptJobOfferCommandValidator()
    {
        RuleFor(x => x.OfferId)
            .GreaterThan(0)
            .WithMessage("معرف العرض مطلوب");

        RuleFor(x => x.JoiningDate)
            .GreaterThanOrEqualTo(DateTime.Today.AddDays(-30))
            .When(x => x.JoiningDate.HasValue)
            .WithMessage("تاريخ الالتحاق لا يمكن أن يكون قديماً جداً");

        RuleFor(x => x.EmployeeNumber)
            .MaximumLength(20)
            .WithMessage("الرقم الوظيفي لا يتجاوز 20 حرف");

        RuleFor(x => x.Gender)
            .Must(g => g == null || g == "M" || g == "F")
            .WithMessage("الجنس يجب أن يكون M أو F");

        RuleFor(x => x.NationalId)
            .MaximumLength(20)
            .WithMessage("رقم الهوية لا يتجاوز 20 حرف");

        RuleFor(x => x.Mobile)
            .MaximumLength(15)
            .WithMessage("رقم الجوال لا يتجاوز 15 رقم");
    }
}

/// <summary>
/// معالج أمر قبول العرض - تحويل المرشح إلى موظف
/// </summary>
public class AcceptJobOfferCommandHandler : IRequestHandler<AcceptJobOfferCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AcceptJobOfferCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(AcceptJobOfferCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // STEP 1: Start Database Transaction
        // ═══════════════════════════════════════════════════════════
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // ═══════════════════════════════════════════════════════════
            // STEP 2: Fetch Offer with Related Data
            // ═══════════════════════════════════════════════════════════
            var offer = await _context.JobOffers
                .Include(o => o.Application)
                    .ThenInclude(a => a.Candidate)
                .Include(o => o.Application)
                    .ThenInclude(a => a.Vacancy)
                .FirstOrDefaultAsync(o => o.OfferId == request.OfferId && o.IsDeleted == 0, cancellationToken);

            if (offer == null)
                return Result<int>.Failure("العرض الوظيفي المحدد غير موجود");

            if (offer.Status == "ACCEPTED")
                return Result<int>.Failure("هذا العرض تم قبوله مسبقاً");

            if (offer.Status == "REJECTED")
                return Result<int>.Failure("لا يمكن قبول عرض سبق رفضه");

            var candidate = offer.Application.Candidate;
            var vacancy = offer.Application.Vacancy;

            // ═══════════════════════════════════════════════════════════
            // STEP 3: Update Offer Status
            // ═══════════════════════════════════════════════════════════
            offer.Status = "ACCEPTED";
            offer.JoiningDate = request.JoiningDate ?? DateTime.Now;
            offer.UpdatedBy = _currentUserService.UserId;
            offer.UpdatedAt = DateTime.UtcNow;

            // ═══════════════════════════════════════════════════════════
            // STEP 4: Generate Employee Number
            // ═══════════════════════════════════════════════════════════
            string employeeNumber = request.EmployeeNumber 
                ?? await GenerateEmployeeNumberAsync(cancellationToken);

            // ═══════════════════════════════════════════════════════════
            // STEP 5: Create New Employee with Robust Data Mapping
            // ═══════════════════════════════════════════════════════════
            var newEmployee = new Employee
            {
                EmployeeNumber = employeeNumber,
                
                // ✅ Personal Data - Use candidate data where available
                FirstNameAr = candidate.FirstNameAr ?? "",
                SecondNameAr = "",
                ThirdNameAr = "",
                LastNameAr = candidate.FamilyNameAr ?? "",
                FullNameEn = candidate.FullNameEn,
                
                Email = candidate.Email,
                
                // ✅ Use command Mobile if provided, fallback to candidate Phone, or empty
                Mobile = request.Mobile ?? candidate.Phone ?? "",
                
                // ✅ Use command NationalId if provided, or generate temp
                // (Candidate entity doesn't have NationalId field)
                NationalId = request.NationalId ?? "TEMP-" + DateTime.Now.Ticks,
                
                // ✅ Use command BirthDate if provided, or default
                // (Candidate entity doesn't have BirthDate field)
                BirthDate = request.BirthDate ?? DateTime.Now.AddYears(-25),
                
                // ✅ Use command Gender if provided, or default
                // (Candidate entity doesn't have Gender field)
                Gender = request.Gender ?? "M",
                
                // ✅ Use command MaritalStatus if provided, or default
                MaritalStatus = request.MaritalStatus ?? "Single",
                
                // Job Data
                HireDate = offer.JoiningDate ?? DateTime.Now,
                JobId = vacancy.JobId,
                DepartmentId = vacancy.DeptId,
                NationalityId = candidate.NationalityId,
                
                IsActive = true,
                
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync(cancellationToken);

            // ═══════════════════════════════════════════════════════════
            // STEP 6: Setup EmployeeCompensation (Backward Compatibility)
            // ═══════════════════════════════════════════════════════════
            var compensation = new EmployeeCompensation
            {
                EmployeeId = newEmployee.EmployeeId,
                BasicSalary = offer.BasicSalary ?? 0,
                HousingAllowance = offer.HousingAllowance ?? 0,
                TransportAllowance = offer.TransportAllowance ?? 0,
                MedicalAllowance = offer.MedicalAllowance ?? 0,
                OtherAllowances = offer.OtherAllowances ?? 0
            };
            _context.EmployeeCompensations.Add(compensation);

            // ═══════════════════════════════════════════════════════════
            // STEP 7: ✅ Complete Financial Sync Using SalaryStructureSyncHelper
            // ═══════════════════════════════════════════════════════════
            await SalaryStructureSyncHelper.SyncAllContractComponentsAsync(
                _context,
                newEmployee.EmployeeId,
                basicSalary: offer.BasicSalary ?? 0,
                housingAllowance: offer.HousingAllowance ?? 0,
                transportAllowance: offer.TransportAllowance ?? 0,
                medicalAllowance: offer.MedicalAllowance ?? 0,
                otherAllowances: offer.OtherAllowances ?? 0,
                cancellationToken
            );

            // ═══════════════════════════════════════════════════════════
            // STEP 8: Commit Transaction
            // ═══════════════════════════════════════════════════════════
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<int>.Success(
                newEmployee.EmployeeId,
                $"تم قبول العرض وتوظيف المرشح بنجاح - رقم الموظف: {employeeNumber}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<int>.Failure($"حدث خطأ أثناء معالجة العرض: {ex.Message}");
        }
    }

    /// <summary>
    /// توليد رقم وظيفي فريد
    /// </summary>
    private async Task<string> GenerateEmployeeNumberAsync(CancellationToken cancellationToken)
    {
        // نمط: EMP-YYYY-NNNN
        var year = DateTime.Now.Year;
        var lastEmployee = await _context.Employees
            .Where(e => e.EmployeeNumber.StartsWith($"EMP-{year}"))
            .OrderByDescending(e => e.EmployeeNumber)
            .FirstOrDefaultAsync(cancellationToken);

        int nextNumber = 1;
        if (lastEmployee != null)
        {
            var parts = lastEmployee.EmployeeNumber.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int lastNum))
                nextNumber = lastNum + 1;
        }

        return $"EMP-{year}-{nextNumber:D4}";
    }
}
