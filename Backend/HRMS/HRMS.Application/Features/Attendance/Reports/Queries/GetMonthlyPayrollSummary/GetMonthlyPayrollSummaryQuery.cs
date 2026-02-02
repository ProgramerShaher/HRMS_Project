using MediatR;
using HRMS.Core.Utilities;

namespace HRMS.Application.Features.Attendance.Reports.Queries.GetMonthlyPayrollSummary;

public record PayrollAttendanceSummaryDto
{
    public int EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string FullNameAr { get; init; } = string.Empty;
    public string DepartmentName { get; init; } = string.Empty;
    public int TotalDeepLateMinutes { get; init; } // تأخير كبير يستوجب الخصم
    public int TotalShortLateMinutes { get; init; } // تأخير قصير
    public int TotalAbsenceDays { get; init; }
    public int TotalSickLeaveDays { get; init; }
    public int TotalUnpaidLeaveDays { get; init; }
    public int TotalOvertimeMinutes { get; init; }
    public decimal ProposedDeductionAmount { get; init; } // Placeholder logic
}

public record GetMonthlyPayrollSummaryQuery(int Month, int Year) : IRequest<Result<List<PayrollAttendanceSummaryDto>>>;
