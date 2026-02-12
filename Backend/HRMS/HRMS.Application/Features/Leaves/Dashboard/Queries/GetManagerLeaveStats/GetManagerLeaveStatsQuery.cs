using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HRMS.Application.DTOs.Leaves;

namespace HRMS.Application.Features.Leaves.Dashboard.Queries.GetManagerLeaveStats
{
    public record GetManagerLeaveStatsQuery() : IRequest<Result<LeaveDashboardStatsDto>>;

    public class GetManagerLeaveStatsQueryHandler : IRequestHandler<GetManagerLeaveStatsQuery, Result<LeaveDashboardStatsDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetManagerLeaveStatsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<LeaveDashboardStatsDto>> Handle(GetManagerLeaveStatsQuery request, CancellationToken cancellationToken)
        {
            var year = (short)DateTime.Today.Year;

            // 1. Get all requests
            var requests = await _context.LeaveRequests
                .Where(r => r.IsDeleted == 0)
                .Select(r => new { r.Status, r.DaysCount, r.StartDate })
                .ToListAsync(cancellationToken);

            // 2. Get all balances for current year
            var balances = await _context.LeaveBalances
                .Where(b => b.Year == year && b.IsDeleted == 0)
                .Select(b => b.CurrentBalance)
                .ToListAsync(cancellationToken);

            var pending = requests.Count(r => r.Status == "PENDING" || r.Status == "MANAGER_APPROVED" || r.Status == "HR_APPROVED");
            var approved = requests.Count(r => r.Status == "APPROVED");
            var rejected = requests.Count(r => r.Status == "REJECTED");
            
            var totalConsumedInYear = requests
                .Where(r => r.Status == "APPROVED" && r.StartDate.Year == year)
                .Sum(r => (decimal)r.DaysCount);

            var totalRemaining = balances.Sum();

            var stats = new LeaveDashboardStatsDto
            {
                TotalEntitlement = totalRemaining + totalConsumedInYear,
                TotalRequestedDays = requests.Sum(r => r.DaysCount),
                ConsumedDays = totalConsumedInYear,
                RemainingDays = totalRemaining,
                PendingRequestsCount = pending,
                ApprovedRequestsCount = approved,
                RejectedRequestsCount = rejected,
                LeaveTypeSummaries = new List<LeaveTypeSummaryDto>()
            };

            return Result<LeaveDashboardStatsDto>.Success(stats);
        }
    }
}
