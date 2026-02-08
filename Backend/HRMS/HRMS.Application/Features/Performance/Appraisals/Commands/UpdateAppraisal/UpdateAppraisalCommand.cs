using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Performance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Appraisals.Commands.UpdateAppraisal;

public class UpdateAppraisalCommand : IRequest<Result<int>>
{
    public int AppraisalId { get; set; }
    public string? Comments { get; set; }
    public List<UpdateAppraisalDetailDto> Details { get; set; } = new();
}

public class UpdateAppraisalDetailDto
{
    public int KpiId { get; set; }
    public decimal EmployeeScore { get; set; }
    public decimal ManagerScore { get; set; }
    public string? Comments { get; set; }
}

public class UpdateAppraisalCommandValidator : AbstractValidator<UpdateAppraisalCommand>
{
    public UpdateAppraisalCommandValidator()
    {
        RuleFor(x => x.AppraisalId).GreaterThan(0);
        RuleFor(x => x.Details).NotEmpty().WithMessage("يجب إدخال تفاصيل التقييم");
    }
}

public class UpdateAppraisalCommandHandler : IRequestHandler<UpdateAppraisalCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAppraisalCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(UpdateAppraisalCommand request, CancellationToken cancellationToken)
    {
        var appraisal = await _context.EmployeeAppraisals
            .Include(a => a.Details)
            .FirstOrDefaultAsync(a => a.AppraisalId == request.AppraisalId && a.IsDeleted == 0, cancellationToken);

        if (appraisal == null)
            return Result<int>.Failure("التقييم غير موجود");

        // Update basic info
        appraisal.Comments = request.Comments;
        appraisal.UpdatedBy = _currentUserService.UserId;
        appraisal.UpdatedAt = DateTime.UtcNow;

        // Update Details logic (Simplified for now - Replace/Update)
        // For a full ERP, we might want to track history, but here we update existing or add new
        
        decimal totalScore = 0;
        decimal totalWeight = 0;

        foreach (var detailDto in request.Details)
        {
            var kpi = await _context.KpiLibraries.FindAsync(detailDto.KpiId);
            if (kpi == null) continue;

            var existingDetail = appraisal.Details.FirstOrDefault(d => d.KpiId == detailDto.KpiId);
            
            if (existingDetail != null)
            {
                existingDetail.EmployeeScore = detailDto.EmployeeScore;
                existingDetail.ManagerScore = detailDto.ManagerScore;
                existingDetail.Comments = detailDto.Comments;
                // Recalculate FinalScore for this KPI (Weighted)
                existingDetail.FinalScore = (detailDto.ManagerScore * kpi.Weight) / 100;
            }
            else
            {
                var newDetail = new AppraisalDetail
                {
                    AppraisalId = appraisal.AppraisalId,
                    KpiId = detailDto.KpiId,
                    EmployeeScore = detailDto.EmployeeScore,
                    ManagerScore = detailDto.ManagerScore,
                    Comments = detailDto.Comments,
                    FinalScore = (detailDto.ManagerScore * kpi.Weight) / 100
                };
                _context.AppraisalDetails.Add(newDetail);
            }

            totalScore += (detailDto.ManagerScore * kpi.Weight) / 100;
            totalWeight += kpi.Weight;
        }

        appraisal.TotalScore = totalScore;
        appraisal.Status = "COMPLETED"; // Or keep as is

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(appraisal.AppraisalId, "تم تحديث التقييم بنجاح");
    }
}
