namespace HRMS.Application.DTOs.Reports.Analytics;

public class DailyAttendanceDetailsDto
{
    public long RecordId { get; set; }
    public DateTime Date { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string PlannedShift { get; set; } = string.Empty;
    public string InTime { get; set; } = string.Empty;
    public string OutTime { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int LateMinutes { get; set; }
    public int OvertimeMinutes { get; set; }
}

public class MonthlyPayslipReportDto
{
    public long PayslipId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public decimal TotalAllowances { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
    public int AbsenceDays { get; set; }
    public int LateMinutes { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
}

public class LeaveHistoryDto
{
    public long RequestId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
}

public class RecruitmentReportDto
{
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Applied, Interview, Offer, Hired, Rejected
    public DateTime ApplicationDate { get; set; }
}

public class PerformanceReportDto
{
    public int AppraisalId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string CycleName { get; set; } = string.Empty; // e.g., "Q1 2024"
    public decimal OverallScore { get; set; }
    public string Rating { get; set; } = string.Empty; // Excellent, Good, etc.
    public string EvaluatorName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Completed, Pending
}
