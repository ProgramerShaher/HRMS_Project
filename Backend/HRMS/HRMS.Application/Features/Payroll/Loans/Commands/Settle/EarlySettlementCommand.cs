using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Loans.Commands.Settle;

/// <summary>
/// Early Loan Settlement Command
/// </summary>
public class EarlySettlementCommand : IRequest<Result<bool>>
{
    public int LoanId { get; set; }
    public string? SettlementNotes { get; set; }
}

/// <summary>
/// Early Settlement Command Handler
/// </summary>
public class EarlySettlementCommandHandler : IRequestHandler<EarlySettlementCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public EarlySettlementCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(EarlySettlementCommand request, CancellationToken cancellationToken)
    {
        // جلب القرض مع الأقساط
        var loan = await _context.Loans
            .Include(l => l.Installments)
            .FirstOrDefaultAsync(l => l.LoanId == request.LoanId && l.IsDeleted == 0, cancellationToken);

        if (loan == null)
            return Result<bool>.Failure("القرض غير موجود");

        // Business Rule: لا يمكن تسوية قرض مغلق أو مسدد مسبقاً
        if (loan.Status == "CLOSED" || loan.Status == "SETTLED")
            return Result<bool>.Failure("القرض مغلق أو مسدد مسبقاً");

        // جلب جميع الأقساط غير المدفوعة
        var unpaidInstallments = loan.Installments
            .Where(i => i.Status == "UNPAID")
            .ToList();

        if (!unpaidInstallments.Any())
            return Result<bool>.Failure("لا توجد أقساط غير مدفوعة للتسوية");

        // تحديث حالة جميع الأقساط غير المدفوعة
        foreach (var installment in unpaidInstallments)
        {
            installment.Status = "SETTLED_MANUALLY";
            installment.PaidDate = DateTime.UtcNow;
            installment.SettlementNotes = request.SettlementNotes;
            installment.UpdatedBy = _currentUserService.UserId;
            installment.UpdatedAt = DateTime.UtcNow;
        }

        // تحديث حالة القرض
        loan.Status = "CLOSED";
        loan.SettlementDate = DateTime.UtcNow;
        loan.SettlementNotes = request.SettlementNotes;
        loan.UpdatedBy = _currentUserService.UserId;
        loan.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, $"تم تسوية القرض بنجاح. عدد الأقساط المسددة: {unpaidInstallments.Count}");
    }
}
