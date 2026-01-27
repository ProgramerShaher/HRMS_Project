using MediatR;

namespace HRMS.Application.Features.Core.Banks.Commands.DeleteBank;

/// <summary>
/// أمر حذف بنك من النظام
/// </summary>
/// <remarks>
/// يستخدم لحذف بنك غير مستخدم من قاعدة البيانات
/// </remarks>
public class DeleteBankCommand : IRequest<bool>
{
    /// <summary>
    /// المعرف الفريد للبنك المراد حذفه
    /// </summary>
    public int BankId { get; set; }

    /// <summary>
    /// المنشئ
    /// </summary>
    /// <param name="bankId">معرف البنك</param>
    public DeleteBankCommand(int bankId)
    {
        BankId = bankId;
    }
}
