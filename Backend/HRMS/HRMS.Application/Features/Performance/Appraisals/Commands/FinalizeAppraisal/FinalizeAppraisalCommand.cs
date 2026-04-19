using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Appraisals.Commands.FinalizeAppraisal;

public class FinalizeAppraisalCommand : IRequest<Result<int>>
{
    public int AppraisalId { get; set; }
    public List<FinalScoreDto> Scores { get; set; } = new();
}

public class FinalScoreDto
{
    public long DetailId { get; set; }
    public decimal FinalScore { get; set; }
}

public class FinalizeAppraisalCommandValidator : AbstractValidator<FinalizeAppraisalCommand>
{
    public FinalizeAppraisalCommandValidator()
    {
        RuleFor(x => x.AppraisalId).GreaterThan(0);
        RuleForEach(x => x.Scores).ChildRules(k => {
            k.RuleFor(x => x.FinalScore).InclusiveBetween(0, 100);
        });
    }
}

public class FinalizeAppraisalCommandHandler : IRequestHandler<FinalizeAppraisalCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public FinalizeAppraisalCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(FinalizeAppraisalCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // ✅ FIX #2: ThenInclude Kpi to access Weight for Weighted Average
            var appraisal = await _context.EmployeeAppraisals
                .Include(a => a.Details)
                    .ThenInclude(d => d.Kpi)
                .FirstOrDefaultAsync(a => a.AppraisalId == request.AppraisalId && a.IsDeleted == 0, cancellationToken);

            if (appraisal == null) return Result<int>.Failure("التقييم غير موجود.");
            if (appraisal.Status != "HR_REVIEW") return Result<int>.Failure("هذا التقييم ليس جاهزاً للاعتماد النهائي.");

            // ✅ FIX: Weighted Average — Σ(score × weight) / Σ(weights)
            decimal totalWeightedScore = 0;
            decimal totalWeight = 0;

            foreach (var scoreDto in request.Scores)
            {
                var detail = appraisal.Details.FirstOrDefault(d => d.DetailId == scoreDto.DetailId);
                if (detail != null)
                {
                    detail.FinalScore = scoreDto.FinalScore;
                    detail.UpdatedBy = _currentUserService.UserId;
                    detail.UpdatedAt = DateTime.UtcNow;

                    // كل KPI له وزنه — إذا كان الوزن صفراً نستخدم 1 كافتراضي
                    decimal kpiWeight = (detail.Kpi?.Weight > 0) ? detail.Kpi!.Weight : 1.0m;
                    totalWeightedScore += scoreDto.FinalScore * kpiWeight;
                    totalWeight += kpiWeight;
                }
            }

            appraisal.FinalScore = totalWeight > 0
                ? Math.Round(totalWeightedScore / totalWeight, 2)
                : 0;

            appraisal.Grade = CalculateGrade(appraisal.FinalScore.Value);
            appraisal.Status = "COMPLETED";
            appraisal.UpdatedBy = _currentUserService.UserId;
            appraisal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<int>.Success(appraisal.AppraisalId,
                $"تم اعتماد التقييم نهائياً — الدرجة الموزونة: {appraisal.FinalScore} ({appraisal.Grade})");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<int>.Failure($"خطأ داخلي: {ex.Message}");
        }
    }

    private string CalculateGrade(decimal score)
    {
        if (score >= 90) return "ممتاز";
        if (score >= 80) return "جيد جداً";
        if (score >= 70) return "جيد";
        if (score >= 60) return "مقبول";
        return "ضعيف";
    }
}
