namespace HRMS.Application.DTOs.Core;

/// <summary>
/// كائن نقل البيانات لعرض قائمة البنوك (مبسط)
/// </summary>
/// <remarks>
/// يستخدم في القوائم والجداول لتحسين الأداء بعرض البيانات الأساسية فقط
/// </remarks>
public class BankListDto
{
    /// <summary>
    /// المعرف الفريد للبنك
    /// </summary>
    public int BankId { get; set; }

    /// <summary>
    /// اسم البنك بالعربية
    /// </summary>
    public string BankNameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم البنك بالإنجليزية
    /// </summary>
    public string BankNameEn { get; set; } = string.Empty;

    /// <summary>
    /// رمز السويفت الدولي
    /// </summary>
    public string? SwiftCode { get; set; }

    /// <summary>
    /// رمز البنك المحلي
    /// </summary>
    public string? BankCode { get; set; }

    /// <summary>
    /// حالة البنك (نشط/غير نشط)
    /// </summary>
    public bool IsActive { get; set; }
}
