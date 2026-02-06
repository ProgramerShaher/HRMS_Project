using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.KpiLibrary.Commands.Update;

public class UpdateKpiCommand : IRequest<Result<bool>>
{
    public int KpiId { get; set; }
    public string KpiNameAr { get; set; } = string.Empty;
    public string? KpiDescription { get; set; }
    public string? Category { get; set; }
    public string? MeasurementUnit { get; set; }
}

public class UpdateKpiCommandHandler : IRequestHandler<UpdateKpiCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateKpiCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(UpdateKpiCommand request, CancellationToken cancellationToken)
    {
        var kpi = await _context.KpiLibraries
            .FirstOrDefaultAsync(k => k.KpiId == request.KpiId, cancellationToken);

        if (kpi == null)
            return Result<bool>.Failure("مؤشر الأداء غير موجود");

        var duplicateExists = await _context.KpiLibraries
            .AnyAsync(k => k.KpiNameAr == request.KpiNameAr && k.KpiId != request.KpiId, cancellationToken);

        if (duplicateExists)
            return Result<bool>.Failure("يوجد KPI آخر بنفس الاسم");

        kpi.KpiNameAr = request.KpiNameAr;
        kpi.KpiDescription = request.KpiDescription;
        kpi.Category = request.Category;
        kpi.MeasurementUnit = request.MeasurementUnit;
        kpi.UpdatedBy = _currentUserService.UserId;
        kpi.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم تحديث مؤشر الأداء بنجاح");
    }
}

public class UpdateKpiCommandValidator : AbstractValidator<UpdateKpiCommand>
{
    public UpdateKpiCommandValidator()
    {
        RuleFor(x => x.KpiId)
            .GreaterThan(0).WithMessage("معرف مؤشر الأداء مطلوب");

        RuleFor(x => x.KpiNameAr)
            .NotEmpty().WithMessage("اسم مؤشر الأداء مطلوب")
            .MaximumLength(200).WithMessage("الاسم لا يمكن أن يتجاوز 200 حرف");
    }
}
