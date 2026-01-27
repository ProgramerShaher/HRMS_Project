using MediatR;

namespace HRMS.Application.Features.Core.Banks.Commands.CreateBank;

/// <summary>
/// أمر إنشاء بنك جديد
/// </summary>
/// <remarks>
/// يستخدم لإضافة بنك جديد إلى النظام
/// </remarks>
public class CreateBankCommand : IRequest<int>
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
