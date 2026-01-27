using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Core;

/// <summary>
/// نقل بيانات إعدادات النظام
/// </summary>
public class SystemSettingDto
{
    /// <summary>
    /// معرف الإعداد
    /// </summary>
    public int SettingId { get; set; }

    /// <summary>
    /// مفتاح الإعداد (فريد)
    /// </summary>
    [Required]
    public string SettingKey { get; set; } = string.Empty;

    /// <summary>
    /// قيمة الإعداد
    /// </summary>
    public string? SettingValue { get; set; }

    /// <summary>
    /// نوع الإعداد (نص، رقم، تاريخ)
    /// </summary>
    public string? SettingType { get; set; }

    /// <summary>
    /// وصف الإعداد بالعربية
    /// </summary>
    public string? DescriptionAr { get; set; }
}
