using HRMS.Application.DTOs.Payroll.Processing;

namespace HRMS.Application.DTOs.Reports.Analytics;

/// <summary>
/// Data Transfer Object for the Comprehensive HR Dashboard.
/// Aggregates metrics from Attendance, Personnel, Leaves, Payroll, and Requests.
/// </summary>
public class ComprehensiveDashboardDto
{
    public DateTime ReportDate { get; set; }
    public AttendanceMetricsDto AttendanceMetrics { get; set; } = new();
    public PersonnelMetricsDto PersonnelMetrics { get; set; } = new();
    public RequestsMetricsDto RequestsMetrics { get; set; } = new();
    public FinancialMetricsDto FinancialMetrics { get; set; } = new();
    public List<HolidayMetricDto> HolidayMetrics { get; set; } = new();

    // New Sections
    public WeeklyAnalyticsDto WeeklyMetrics { get; set; } = new();
    public MonthlyAnalyticsDto MonthlyMetrics { get; set; } = new();
}

public class AttendanceMetricsDto
{
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLeaves { get; set; } // Approved leaves for today
    public int TotalLate { get; set; }
    public Dictionary<string, int> ShiftDistribution { get; set; } = new(); // e.g., "Morning": 50, "Night": 10
}

public class PersonnelMetricsDto
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int InactiveEmployees { get; set; }
    public List<DepartmentStatDto> DepartmentStats { get; set; } = new();
}

public class DepartmentStatDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
}

public class RequestsMetricsDto
{
    public int PendingLeaveRequests { get; set; }
    public int PendingOvertimeRequests { get; set; }
    public int PendingLoanRequests { get; set; }
    public int PendingShiftSwaps { get; set; }
    public int PendingPermissions { get; set; }
}

public class FinancialMetricsDto
{
    public decimal TotalPendingSalaries { get; set; } // Estimated from pending payroll runs
    public Dictionary<string, decimal> PendingSalariesByDepartment { get; set; } = new();
    public decimal TotalActiveLoansAmount { get; set; }
    public int ActiveLoansCount { get; set; }
    public PayrollRunSummary? LastPayrollSummary { get; set; }
}

public class HolidayMetricDto
{
    public string HolidayType { get; set; } = string.Empty; // Religious, National, Official
    public int DaysCount { get; set; }
    public int HolidaysCount { get; set; }
}
