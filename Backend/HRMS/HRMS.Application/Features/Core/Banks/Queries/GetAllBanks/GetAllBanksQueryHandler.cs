using MediatR;
using HRMS.Application.DTOs.Core;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Banks.Queries.GetAllBanks;

/// <summary>
/// معالج استعلام الحصول على قائمة البنوك
/// </summary>
/// <remarks>
/// يقوم بجلب قائمة البنوك مع الترقيم والترتيب والفلترة
/// </remarks>
public class GetAllBanksQueryHandler : IRequestHandler<GetAllBanksQuery, PagedResult<BankListDto>>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// المنشئ
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات</param>
    public GetAllBanksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// معالجة الاستعلام
    /// </summary>
    /// <param name="request">بيانات الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>قائمة مرقمة من البنوك</returns>
    public async Task<PagedResult<BankListDto>> Handle(GetAllBanksQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Banks.AsQueryable();

        // فلترة حسب الحالة
        if (request.IsActive.HasValue)
        {
            query = query.Where(b => b.IsActive == request.IsActive.Value);
        }

        // العدد الكلي
        var totalCount = await query.CountAsync(cancellationToken);

        // الترتيب
        query = request.SortBy.ToLower() switch
        {
            "banknameen" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(b => b.BankNameEn)
                : query.OrderBy(b => b.BankNameEn),
            "bankcode" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(b => b.BankCode)
                : query.OrderBy(b => b.BankCode),
            "createdat" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(b => b.CreatedAt)
                : query.OrderBy(b => b.CreatedAt),
            _ => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(b => b.BankNameAr)
                : query.OrderBy(b => b.BankNameAr)
        };

        // الترقيم والإسقاط
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new BankListDto
            {
                BankId = b.BankId,
                BankNameAr = b.BankNameAr,
                BankNameEn = b.BankNameEn,
                SwiftCode = b.SwiftCode,
                BankCode = b.BankCode,
                IsActive = b.IsActive
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedResult<BankListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
