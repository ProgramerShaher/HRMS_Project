using MediatR;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Banks.Commands.DeleteBank;

/// <summary>
/// معالج أمر حذف البنك
/// </summary>
/// <remarks>
/// يقوم بحذف البنك من قاعدة البيانات بعد التحقق من عدم استخدامه
/// </remarks>
public class DeleteBankCommandHandler : IRequestHandler<DeleteBankCommand, bool>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// المنشئ
    /// </summary>
    /// <param name="context">سياق قاعدة البيانات</param>
    public DeleteBankCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// معالجة أمر حذف البنك
    /// </summary>
    /// <param name="request">بيانات الأمر</param>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>true إذا تم الحذف بنجاح</returns>
    /// <exception cref="KeyNotFoundException">إذا لم يتم العثور على البنك</exception>
    public async Task<bool> Handle(DeleteBankCommand request, CancellationToken cancellationToken)
    {
        var bank = await _context.Banks
            .FirstOrDefaultAsync(b => b.BankId == request.BankId, cancellationToken);

        if (bank == null)
            throw new KeyNotFoundException($"البنك برقم {request.BankId} غير موجود");

        // الحذف
        _context.Banks.Remove(bank);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
