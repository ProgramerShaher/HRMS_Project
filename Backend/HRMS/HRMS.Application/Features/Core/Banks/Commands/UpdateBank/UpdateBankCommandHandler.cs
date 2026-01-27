using MediatR;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Banks.Commands.UpdateBank;

/// <summary>
/// معالج أمر تحديث البنك
/// </summary>
/// <remarks>
/// يقوم بتحديث بيانات البنك في قاعدة البيانات بعد التحقق من صحتها
/// </remarks>
public class UpdateBankCommandHandler : IRequestHandler<UpdateBankCommand, int>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// المنشئ
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات</param>
    public UpdateBankCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// معالجة أمر تحديث البنك
    /// </summary>
    /// <param name="request">بيانات الأمر</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>معرف البنك المحدث</returns>
    /// <exception cref="KeyNotFoundException">إذا لم يتم العثور على البنك</exception>
    public async Task<int> Handle(UpdateBankCommand request, CancellationToken cancellationToken)
    {
        var bank = await _context.Banks
            .FirstOrDefaultAsync(b => b.BankId == request.BankId, cancellationToken);

        if (bank == null)
            throw new KeyNotFoundException($"البنك برقم {request.BankId} غير موجود");

        // تحديث البيانات
        bank.BankNameAr = request.BankNameAr;
        bank.BankNameEn = request.BankNameEn;
        bank.SwiftCode = request.SwiftCode;
        bank.BankCode = request.BankCode;
        bank.Address = request.Address;
        bank.Phone = request.Phone;
        bank.Email = request.Email;
        bank.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return bank.BankId;
    }
}
