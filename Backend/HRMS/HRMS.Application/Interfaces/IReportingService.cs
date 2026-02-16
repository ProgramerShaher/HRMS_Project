using HRMS.Application.DTOs.Reports.Analytics;

namespace HRMS.Application.Interfaces;

public interface IReportingService
{
    // HR & Personnel Analytics
    Task<AnalyticsHROverviewDto> GetHROverviewAsync();
    
    // Attendance Analytics
    Task<AnalyticsAttendanceStatsDto> GetAttendanceStatsAsync(DateTime startDate, DateTime endDate);
    
    // Payroll Analytics
    Task<AnalyticsPayrollStatsDto> GetPayrollStatsAsync(int month, int year);

    // Operational Reports (Keeping original names for now unless conflict found)
    Task<List<EmployeeCensusDto>> GetEmployeeCensusReportAsync(int? departmentId = null, string? status = null);
    
    Task<List<DailyAttendanceDetailsDto>> GetDetailedAttendanceReportAsync(DateTime startDate, DateTime endDate, int? departmentId = null);
    
    Task<List<MonthlyPayslipReportDto>> GetMonthlyPayslipReportAsync(int month, int year, int? departmentId = null);

    Task<List<LeaveHistoryDto>> GetLeaveHistoryReportAsync(DateTime startDate, DateTime endDate, int? departmentId = null);

    // Module Specific
    Task<List<RecruitmentReportDto>> GetRecruitmentReportAsync(DateTime startDate, DateTime endDate, string? status = null);
    
    Task<List<PerformanceReportDto>> GetPerformanceReportAsync(int cycleId, int? departmentId = null);

    // Comprehensive Dashboard
    Task<ComprehensiveDashboardDto> GetComprehensiveDashboardAsync();
}
