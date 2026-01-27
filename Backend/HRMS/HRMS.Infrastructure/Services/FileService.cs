using HRMS.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace HRMS.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("الملف فارغ");

        // 💡 الحل: إذا كان WebRootPath نول، نستخدم مسار المشروع الحالي وننشئ مجلد wwwroot
        var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        // 1. تحديد المسار الأساسي (wwwroot/uploads/...)
        var uploadsRoot = Path.Combine(rootPath, "uploads");
        var finalPath = Path.Combine(uploadsRoot, folderName);

        // 2. إنشاء المجلدات إذا لم تكن موجودة
        if (!Directory.Exists(finalPath))
            Directory.CreateDirectory(finalPath);

        // 3. توليد اسم فريد (GUID)
        var fileExtension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var fullPath = Path.Combine(finalPath, uniqueFileName);

        // 4. الحفظ
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 5. إرجاع المسار النسبي (للحفظ في قاعدة البيانات)
        return Path.Combine("uploads", folderName, uniqueFileName).Replace("\\", "/");
    }

    public Task DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return Task.CompletedTask;

        var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var fullPath = Path.Combine(rootPath, filePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}