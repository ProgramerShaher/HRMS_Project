using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.Dashboard.Queries.GetApprovalStats;

// DTO
public record ApprovalStatsDto(int TotalPending, int ApprovedToday, int RejectedThisMonth);

// Query
public record GetApprovalStatsQuery : IRequest<Result<ApprovalStatsDto>>;

// Handler
public class GetApprovalStatsQueryHandler : IRequestHandler<GetApprovalStatsQuery, Result<ApprovalStatsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetApprovalStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ApprovalStatsDto>> Handle(GetApprovalStatsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        // استخدام استعلام مجمع (Optimized Aggregation)
        // بدل 3 استعلامات منفصلة، نحاول تجميعها لكن EF Core قد لا يدعم GroupBy معقد بسهولة
        // سنستخدم 3 عدادات سريعة لأنها مفهرسة غالباً

        var totalPending = await _context.LeaveRequests
            .CountAsync(r => r.Status == "PENDING" && r.IsDeleted == 0, cancellationToken);

        var approvedToday = await _context.WorkflowApprovals
            .CountAsync(w => w.RequestType == "LEAVE" 
                          && w.Status == "APPROVED" 
                          && w.ApprovalDate >= today
                          && w.IsDeleted == 0, cancellationToken);

        var rejectedThisMonth = await _context.WorkflowApprovals
            .CountAsync(w => w.RequestType == "LEAVE" 
                          && w.Status == "REJECTED" 
                          &&(w.ApprovalDate >= startOfMonth)
                          && w.IsDeleted == 0, cancellationToken);

        return Result<ApprovalStatsDto>.Success(new ApprovalStatsDto(totalPending, approvedToday, rejectedThisMonth));
    }
}
