using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HRMS.Application.DTOs.Leaves;

namespace HRMS.Application.Features.Leaves.Dashboard.Queries.GetEmployeeLeaveStats
{
    public record GetEmployeeLeaveStatsQuery(int EmployeeId) : IRequest<Result<LeaveDashboardStatsDto>>;

    public class GetEmployeeLeaveStatsQueryHandler : IRequestHandler<GetEmployeeLeaveStatsQuery, Result<LeaveDashboardStatsDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetEmployeeLeaveStatsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<LeaveDashboardStatsDto>> Handle(GetEmployeeLeaveStatsQuery request, CancellationToken cancellationToken)
        {
            var year = (short)DateTime.Today.Year;

            // 1. Get all relevant Leave Types (Deductible)
            var leaveTypes = await _context.LeaveTypes
                .Where(lt => lt.IsDeleted == 0 && lt.IsDeductible == 1)
                .ToListAsync(cancellationToken);

            // 2. Get all Requests for the employee in the current year
            var requests = await _context.LeaveRequests
                .Where(r => r.EmployeeId == request.EmployeeId && r.IsDeleted == 0)
                .Select(r => new { r.Status, r.DaysCount, r.StartDate, r.LeaveTypeId })
                .ToListAsync(cancellationToken);

            // 3. Get existing Balances for current year
            var balances = await _context.EmployeeLeaveBalances
                .Where(b => b.EmployeeId == request.EmployeeId && b.Year == year && b.IsDeleted == 0)
                .ToDictionaryAsync(b => b.LeaveTypeId, cancellationToken);

            var pending = requests.Count(r => r.Status == "PENDING" || r.Status == "MANAGER_APPROVED" || r.Status == "HR_APPROVED");
            var approved = requests.Count(r => r.Status == "APPROVED");
            var rejected = requests.Count(r => r.Status == "REJECTED");
            var totalRequested = requests.Sum(r => r.DaysCount);
            
            decimal totalConsumed = requests
                .Where(r => r.Status == "APPROVED" && r.StartDate.Year == year)
                .Sum(r => (decimal)r.DaysCount);

            var summaries = new List<LeaveTypeSummaryDto>();
            decimal totalRemaining = 0;
            decimal totalEntitlement = 0;

            foreach (var lt in leaveTypes)
            {
                var consumedForType = requests
                    .Where(r => r.LeaveTypeId == lt.LeaveTypeId && r.Status == "APPROVED" && r.StartDate.Year == year)
                    .Sum(r => (decimal)r.DaysCount);

                decimal remainingForType = 0;
                decimal entitlementForType = 0;

                if (balances.TryGetValue(lt.LeaveTypeId, out var b))
                {
                    remainingForType = b.CurrentBalance;
                    entitlementForType = b.CurrentBalance + consumedForType;
                }
                else
                {
                    // Fallback to default days if no balance record exists
                    entitlementForType = lt.DefaultDays;
                    remainingForType = lt.DefaultDays - consumedForType;
                }

                totalRemaining += remainingForType;
                totalEntitlement += entitlementForType;

                summaries.Add(new LeaveTypeSummaryDto
                {
                    LeaveTypeId = lt.LeaveTypeId,
                    LeaveTypeNameAr = lt.LeaveNameAr,
                    LeaveTypeNameEn = lt.LeaveNameEn ?? string.Empty,
                    TotalDays = entitlementForType,
                    ConsumedDays = consumedForType,
                    RemainingDays = remainingForType
                });
            }

            var stats = new LeaveDashboardStatsDto
            {
                TotalEntitlement = totalEntitlement,
                TotalRequestedDays = totalRequested,
                ConsumedDays = totalConsumed,
                RemainingDays = totalRemaining,
                PendingRequestsCount = pending,
                ApprovedRequestsCount = approved,
                RejectedRequestsCount = rejected,
                LeaveTypeSummaries = summaries
            };

            return Result<LeaveDashboardStatsDto>.Success(stats);
        }
    }
}
