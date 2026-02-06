using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.AppraisalCycles.Commands.Delete;

public class DeleteAppraisalCycleCommand : IRequest<Result<bool>>
{
    public int CycleId { get; set; }
}

public class DeleteAppraisalCycleCommandHandler : IRequestHandler<DeleteAppraisalCycleCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteAppraisalCycleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteAppraisalCycleCommand request, CancellationToken cancellationToken)
    {
        var cycle = await _context.AppraisalCycles
            .FirstOrDefaultAsync(c => c.CycleId == request.CycleId, cancellationToken);

        if (cycle == null)
            return Result<bool>.Failure("فترة التقييم غير موجودة");

        // ✅ التحقق من عدم وجود تقييمات مرتبطة
        var hasAppraisals = await _context.EmployeeAppraisals
            .AnyAsync(a => a.CycleId == request.CycleId, cancellationToken);

        if (hasAppraisals)
            return Result<bool>.Failure("لا يمكن حذف فترة التقييم لأنها تحتوي على تقييمات موجودة");

        _context.AppraisalCycles.Remove(cycle);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم حذف فترة التقييم بنجاح");
    }
}

public class DeleteAppraisalCycleCommandValidator : AbstractValidator<DeleteAppraisalCycleCommand>
{
    public DeleteAppraisalCycleCommandValidator()
    {
        RuleFor(x => x.CycleId)
            .GreaterThan(0).WithMessage("معرف فترة التقييم مطلوب");
    }
}
