using System.Collections.Generic;

namespace HRMS.Application.DTOs.Leaves
{
    public record LeaveDashboardStatsDto
    {
        public decimal TotalEntitlement { get; set; }
        public decimal TotalRequestedDays { get; set; }
        public decimal ConsumedDays { get; set; }
        public decimal RemainingDays { get; set; }
        public int PendingRequestsCount { get; set; }
        public int ApprovedRequestsCount { get; set; }
        public int RejectedRequestsCount { get; set; }
        public List<LeaveTypeSummaryDto> LeaveTypeSummaries { get; set; } = new();
    }

    public record LeaveTypeSummaryDto
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeNameAr { get; set; } = string.Empty;
        public string LeaveTypeNameEn { get; set; } = string.Empty;
        public decimal TotalDays { get; set; }
        public decimal ConsumedDays { get; set; }
        public decimal RemainingDays { get; set; }
    }
}
