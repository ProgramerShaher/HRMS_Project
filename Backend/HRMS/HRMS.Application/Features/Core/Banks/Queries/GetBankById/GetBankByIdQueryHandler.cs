using MediatR;
using HRMS.Application.DTOs.Core;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Banks.Queries.GetBankById;

/// <summary>
/// معالج استعلام الحصول على بنك بمعرفه
/// </summary>
/// <remarks>
/// يقوم بجلب بيانات البنك من قاعدة البيانات وتحويلها إلى DTO
/// </remarks>
public class GetBankByIdQueryHandler : IRequestHandler<GetBankByIdQuery, BankDto?>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// المنشئ
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات</param>
    public GetBankByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// معالجة الاستعلام
    /// </summary>
    /// <param name="request">بيانات الاستعلام</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>بيانات البنك أو null إذا لم يتم العثور عليه</returns>
    public async Task<BankDto?> Handle(GetBankByIdQuery request, CancellationToken cancellationToken)
    {
        var bank = await _context.Banks
            .Where(b => b.BankId == request.BankId)
            .Select(b => new BankDto
            {
                BankId = b.BankId,
                BankNameAr = b.BankNameAr,
                BankNameEn = b.BankNameEn,
                SwiftCode = b.SwiftCode,
                BankCode = b.BankCode,
                Address = b.Address,
                Phone = b.Phone,
                Email = b.Email,
                IsActive = b.IsActive,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return bank;
    }
}
