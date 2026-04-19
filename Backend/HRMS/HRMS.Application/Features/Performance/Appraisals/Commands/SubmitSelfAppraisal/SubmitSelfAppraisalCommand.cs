using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRMS.Application.Features.Performance.Appraisals.Commands.SubmitSelfAppraisal;

public class SubmitSelfAppraisalCommand : IRequest<Result<int>>
{
    public int AppraisalId { get; set; }
    public List<SelfScoreDto> Scores { get; set; } = new();
    public string? EmployeeComment { get; set; }
}

public class SelfScoreDto
{
    public long DetailId { get; set; }
    public decimal EmployeeScore { get; set; }
    public string? Comments { get; set; }
}

public class SubmitSelfAppraisalCommandValidator : AbstractValidator<SubmitSelfAppraisalCommand>
{
    public SubmitSelfAppraisalCommandValidator()
    {
        RuleFor(x => x.AppraisalId).GreaterThan(0);
        RuleForEach(x => x.Scores).ChildRules(k => {
            k.RuleFor(x => x.EmployeeScore).InclusiveBetween(0, 100);
        });
    }
}

public class SubmitSelfAppraisalCommandHandler : IRequestHandler<SubmitSelfAppraisalCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public SubmitSelfAppraisalCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(SubmitSelfAppraisalCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var appraisal = await _context.EmployeeAppraisals
                .Include(a => a.Details)
                .FirstOrDefaultAsync(a => a.AppraisalId == request.AppraisalId && a.IsDeleted == 0, cancellationToken);

            if (appraisal == null) return Result<int>.Failure("التقييم غير موجود.");
            if (appraisal.Status != "SELF_EVALUATION") return Result<int>.Failure("هذا التقييم ليس في مرحلة التقييم الذاتي.");

            // Update scores
            foreach (var score in request.Scores)
            {
                var detail = appraisal.Details.FirstOrDefault(d => d.DetailId == score.DetailId);
                if (detail != null)
                {
                    detail.EmployeeScore = score.EmployeeScore;
                    detail.Comments = score.Comments; // append or overwrite
                }
            }

            appraisal.EmployeeComment = request.EmployeeComment;
            appraisal.Status = "MANAGER_EVALUATION"; // Move to next phase
            appraisal.UpdatedBy = _currentUserService.UserId;
            appraisal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<int>.Success(appraisal.AppraisalId, "تم تسليم التقييم الذاتي لإدارتك بنجاح.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<int>.Failure($"خطأ داخلي: {ex.Message}");
        }
    }
}
