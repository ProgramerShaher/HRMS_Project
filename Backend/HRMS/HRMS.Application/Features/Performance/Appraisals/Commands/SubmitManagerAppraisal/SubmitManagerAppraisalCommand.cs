using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRMS.Application.Features.Performance.Appraisals.Commands.SubmitManagerAppraisal;

public class SubmitManagerAppraisalCommand : IRequest<Result<int>>
{
    public int AppraisalId { get; set; }
    public List<ManagerScoreDto> Scores { get; set; } = new();
    public string? ManagerComment { get; set; }
}

public class ManagerScoreDto
{
    public long DetailId { get; set; }
    public decimal ManagerScore { get; set; }
    public string? Comments { get; set; }
}

public class SubmitManagerAppraisalCommandValidator : AbstractValidator<SubmitManagerAppraisalCommand>
{
    public SubmitManagerAppraisalCommandValidator()
    {
        RuleFor(x => x.AppraisalId).GreaterThan(0);
        RuleForEach(x => x.Scores).ChildRules(k => {
            k.RuleFor(x => x.ManagerScore).InclusiveBetween(0, 100);
        });
    }
}

public class SubmitManagerAppraisalCommandHandler : IRequestHandler<SubmitManagerAppraisalCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public SubmitManagerAppraisalCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(SubmitManagerAppraisalCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var appraisal = await _context.EmployeeAppraisals
                .Include(a => a.Details)
                .FirstOrDefaultAsync(a => a.AppraisalId == request.AppraisalId && a.IsDeleted == 0, cancellationToken);

            if (appraisal == null) return Result<int>.Failure("التقييم غير موجود.");
            if (appraisal.Status != "MANAGER_EVALUATION") return Result<int>.Failure("هذا التقييم ليس في مرحلة تقييم المدير.");

            // Update scores
            foreach (var score in request.Scores)
            {
                var detail = appraisal.Details.FirstOrDefault(d => d.DetailId == score.DetailId);
                if (detail != null)
                {
                    detail.ManagerScore = score.ManagerScore;
                    if (!string.IsNullOrEmpty(score.Comments))
                    {
                        detail.Comments = (detail.Comments + " | " + score.Comments).Trim(' ', '|');
                    }
                }
            }

            appraisal.Comments = request.ManagerComment;
            appraisal.Status = "HR_REVIEW"; // Move to HR
            appraisal.UpdatedBy = _currentUserService.UserId;
            appraisal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<int>.Success(appraisal.AppraisalId, "تم حفظ تقييم المدير وإرساله للموارد البشرية.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<int>.Failure($"خطأ داخلي: {ex.Message}");
        }
    }
}
