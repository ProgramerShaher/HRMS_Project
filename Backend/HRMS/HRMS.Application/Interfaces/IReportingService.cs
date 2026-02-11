using HRMS.Application.DTOs.Reports.Analytics;

namespace HRMS.Application.Interfaces;

public interface IReportingService
{
    // HR & Personnel Analytics
    Task<HROverviewDto> GetHROverviewAsync();
    
    // Attendance Analytics
    Task<AttendanceStatsDto> GetAttendanceStatsAsync(DateTime startDate, DateTime endDate);
    
    // Payroll Analytics
    Task<PayrollStatsDto> GetPayrollStatsAsync(int month, int year);

    // Operational Reports
    Task<List<EmployeeCensusDto>> GetEmployeeCensusReportAsync(int? departmentId = null, string? status = null);
    
    Task<List<DailyAttendanceDetailsDto>> GetDetailedAttendanceReportAsync(DateTime startDate, DateTime endDate, int? departmentId = null);
    
    Task<List<MonthlyPayslipReportDto>> GetMonthlyPayslipReportAsync(int month, int year, int? departmentId = null);

    Task<List<LeaveHistoryDto>> GetLeaveHistoryReportAsync(DateTime startDate, DateTime endDate, int? departmentId = null);

    // Module Specific
    Task<List<RecruitmentReportDto>> GetRecruitmentReportAsync(DateTime startDate, DateTime endDate, string? status = null);
    
    Task<List<PerformanceReportDto>> GetPerformanceReportAsync(int cycleId, int? departmentId = null);
}
