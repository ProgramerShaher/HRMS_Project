using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Applications.Commands.ChangeStatus;

/// <summary>
/// أمر تغيير حالة طلب التوظيف (Applied → Shortlisted → Rejected)
/// </summary>
public class ChangeApplicationStatusCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف الطلب
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// الحالة الجديدة (APPLIED, SHORTLISTED, REJECTED)
    /// </summary>
    public string NewStatus { get; set; } = string.Empty;

    /// <summary>
    /// سبب الرفض (مطلوب إذا كانت الحالة REJECTED)
    /// </summary>
    public string? RejectionReason { get; set; }
}

public class ChangeApplicationStatusCommandHandler : IRequestHandler<ChangeApplicationStatusCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ChangeApplicationStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(ChangeApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.JobApplications
            .FirstOrDefaultAsync(a => a.AppId == request.AppId, cancellationToken);

        if (application == null)
            return Result<bool>.Failure("طلب التوظيف غير موجود");

        // ✅ التحقق من الحالة الجديدة
        var validStatuses = new[] { "APPLIED", "SHORTLISTED", "REJECTED" };
        if (!validStatuses.Contains(request.NewStatus))
            return Result<bool>.Failure("الحالة غير صالحة");

        // ✅ إذا كانت الحالة REJECTED، يجب تقديم سبب الرفض
        if (request.NewStatus == "REJECTED" && string.IsNullOrWhiteSpace(request.RejectionReason))
            return Result<bool>.Failure("سبب الرفض مطلوب");

        // تحديث الحالة
        application.Status = request.NewStatus;
        
        if (request.NewStatus == "REJECTED")
        {
            application.RejectionReason = request.RejectionReason;
        }

        application.UpdatedBy = _currentUserService.UserId;
        application.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, $"تم تحديث حالة الطلب إلى {request.NewStatus}");
    }
}

public class ChangeApplicationStatusCommandValidator : AbstractValidator<ChangeApplicationStatusCommand>
{
    public ChangeApplicationStatusCommandValidator()
    {
        RuleFor(x => x.AppId)
            .GreaterThan(0).WithMessage("معرف الطلب مطلوب");

        RuleFor(x => x.NewStatus)
            .NotEmpty().WithMessage("الحالة الجديدة مطلوبة")
            .Must(s => new[] { "APPLIED", "SHORTLISTED", "REJECTED" }.Contains(s))
            .WithMessage("الحالة يجب أن تكون: APPLIED, SHORTLISTED, REJECTED");

        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("سبب الرفض مطلوب عند رفض الطلب")
            .When(x => x.NewStatus == "REJECTED");

        RuleFor(x => x.RejectionReason)
            .MaximumLength(200).WithMessage("سبب الرفض لا يمكن أن يتجاوز 200 حرف")
            .When(x => !string.IsNullOrEmpty(x.RejectionReason));
    }
}
