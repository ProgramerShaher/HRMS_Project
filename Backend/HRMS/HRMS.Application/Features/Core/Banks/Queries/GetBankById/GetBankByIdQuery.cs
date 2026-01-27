using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Banks.Queries.GetBankById;

/// <summary>
/// استعلام للحصول على بنك بمعرفه
/// </summary>
/// <remarks>
/// يستخدم لجلب بيانات بنك محدد من قاعدة البيانات
/// </remarks>
public class GetBankByIdQuery : IRequest<BankDto?>
{
    /// <summary>
    /// المعرف الفريد للبنك
    /// </summary>
    public int BankId { get; set; }

    /// <summary>
    /// المنشئ
    /// </summary>
    /// <param name="id">معرف البنك</param>
    public GetBankByIdQuery(int id)
    {
        BankId = id;
    }
}
