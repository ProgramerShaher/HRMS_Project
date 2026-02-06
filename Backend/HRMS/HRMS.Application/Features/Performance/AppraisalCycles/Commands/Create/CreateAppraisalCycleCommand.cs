using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Performance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.AppraisalCycles.Commands.Create;

/// <summary>
/// أمر إنشاء فترة تقييم جديدة
/// </summary>
public class CreateAppraisalCycleCommand : IRequest<Result<int>>
{
    public string CycleName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "PLANNING"; // PLANNING, ACTIVE, COMPLETED
}

public class CreateAppraisalCycleCommandHandler : IRequestHandler<CreateAppraisalCycleCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateAppraisalCycleCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(CreateAppraisalCycleCommand request, CancellationToken cancellationToken)
    {
        // التحقق من عدم تداخل التواريخ
        var overlapping = await _context.AppraisalCycles
            .AnyAsync(c => 
                (request.StartDate >= c.StartDate && request.StartDate <= c.EndDate) ||
                (request.EndDate >= c.StartDate && request.EndDate <= c.EndDate), 
                cancellationToken);

        if (overlapping)
            return Result<int>.Failure("توجد فترة تقييم أخرى متداخلة مع هذه التواريخ");

        var cycle = new AppraisalCycle
        {
            CycleNameAr = request.CycleName,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = 1,
            CreatedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.AppraisalCycles.Add(cycle);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(cycle.CycleId, "تم إنشاء فترة التقييم بنجاح");
    }
}

public class CreateAppraisalCycleCommandValidator : AbstractValidator<CreateAppraisalCycleCommand>
{
    public CreateAppraisalCycleCommandValidator()
    {
        RuleFor(x => x.CycleName)
            .NotEmpty().WithMessage("اسم فترة التقييم مطلوب")
            .MaximumLength(100).WithMessage("الاسم لا يمكن أن يتجاوز 100 حرف");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاريخ البداية مطلوب");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("تاريخ النهاية مطلوب")
            .GreaterThan(x => x.StartDate).WithMessage("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("حالة الفترة مطلوبة")
            .Must(s => new[] { "PLANNING", "ACTIVE", "COMPLETED", "CANCELLED" }.Contains(s))
            .WithMessage("الحالة يجب أن تكون: PLANNING, ACTIVE, COMPLETED, CANCELLED");
    }
}
