namespace HRMS.Application.DTOs.Reports.Analytics;

public class HROverviewDto
{
    public int TotalEmployees { get; set; }
    public int TotalDepartments { get; set; }
    public int NewHiresThisMonth { get; set; }
    public List<DepartmentDistributionDto> DepartmentDistribution { get; set; } = new();
    public List<DocumentExpiryDto> UpcomingDocumentExpirations { get; set; } = new();
}

public class DepartmentDistributionDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
}

public class DocumentExpiryDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int DaysRemaining { get; set; }
}

public class AttendanceStatsDto
{
    public int TotalWorkingDays { get; set; }
    public int TotalPresent { get; set; }
    public int TotalAbsent { get; set; }
    public int TotalLate { get; set; }
    public double AbsenteeismRate { get; set; }
    public List<DailyAttendanceSummaryDto> DailyTrend { get; set; } = new();
    public List<EmployeeAttendanceRankingDto> TopLateEmployees { get; set; } = new();
}

public class DailyAttendanceSummaryDto
{
    public DateTime Date { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int LateCount { get; set; }
}

public class EmployeeAttendanceRankingDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int Count { get; set; } // Minutes or Days
}

public class PayrollStatsDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalNetPay { get; set; }
    public decimal TotalBasicSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public List<PayrollBreakdownDto> DepartmentCost { get; set; } = new();
}

public class PayrollBreakdownDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
