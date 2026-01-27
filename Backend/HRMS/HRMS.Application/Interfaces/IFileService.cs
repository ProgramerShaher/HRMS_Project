using Microsoft.AspNetCore.Http;

namespace HRMS.Application.Interfaces;

public interface IFileService
{
    /// <summary>
    /// رفع ملف وحفظه في المسار المحدد
    /// </summary>
    /// <param name="file">الملف المرفوع</param>
    /// <param name="folderName">اسم المجلد الفرعي (e.g. employees/documents)</param>
    /// <returns>المسار النسبي للملف المحفوظ</returns>
    Task<string> UploadFileAsync(IFormFile file, string folderName);

    /// <summary>
    /// حذف ملف من السيرفر
    /// </summary>
    Task DeleteFileAsync(string filePath);
}
