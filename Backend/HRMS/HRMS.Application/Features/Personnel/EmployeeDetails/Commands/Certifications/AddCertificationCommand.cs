using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Certifications;

/// <summary>
/// أمر إضافة شهادة مهنية للموظف
/// Add Employee Certification Command
/// </summary>
public class AddCertificationCommand : IRequest<Result<int>>
{
    /// <summary>
    /// معرف الموظف
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// اسم الشهادة بالعربية
    /// </summary>
    public string CertNameAr { get; set; } = string.Empty;

    /// <summary>
    /// الجهة المانحة
    /// </summary>
    public string? IssuingAuthority { get; set; }

    /// <summary>
    /// تاريخ الإصدار
    /// </summary>
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// تاريخ الانتهاء
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// رقم الشهادة
    /// </summary>
    public string? CertNumber { get; set; }

    /// <summary>
    /// هل هي إلزامية (1=نعم، 0=لا)
    /// </summary>
    public byte IsMandatory { get; set; }

    /// <summary>
    /// ملف الشهادة المرفق (اختياري)
    /// </summary>
    public IFormFile? Attachment { get; set; }
}

/// <summary>
/// معالج أمر إضافة شهادة مهنية
/// </summary>
public class AddCertificationCommandHandler : IRequestHandler<AddCertificationCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public AddCertificationCommandHandler(
        IApplicationDbContext context,
        IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<int>> Handle(AddCertificationCommand request, CancellationToken cancellationToken)
    {
        // 1. رفع الملف المرفق إن وجد
        string? attachmentPath = null;
        if (request.Attachment != null)
        {
            attachmentPath = await _fileService.UploadFileAsync(
                request.Attachment,
                $"employees/{request.EmployeeId}/certifications");
        }

        // 2. إنشاء كيان الشهادة
        var certification = new EmployeeCertification
        {
            EmployeeId = request.EmployeeId,
            CertNameAr = request.CertNameAr,
            IssuingAuthority = request.IssuingAuthority,
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate,
            CertNumber = request.CertNumber,
            IsMandatory = request.IsMandatory,
            AttachmentPath = attachmentPath
        };

        // 3. حفظ في قاعدة البيانات
        _context.Certifications.Add(certification);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(
            certification.CertId,
            "تم إضافة الشهادة المهنية بنجاح");
    }
}

/// <summary>
/// التحقق من صحة أمر إضافة شهادة مهنية
/// </summary>
public class AddCertificationCommandValidator : AbstractValidator<AddCertificationCommand>
{
    private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public AddCertificationCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .WithMessage("معرف الموظف غير صحيح");

        RuleFor(x => x.CertNameAr)
            .NotEmpty()
            .WithMessage("اسم الشهادة مطلوب")
            .MaximumLength(200)
            .WithMessage("اسم الشهادة يجب ألا يتجاوز 200 حرف");

        RuleFor(x => x.IssuingAuthority)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.IssuingAuthority))
            .WithMessage("الجهة المانحة يجب ألا تتجاوز 200 حرف");

        RuleFor(x => x.CertNumber)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.CertNumber))
            .WithMessage("رقم الشهادة يجب ألا يتجاوز 100 حرف");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.IssueDate)
            .When(x => x.IssueDate.HasValue && x.ExpiryDate.HasValue)
            .WithMessage("تاريخ الانتهاء يجب أن يكون بعد تاريخ الإصدار");

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
