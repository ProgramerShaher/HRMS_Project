using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Recruitment;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Candidates.Commands.Create;

/// <summary>
/// أمر تسجيل مرشح جديد في نظام التوظيف
/// </summary>
public class CreateCandidateCommand : IRequest<Result<int>>
{
    /// <summary>
    /// الاسم الكامل بالإنجليزية (مطلوب)
    /// </summary>
    public string FullNameEn { get; set; } = string.Empty;

    /// <summary>
    /// الاسم الأول بالعربية
    /// </summary>
    public string? FirstNameAr { get; set; }

    /// <summary>
    /// اسم العائلة بالعربية
    /// </summary>
    public string? FamilyNameAr { get; set; }

    /// <summary>
    /// البريد الإلكتروني (مطلوب وفريد)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// رقم الهاتف
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// معرف الجنسية
    /// </summary>
    public int? NationalityId { get; set; }

    /// <summary>
    /// مسار ملف السيرة الذاتية
    /// </summary>
    public string? CvFilePath { get; set; }

    /// <summary>
    /// ملف السيرة الذاتية (لرفع الملف مباشرة)
    /// </summary>
    public Microsoft.AspNetCore.Http.IFormFile? CvFile { get; set; }

    /// <summary>
    /// رابط حساب LinkedIn
    /// </summary>
    public string? LinkedinProfile { get; set; }
}

public class CreateCandidateCommandHandler : IRequestHandler<CreateCandidateCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;

    public CreateCandidateCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileService fileService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileService = fileService;
    }

    public async Task<Result<int>> Handle(CreateCandidateCommand request, CancellationToken cancellationToken)
    {
        // ✅ التحقق من عدم تكرار البريد الإلكتروني
        var emailExists = await _context.Candidates
            .AnyAsync(c => c.Email == request.Email, cancellationToken);

        if (emailExists)
            return Result<int>.Failure("البريد الإلكتروني مسجل بالفعل");

        // التحقق من الجنسية إذا تم تحديدها
        if (request.NationalityId.HasValue)
        {
            var nationalityExists = await _context.Countries
                .AnyAsync(c => c.CountryId == request.NationalityId.Value, cancellationToken);

            if (!nationalityExists)
                return Result<int>.Failure("الجنسية المحددة غير موجودة");
        }

        string? cvPath = request.CvFilePath;

        // معالجة رفع الملف إذا وجد
        if (request.CvFile != null && request.CvFile.Length > 0)
        {
            try 
            {
                cvPath = await _fileService.UploadFileAsync(request.CvFile, "candidates/cvs");
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"فشل رفع ملف السيرة الذاتية: {ex.Message}");
            }
        }

        // إنشاء المرشح
        var candidate = new Candidate
        {
            FullNameEn = request.FullNameEn,
            FirstNameAr = request.FirstNameAr,
            FamilyNameAr = request.FamilyNameAr,
            Email = request.Email,
            Phone = request.Phone,
            NationalityId = request.NationalityId,
            CvFilePath = cvPath,
            LinkedinProfile = request.LinkedinProfile,
            CreatedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Candidates.Add(candidate);
        // await _context.SaveChangesAsync(cancellationToken); // Moved after Add

        try 
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // If DB save fails but file was uploaded, strictly speaking we should delete the file
            // But for now keeping it simple.
            return Result<int>.Failure($"حدث خطأ أثناء حفظ البيانات: {ex.Message}");
        }

        return Result<int>.Success(candidate.CandidateId, "تم تسجيل المرشح بنجاح");
    }
}

public class CreateCandidateCommandValidator : AbstractValidator<CreateCandidateCommand>
{
    public CreateCandidateCommandValidator()
    {
        RuleFor(x => x.FullNameEn)
            .NotEmpty().WithMessage("الاسم الكامل بالإنجليزية مطلوب")
            .MaximumLength(200).WithMessage("الاسم لا يمكن أن يتجاوز 200 حرف");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب")
            .EmailAddress().WithMessage("البريد الإلكتروني غير صالح")
            .MaximumLength(100).WithMessage("البريد الإلكتروني لا يمكن أن يتجاوز 100 حرف");

        RuleFor(x => x.Phone)
            .Matches(@"^[0-9+\-\s()]+$").WithMessage("رقم الهاتف غير صالح")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.FirstNameAr)
            .MaximumLength(50).WithMessage("الاسم الأول لا يمكن أن يتجاوز 50 حرف")
            .When(x => !string.IsNullOrEmpty(x.FirstNameAr));

        RuleFor(x => x.LinkedinProfile)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("رابط LinkedIn غير صالح")
            .When(x => !string.IsNullOrEmpty(x.LinkedinProfile));
    }
}
