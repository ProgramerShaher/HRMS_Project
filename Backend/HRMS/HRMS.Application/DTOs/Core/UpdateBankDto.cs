namespace HRMS.Application.DTOs.Core;

/// <summary>
/// كائن نقل البيانات لتحديث معلومات البنك
/// </summary>
public class UpdateBankDto
{
    /// <summary>
    /// اسم البنك بالعربية
    /// </summary>
    public string BankNameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم البنك بالإنجليزية
    /// </summary>
    public string BankNameEn { get; set; } = string.Empty;

    /// <summary>
    /// رمز السويفت الدولي (8 أو 11 حرف)
    /// </summary>
    public string? SwiftCode { get; set; }

    /// <summary>
    /// رمز البنك المحلي
    /// </summary>
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
}
