using HRMS.Application.DTOs.Reports.Analytics;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Reports;

[Route("api/[controller]")]
[ApiController]
// [Authorize] // Uncomment to secure endpoints
public class ReportsController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportsController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    // ═══════════════════════════════════════════════════════════
    // 1. Analytics Dashboards (Aggregated Data)
    // ═══════════════════════════════════════════════════════════

    [HttpGet("analytics/hr-overview")]
    public async Task<ActionResult<Result<AnalyticsHROverviewDto>>> GetHROverview()
    {
        var data = await _reportingService.GetHROverviewAsync();
        return Ok(Result<AnalyticsHROverviewDto>.Success(data));
    }

    [HttpGet("dashboard/comprehensive")]
    public async Task<ActionResult<Result<ComprehensiveDashboardDto>>> GetComprehensiveDashboard()
    {
        var data = await _reportingService.GetComprehensiveDashboardAsync();
        return Ok(Result<ComprehensiveDashboardDto>.Success(data));
    }

    [HttpGet("analytics/attendance")]
    public async Task<ActionResult<Result<AnalyticsAttendanceStatsDto>>> GetAttendanceStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var data = await _reportingService.GetAttendanceStatsAsync(startDate, endDate);
        return Ok(Result<AnalyticsAttendanceStatsDto>.Success(data));
    }

    [HttpGet("analytics/payroll")]
    public async Task<ActionResult<Result<AnalyticsPayrollStatsDto>>> GetPayrollStats([FromQuery] int month, [FromQuery] int year)
    {
        var data = await _reportingService.GetPayrollStatsAsync(month, year);
        return Ok(Result<AnalyticsPayrollStatsDto>.Success(data));
    }

    // ═══════════════════════════════════════════════════════════
    // 2. Operational Reports (Detailed Lists)
    // ═══════════════════════════════════════════════════════════

    [HttpGet("reports/employee-census")]
    public async Task<ActionResult<Result<List<EmployeeCensusDto>>>> GetEmployeeCensusReport([FromQuery] int? departmentId, [FromQuery] string? status)
    {
        var data = await _reportingService.GetEmployeeCensusReportAsync(departmentId, status);
        return Ok(Result<List<EmployeeCensusDto>>.Success(data));
    }

    [HttpGet("reports/attendance-detailed")]
    public async Task<ActionResult<Result<List<DailyAttendanceDetailsDto>>>> GetDetailedAttendanceReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? departmentId)
    {
        var data = await _reportingService.GetDetailedAttendanceReportAsync(startDate, endDate, departmentId);
        return Ok(Result<List<DailyAttendanceDetailsDto>>.Success(data));
    }

    [HttpGet("reports/payslip-monthly")]
    public async Task<ActionResult<Result<List<MonthlyPayslipReportDto>>>> GetMonthlyPayslipReport([FromQuery] int month, [FromQuery] int year, [FromQuery] int? departmentId)
    {
        var data = await _reportingService.GetMonthlyPayslipReportAsync(month, year, departmentId);
        return Ok(Result<List<MonthlyPayslipReportDto>>.Success(data));
    }

    [HttpGet("reports/leave-history")]
    public async Task<ActionResult<Result<List<LeaveHistoryDto>>>> GetLeaveHistoryReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? departmentId)
    {
        var data = await _reportingService.GetLeaveHistoryReportAsync(startDate, endDate, departmentId);
        return Ok(Result<List<LeaveHistoryDto>>.Success(data));
    }

    [HttpGet("reports/recruitment-pipeline")]
    public async Task<ActionResult<Result<List<RecruitmentReportDto>>>> GetRecruitmentReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string? status)
    {
        var data = await _reportingService.GetRecruitmentReportAsync(startDate, endDate, status);
        return Ok(Result<List<RecruitmentReportDto>>.Success(data));
    }

    [HttpGet("reports/performance-appraisals")]
    public async Task<ActionResult<Result<List<PerformanceReportDto>>>> GetPerformanceReport([FromQuery] int cycleId, [FromQuery] int? departmentId)
    {
        var data = await _reportingService.GetPerformanceReportAsync(cycleId, departmentId);
        return Ok(Result<List<PerformanceReportDto>>.Success(data));
    }
}
