using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Applications.Commands.ChangeStatus;

public class ChangeApplicationStatusCommand : IRequest<Result<bool>>
{
    public int AppId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class ChangeApplicationStatusCommandValidator : AbstractValidator<ChangeApplicationStatusCommand>
{
    public ChangeApplicationStatusCommandValidator()
    {
        RuleFor(x => x.AppId).GreaterThan(0);
        RuleFor(x => x.Status).NotEmpty().Must(s => new[] { "NEW", "SCREENING", "INTERVIEW", "OFFERED", "HIRED", "REJECTED" }.Contains(s))
            .WithMessage("حالة الطلب غير صالحة");
    }
}

public class ChangeApplicationStatusCommandHandler : IRequestHandler<ChangeApplicationStatusCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ChangeApplicationStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(ChangeApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var application = await _context.JobApplications
            .FirstOrDefaultAsync(a => a.AppId == request.AppId && a.IsDeleted == 0, cancellationToken);

        if (application == null)
            return Result<bool>.Failure("طلب التوظيف غير موجود");

        application.Status = request.Status;
        // application.Notes = request.Notes; // If Notes field exists
        application.UpdatedBy = _currentUserService.UserId;
        application.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم تغيير حالة الطلب بنجاح");
    }
}
