using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.AppraisalCycles.Commands.Update;

public class UpdateAppraisalCycleCommand : IRequest<Result<bool>>
{
    public int CycleId { get; set; }
    public string CycleName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "PLANNING";
}

public class UpdateAppraisalCycleCommandHandler : IRequestHandler<UpdateAppraisalCycleCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAppraisalCycleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(UpdateAppraisalCycleCommand request, CancellationToken cancellationToken)
    {
        var cycle = await _context.AppraisalCycles
            .FirstOrDefaultAsync(c => c.CycleId == request.CycleId, cancellationToken);

        if (cycle == null)
            return Result<bool>.Failure("فترة التقييم غير موجودة");

        // التحقق من عدم تداخل التواريخ (مع استثناء السجل الحالي)
        var overlapping = await _context.AppraisalCycles
            .AnyAsync(c => c.CycleId != request.CycleId &&
                ((request.StartDate >= c.StartDate && request.StartDate <= c.EndDate) ||
                 (request.EndDate >= c.StartDate && request.EndDate <= c.EndDate)), 
                cancellationToken);

        if (overlapping)
            return Result<bool>.Failure("توجد فترة تقييم أخرى متداخلة مع هذه التواريخ");

        cycle.CycleNameAr = request.CycleName;
        cycle.StartDate = request.StartDate;
        cycle.EndDate = request.EndDate;
        cycle.IsActive = 1;
        cycle.UpdatedBy = _currentUserService.UserId;
        cycle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم تحديث فترة التقييم بنجاح");
    }
}

public class UpdateAppraisalCycleCommandValidator : AbstractValidator<UpdateAppraisalCycleCommand>
{
    public UpdateAppraisalCycleCommandValidator()
    {
        RuleFor(x => x.CycleId)
            .GreaterThan(0).WithMessage("معرف فترة التقييم مطلوب");

        RuleFor(x => x.CycleName)
            .NotEmpty().WithMessage("اسم فترة التقييم مطلوب")
            .MaximumLength(100).WithMessage("الاسم لا يمكن أن يتجاوز 100 حرف");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("حالة الفترة مطلوبة");
    }
}
