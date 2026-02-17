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
    public PerformanceMetricsDto PerformanceMetrics { get; set; } = new();
    public SetupMetricsDto SetupMetrics { get; set; } = new();

    // New Sections
    public WeeklyAnalyticsDto WeeklyMetrics { get; set; } = new();
    public MonthlyAnalyticsDto MonthlyMetrics { get; set; } = new();
}

public class PerformanceMetricsDto
{
    public int ActiveAppraisalCycles { get; set; }
    public int PendingEvaluations { get; set; }
    public double AverageCompanyRating { get; set; }
}

public class SetupMetricsDto
{
    public int TotalDepartments { get; set; }
    public int TotalJobTitles { get; set; }
    public int TotalShiftTypes { get; set; }
    public int TotalActiveUsers { get; set; }
}

public class AttendanceMetricsDto
{
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLeaves { get; set; } // Approved leaves for today
    public int TotalLate { get; set; }
    public double AttendanceRate { get; set; } // Added
    public Dictionary<string, int> ShiftDistribution { get; set; } = new();
}

public class PersonnelMetricsDto
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int InactiveEmployees { get; set; }
    public int ExpiringDocumentsCount { get; set; } // Next 30 days
    public int NewHires { get; set; } // Added
    public int ActiveContracts { get; set; } // Added
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
    public double TotalNetSalary { get; set; } // Added
    public double TotalBasicSalary { get; set; } // Added
    public double TotalDeductions { get; set; } // Added
    public int PendingPayrollCount { get; set; } // Added

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
