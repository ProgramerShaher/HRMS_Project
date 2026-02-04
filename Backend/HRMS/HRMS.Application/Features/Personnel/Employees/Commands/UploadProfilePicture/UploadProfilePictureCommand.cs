using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Commands.UploadProfilePicture;

/// <summary>
/// أمر رفع صورة الملف الشخصي للموظف
/// Upload Employee Profile Picture Command
/// </summary>
public class UploadProfilePictureCommand : IRequest<Result<string>>
{
    /// <summary>
    /// معرف الموظف
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// ملف الصورة
    /// </summary>
    public IFormFile ProfilePicture { get; set; } = null!;
}

/// <summary>
/// معالج أمر رفع صورة الملف الشخصي
/// </summary>
public class UploadProfilePictureCommandHandler : IRequestHandler<UploadProfilePictureCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;

    public UploadProfilePictureCommandHandler(
        IApplicationDbContext context,
        IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Result<string>> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        // 1. التحقق من وجود الموظف
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null)
            return Result<string>.Failure($"الموظف برقم {request.EmployeeId} غير موجود");

        // 2. حذف الصورة القديمة إن وجدت
        if (!string.IsNullOrEmpty(employee.ProfilePicturePath))
        {
            try
            {
                await _fileService.DeleteFileAsync(employee.ProfilePicturePath);
            }
            catch
            {
                // تجاهل الخطأ إذا كان الملف غير موجود
            }
        }

        // 3. رفع الصورة الجديدة
        var filePath = await _fileService.UploadFileAsync(
            request.ProfilePicture,
            $"employees/{request.EmployeeId}/profile");

        // 4. تحديث مسار الصورة في قاعدة البيانات
        employee.ProfilePicturePath = filePath;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(
            filePath,
            "تم رفع صورة الملف الشخصي بنجاح");
    }
}

/// <summary>
/// التحقق من صحة أمر رفع صورة الملف الشخصي
/// </summary>
public class UploadProfilePictureCommandValidator : AbstractValidator<UploadProfilePictureCommand>
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public UploadProfilePictureCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .WithMessage("معرف الموظف غير صحيح");

        RuleFor(x => x.ProfilePicture)
            .NotNull()
            .WithMessage("يجب اختيار صورة")
            .Must(BeValidImage)
            .WithMessage("الملف يجب أن يكون صورة (jpg, jpeg, png)")
            .Must(BeValidSize)
            .WithMessage($"حجم الملف يجب ألا يتجاوز {MaxFileSize / 1024 / 1024} ميجابايت");
    }

    private bool BeValidImage(IFormFile? file)
    {
        if (file == null) return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    private bool BeValidSize(IFormFile? file)
    {
        if (file == null) return false;
        return file.Length <= MaxFileSize;
    }
}
