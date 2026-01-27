using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Banks.Queries.GetAllBanks;

/// <summary>
/// استعلام للحصول على قائمة البنوك مع الترقيم
/// </summary>
/// <remarks>
/// يستخدم لجلب قائمة البنوك مع إمكانية الترقيم والترتيب والفلترة
/// </remarks>
public class GetAllBanksQuery : IRequest<PagedResult<BankListDto>>
{
    /// <summary>
    /// رقم الصفحة (يبدأ من 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// عدد العناصر في الصفحة
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// الترتيب حسب (BankNameAr, BankNameEn, BankCode, CreatedAt)
    /// </summary>
    public string SortBy { get; set; } = "BankNameAr";

    /// <summary>
    /// اتجاه الترتيب (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// فلترة حسب الحالة (نشط/غير نشط)
    /// </summary>
    public bool? IsActive { get; set; }
}
