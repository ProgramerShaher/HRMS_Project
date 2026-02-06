using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Performance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.KpiLibrary.Commands.Create;

/// <summary>
/// أمر إنشاء مؤشر أداء (KPI) جديد
/// </summary>
public class CreateKpiCommand : IRequest<Result<int>>
{
    public string KpiNameAr { get; set; } = string.Empty;
    public string? KpiDescription { get; set; }
    public string? Category { get; set; }
    public string? MeasurementUnit { get; set; }
}

public class CreateKpiCommandHandler : IRequestHandler<CreateKpiCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateKpiCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(CreateKpiCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.KpiLibraries
            .AnyAsync(k => k.KpiNameAr == request.KpiNameAr, cancellationToken);

        if (exists)
            return Result<int>.Failure("يوجد KPI بنفس الاسم مسبقاً");

        var kpi = new HRMS.Core.Entities.Performance.KpiLibrary
        {
            KpiNameAr = request.KpiNameAr,
            KpiDescription = request.KpiDescription,
            Category = request.Category,
            MeasurementUnit = request.MeasurementUnit,
            CreatedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.KpiLibraries.Add(kpi);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(kpi.KpiId, "تم إنشاء مؤشر الأداء بنجاح");
    }
}

public class CreateKpiCommandValidator : AbstractValidator<CreateKpiCommand>
{
    public CreateKpiCommandValidator()
    {
        RuleFor(x => x.KpiNameAr)
            .NotEmpty().WithMessage("اسم مؤشر الأداء مطلوب")
            .MaximumLength(200).WithMessage("الاسم لا يمكن أن يتجاوز 200 حرف");

        RuleFor(x => x.Category)
            .MaximumLength(50).WithMessage("الفئة لا يمكن أن تتجاوز 50 حرف")
            .When(x => !string.IsNullOrEmpty(x.Category));

        RuleFor(x => x.MeasurementUnit)
            .MaximumLength(20).WithMessage("وحدة القياس لا يمكن أن تتجاوز 20 حرف")
            .When(x => !string.IsNullOrEmpty(x.MeasurementUnit));
    }
}
