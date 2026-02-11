namespace HRMS.Application.DTOs.Reports.Analytics;

public class AnalyticsHROverviewDto
{
    public int TotalEmployees { get; set; }
    public int TotalDepartments { get; set; }
    public int NewHiresThisMonth { get; set; }
    public List<AnalyticsDepartmentDistributionDto> DepartmentDistribution { get; set; } = new();
    public List<AnalyticsDocumentExpiryDto> UpcomingDocumentExpirations { get; set; } = new();
}

public class AnalyticsDepartmentDistributionDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
}

public class AnalyticsDocumentExpiryDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int DaysRemaining { get; set; }
}

public class AnalyticsAttendanceStatsDto
{
    public int TotalWorkingDays { get; set; }
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLate { get; set; }
    public double AbsenteeismRate { get; set; }
    public List<AnalyticsDailyAttendanceSummaryDto> DailyTrend { get; set; } = new();
    public List<AnalyticsEmployeeAttendanceRankingDto> TopLateEmployees { get; set; } = new();
}

public class AnalyticsDailyAttendanceSummaryDto
{
    public DateTime Date { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
}

public class AnalyticsEmployeeAttendanceRankingDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int Count { get; set; } // Minutes or Days
}

public class AnalyticsPayrollStatsDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalNetPay { get; set; }
    public decimal TotalBasicSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public List<AnalyticsPayrollBreakdownDto> DepartmentCost { get; set; } = new();
}

public class AnalyticsPayrollBreakdownDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
