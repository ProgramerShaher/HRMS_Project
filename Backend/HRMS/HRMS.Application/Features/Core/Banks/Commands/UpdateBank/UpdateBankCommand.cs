using MediatR;

namespace HRMS.Application.Features.Core.Banks.Commands.UpdateBank;

/// <summary>
/// أمر تحديث بيانات بنك موجود
/// </summary>
/// <remarks>
/// يستخدم هذا الأمر لتحديث معلومات بنك موجود في النظام
/// </remarks>
public class UpdateBankCommand : IRequest<int>
{
    /// <summary>
    /// المعرف الفريد للبنك المراد تحديثه
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
