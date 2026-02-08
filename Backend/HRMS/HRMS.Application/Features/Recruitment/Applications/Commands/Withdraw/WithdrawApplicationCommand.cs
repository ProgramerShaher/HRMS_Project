using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Applications.Commands.Withdraw;

/// <summary>
/// Withdraw Job Application Command
/// </summary>
public class WithdrawApplicationCommand : IRequest<Result<int>>
{
    public int AppId { get; set; }
}

/// <summary>
/// Withdraw Application Command Handler
/// </summary>
public class WithdrawApplicationCommandHandler : IRequestHandler<WithdrawApplicationCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public WithdrawApplicationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(WithdrawApplicationCommand request, CancellationToken cancellationToken)
    {
        // البحث عن طلب التوظيف
        var application = await _context.JobApplications
            .FirstOrDefaultAsync(a => a.AppId == request.AppId && a.IsDeleted == 0, cancellationToken);

        if (application == null)
            return Result<int>.Failure("طلب التوظيف غير موجود");

        // Business Rule: Cannot withdraw if already hired or offered
        if (application.Status == "HIRED")
            return Result<int>.Failure("لا يمكن سحب الطلب بعد التوظيف");

        if (application.Status == "OFFERED")
            return Result<int>.Failure("لا يمكن سحب الطلب بعد إرسال عرض العمل. يرجى رفض العرض أولاً");

        // تحديث الحالة والحذف الناعم
        application.Status = "WITHDRAWN";
        application.IsDeleted = 1;
        application.UpdatedBy = _currentUserService.UserId;
        application.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(application.AppId, "تم سحب طلب التوظيف بنجاح");
    }
}
