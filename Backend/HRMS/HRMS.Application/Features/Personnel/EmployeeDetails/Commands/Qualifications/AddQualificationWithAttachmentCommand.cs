using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Qualifications;

/// <summary>
/// أمر إضافة مؤهل علمي للموظف
/// Add Employee Qualification Command
/// </summary>
public class AddQualificationWithAttachmentCommand : IRequest<Result<int>>
{
    /// <summary>
    /// معرف الموظف
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// نوع الدرجة العلمية
    /// </summary>
    public string DegreeType { get; set; } = string.Empty;

    /// <summary>
    /// التخصص بالعربية
    /// </summary>
    public string MajorAr { get; set; } = string.Empty;

    /// <summary>
    /// الجامعة
    /// </summary>
    public string? UniversityAr { get; set; }

    /// <summary>
    /// معرف الدولة
    /// </summary>
    public int? CountryId { get; set; }

    /// <summary>
    /// سنة التخرج
    /// </summary>
    public short? GraduationYear { get; set; }

    /// <summary>
    /// التقدير
    /// </summary>
    public string? Grade { get; set; }

    /// <summary>
    /// ملف الشهادة المرفق (اختياري)
    /// </summary>
    public IFormFile? Attachment { get; set; }
}

/// <summary>
/// معالج أمر إضافة مؤهل علمي
/// </summary>
public class AddQualificationWithAttachmentCommandHandler : IRequestHandler<AddQualificationWithAttachmentCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public AddQualificationWithAttachmentCommandHandler(
        IApplicationDbContext context,
        IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<int>> Handle(AddQualificationWithAttachmentCommand request, CancellationToken cancellationToken)
    {
        // 1. رفع الملف المرفق إن وجد
        string? attachmentPath = null;
        if (request.Attachment != null)
        {
            attachmentPath = await _fileService.UploadFileAsync(
                request.Attachment,
                $"employees/{request.EmployeeId}/qualifications");
        }

        // 2. إنشاء كيان المؤهل
        var qualification = new EmployeeQualification
        {
            EmployeeId = request.EmployeeId,
            DegreeType = request.DegreeType,
            MajorAr = request.MajorAr,
            UniversityAr = request.UniversityAr,
            CountryId = request.CountryId,
            GraduationYear = request.GraduationYear,
            Grade = request.Grade,
            AttachmentPath = attachmentPath
        };

        // 3. حفظ في قاعدة البيانات
        _context.Qualifications.Add(qualification);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(
            qualification.QualificationId,
            "تم إضافة المؤهل العلمي بنجاح");
    }
}

/// <summary>
/// التحقق من صحة أمر إضافة مؤهل علمي
/// </summary>
public class AddQualificationWithAttachmentCommandValidator : AbstractValidator<AddQualificationWithAttachmentCommand>
{
    private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public AddQualificationWithAttachmentCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .WithMessage("معرف الموظف غير صحيح");

        RuleFor(x => x.DegreeType)
            .NotEmpty()
            .WithMessage("نوع الدرجة العلمية مطلوب")
            .MaximumLength(50)
            .WithMessage("نوع الدرجة يجب ألا يتجاوز 50 حرف");

        RuleFor(x => x.MajorAr)
            .NotEmpty()
            .WithMessage("التخصص مطلوب")
            .MaximumLength(100)
            .WithMessage("التخصص يجب ألا يتجاوز 100 حرف");

        RuleFor(x => x.UniversityAr)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.UniversityAr))
            .WithMessage("اسم الجامعة يجب ألا يتجاوز 200 حرف");

        RuleFor(x => x.GraduationYear)
            .LessThanOrEqualTo((short)DateTime.Now.Year)
            .When(x => x.GraduationYear.HasValue)
            .WithMessage("سنة التخرج لا يمكن أن تكون في المستقبل");

        RuleFor(x => x.Attachment)
            .Must(BeValidFile!)
            .When(x => x.Attachment != null)
            .WithMessage("الملف يجب أن يكون بصيغة PDF أو صورة (jpg, jpeg, png)")
            .Must(BeValidSize!)
            .When(x => x.Attachment != null)
            .WithMessage($"حجم الملف يجب ألا يتجاوز {MaxFileSize / 1024 / 1024} ميجابايت");
    }

    private bool BeValidFile(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    private bool BeValidSize(IFormFile file)
    {
        return file.Length <= MaxFileSize;
    }
}
