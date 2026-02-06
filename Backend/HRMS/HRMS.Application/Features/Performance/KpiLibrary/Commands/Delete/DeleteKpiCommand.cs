using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.KpiLibrary.Commands.Delete;

public class DeleteKpiCommand : IRequest<Result<bool>>
{
    public int KpiId { get; set; }
}

public class DeleteKpiCommandHandler : IRequestHandler<DeleteKpiCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteKpiCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteKpiCommand request, CancellationToken cancellationToken)
    {
        var kpi = await _context.KpiLibraries
            .FirstOrDefaultAsync(k => k.KpiId == request.KpiId, cancellationToken);

        if (kpi == null)
            return Result<bool>.Failure("مؤشر الأداء غير موجود");

        // ✅ التحقق من عدم استخدام KPI في تقييمات موجودة
        var hasAppraisals = await _context.AppraisalDetails
            .AnyAsync(d => d.KpiId == request.KpiId, cancellationToken);

        if (hasAppraisals)
            return Result<bool>.Failure("لا يمكن حذف مؤشر الأداء لأنه مستخدم في تقييمات موجودة");

        _context.KpiLibraries.Remove(kpi);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم حذف مؤشر الأداء بنجاح");
    }
}

public class DeleteKpiCommandValidator : AbstractValidator<DeleteKpiCommand>
{
    public DeleteKpiCommandValidator()
    {
        RuleFor(x => x.KpiId)
            .GreaterThan(0).WithMessage("معرف مؤشر الأداء مطلوب");
    }
}
