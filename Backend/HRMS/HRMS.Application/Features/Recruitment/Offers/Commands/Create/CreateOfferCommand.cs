using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Recruitment;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Offers.Commands.Create;

/// <summary>
/// أمر إنشاء عرض وظيفي مع حزمة مالية كاملة - معايير ERP
/// </summary>
public class CreateOfferCommand : IRequest<Result<int>>
{
    /// <summary>
    /// معرف طلب التوظيف
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// معرف الدرجة الوظيفية (مطلوب للتحقق من نطاق الراتب)
    /// </summary>
    public int JobGradeId { get; set; }

    /// <summary>
    /// الراتب الأساسي
    /// </summary>
    public decimal BasicSalary { get; set; }

    /// <summary>
    /// بدل السكن
    /// </summary>
    public decimal HousingAllowance { get; set; }

    /// <summary>
    /// بدل المواصلات
    /// </summary>
    public decimal TransportAllowance { get; set; }

    /// <summary>
    /// البدل الطبي
    /// </summary>
    public decimal MedicalAllowance { get; set; }

    /// <summary>
    /// البدلات الأخرى
    /// </summary>
    public decimal OtherAllowances { get; set; }

    /// <summary>
    /// تاريخ الالتحاق المتوقع
    /// </summary>
    public DateTime JoiningDate { get; set; }

    /// <summary>
    /// تاريخ العرض
    /// </summary>
    public DateTime OfferDate { get; set; }

    /// <summary>
    /// تاريخ انتهاء صلاحية العرض
    /// </summary>
    public DateTime ExpiryDate { get; set; }

    /// <summary>
    /// شروط وتفاصيل إضافية
    /// </summary>
    public string? Terms { get; set; }
}

/// <summary>
/// معالج أمر إنشاء العرض الوظيفي
/// </summary>
public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateOfferCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // STEP 1: Validate Application Exists
        // ═══════════════════════════════════════════════════════════
        var application = await _context.JobApplications
            .Include(a => a.Vacancy)
            .FirstOrDefaultAsync(a => a.AppId == request.AppId && a.IsDeleted == 0, cancellationToken);

        if (application == null)
            return Result<int>.Failure("طلب التوظيف غير موجود");

        // ═══════════════════════════════════════════════════════════
        // STEP 2: Verify Candidate Passed Interview
        // ═══════════════════════════════════════════════════════════
        var hasPassedInterview = await _context.Interviews
            .AnyAsync(i => i.AppId == request.AppId && i.Result == "PASSED" && i.IsDeleted == 0, cancellationToken);

        if (!hasPassedInterview)
            return Result<int>.Failure("يجب أن يجتاز المرشح مقابلة واحدة على الأقل قبل تقديم عرض");

        // ═══════════════════════════════════════════════════════════
        // STEP 3: ✅ CRITICAL - Validate Salary Against Job Grade
        // ═══════════════════════════════════════════════════════════
        var jobGrade = await _context.JobGrades
            .FirstOrDefaultAsync(g => g.JobGradeId == request.JobGradeId && g.IsDeleted == 0, cancellationToken);

        if (jobGrade == null)
            return Result<int>.Failure($"الدرجة الوظيفية رقم {request.JobGradeId} غير موجودة");

        // ✅ The "Masatara" - Salary Range Validation
        if (request.BasicSalary < jobGrade.MinSalary || request.BasicSalary > jobGrade.MaxSalary)
        {
            return Result<int>.Failure(
                $"الراتب الأساسي ({request.BasicSalary:N2}) خارج نطاق الدرجة الوظيفية " +
                $"({jobGrade.GradeNameAr}). النطاق المسموح: {jobGrade.MinSalary:N2} - {jobGrade.MaxSalary:N2}"
            );
        }

        // ═══════════════════════════════════════════════════════════
        // STEP 4: Create Job Offer with Complete Financial Package
        // ═══════════════════════════════════════════════════════════
        var offer = new JobOffer
        {
            AppId = request.AppId,
            OfferDate = request.OfferDate,
            ExpiryDate = request.ExpiryDate,
            
            // ✅ Complete Financial Package
            BasicSalary = request.BasicSalary,
            HousingAllowance = request.HousingAllowance,
            TransportAllowance = request.TransportAllowance,
            MedicalAllowance = request.MedicalAllowance,
            OtherAllowances = request.OtherAllowances,
            
            JoiningDate = request.JoiningDate,
            Status = "PENDING",
            
            CreatedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.JobOffers.Add(offer);
        await _context.SaveChangesAsync(cancellationToken);

        var totalPackage = request.BasicSalary + request.HousingAllowance + 
                          request.TransportAllowance + request.MedicalAllowance + 
                          request.OtherAllowances;

        return Result<int>.Success(
            offer.OfferId, 
            $"تم إنشاء العرض الوظيفي بنجاح - الدرجة: {jobGrade.GradeNameAr} - الحزمة الكلية: {totalPackage:N2}"
        );
    }
}

/// <summary>
/// التحقق من صحة بيانات العرض - The "Masatara" Validator
/// </summary>
public class CreateOfferCommandValidator : AbstractValidator<CreateOfferCommand>
{
    public CreateOfferCommandValidator()
    {
        RuleFor(x => x.AppId)
            .GreaterThan(0).WithMessage("معرف الطلب مطلوب");

        RuleFor(x => x.JobGradeId)
            .GreaterThan(0).WithMessage("معرف الدرجة الوظيفية مطلوب");

        RuleFor(x => x.BasicSalary)
            .GreaterThan(0).WithMessage("الراتب الأساسي يجب أن يكون أكبر من صفر");

        RuleFor(x => x.HousingAllowance)
            .GreaterThanOrEqualTo(0).WithMessage("بدل السكن لا يمكن أن يكون سالباً");

        RuleFor(x => x.TransportAllowance)
            .GreaterThanOrEqualTo(0).WithMessage("بدل المواصلات لا يمكن أن يكون سالباً");

        RuleFor(x => x.MedicalAllowance)
            .GreaterThanOrEqualTo(0).WithMessage("البدل الطبي لا يمكن أن يكون سالباً");

        RuleFor(x => x.OtherAllowances)
            .GreaterThanOrEqualTo(0).WithMessage("البدلات الأخرى لا يمكن أن تكون سالبة");

        // ✅ Date Validations
        RuleFor(x => x.OfferDate)
            .NotEmpty().WithMessage("تاريخ العرض مطلوب")
            .LessThanOrEqualTo(DateTime.Now.AddDays(1)).WithMessage("تاريخ العرض لا يمكن أن يكون في المستقبل البعيد");

        RuleFor(x => x.ExpiryDate)
            .NotEmpty().WithMessage("تاريخ انتهاء الصلاحية مطلوب")
            .GreaterThan(x => x.OfferDate).WithMessage("تاريخ الانتهاء يجب أن يكون بعد تاريخ العرض");

        RuleFor(x => x.JoiningDate)
            .NotEmpty().WithMessage("تاريخ الالتحاق مطلوب")
            .GreaterThanOrEqualTo(x => x.OfferDate).WithMessage("تاريخ الالتحاق يجب أن يكون بعد أو يساوي تاريخ العرض");

        RuleFor(x => x.Terms)
            .MaximumLength(1000).WithMessage("الشروط لا يمكن أن تتجاوز 1000 حرف")
            .When(x => !string.IsNullOrEmpty(x.Terms));
    }
}
