using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Core;

namespace HRMS.Application.Features.Core.Banks.Commands.CreateBank;

/// <summary>
/// معالج أمر إنشاء بنك جديد
/// </summary>
/// <remarks>
/// يقوم بإضافة بنك جديد إلى قاعدة البيانات بعد التحقق من صحة البيانات
/// </remarks>
public class CreateBankCommandHandler : IRequestHandler<CreateBankCommand, int>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// المنشئ
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات</param>
    public CreateBankCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// معالجة أمر إنشاء البنك
    /// </summary>
    /// <param name="request">بيانات الأمر</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>معرف البنك الجديد</returns>
    public async Task<int> Handle(CreateBankCommand request, CancellationToken cancellationToken)
    {
        var bank = new Bank
        {
            BankNameAr = request.BankNameAr,
            BankNameEn = request.BankNameEn,
            SwiftCode = request.SwiftCode,
            BankCode = request.BankCode,
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        _context.Banks.Add(bank);
        await _context.SaveChangesAsync(cancellationToken);

        return bank.BankId;
    }
}
