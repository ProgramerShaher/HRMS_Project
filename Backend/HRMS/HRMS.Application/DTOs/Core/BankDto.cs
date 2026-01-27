namespace HRMS.Application.DTOs.Core;

/// <summary>
/// كائن نقل البيانات لعرض معلومات البنك الكاملة
/// </summary>
/// <remarks>
/// يستخدم هذا الكائن لإرجاع بيانات البنك مع جميع التفاصيل
/// </remarks>
public class BankDto
{
    /// <summary>
    /// المعرف الفريد للبنك
    /// </summary>
    public int BankId { get; set; }

    /// <summary>
    /// اسم البنك بالعربية
    /// </summary>
    /// <example>البنك الأهلي السعودي</example>
    public string BankNameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم البنك بالإنجليزية
    /// </summary>
    /// <example>Al Ahli Bank</example>
    public string BankNameEn { get; set; } = string.Empty;

    /// <summary>
    /// رمز السويفت الدولي للبنك
    /// </summary>
    /// <example>NCBKSAJE</example>
    public string? SwiftCode { get; set; }

    /// <summary>
    /// رمز البنك المحلي
    /// </summary>
    /// <example>10</example>
    public string? BankCode { get; set; }

    /// <summary>
    /// عنوان البنك
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// رقم هاتف البنك
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// البريد الإلكتروني للبنك
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// حالة البنك (نشط/غير نشط)
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// تاريخ إنشاء السجل
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// تاريخ آخر تحديث
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
