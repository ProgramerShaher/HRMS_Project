using HRMS.Application.DTOs.Reports.Analytics;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Reports;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Secure by default
public class ReportsController : ControllerBase
{
    private readonly IReportingService _reportingService;

    public ReportsController(IReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    // ═══════════════════════════════════════════════════════════
    // 1. HR Analytics Endpoints
    // ═══════════════════════════════════════════════════════════
    
    /// <summary>
    /// الحصول على ملخص الموارد البشرية (عدد الموظفين، التوزيع حسب الأقسام، انتهاء الوثائق)
    /// </summary>
    [HttpGet("analytics/hr-overview")]
    public async Task<ActionResult<Result<HROverviewDto>>> GetHROverview()
    {
        var data = await _reportingService.GetHROverviewAsync();
        return Ok(Result<HROverviewDto>.Success(data));
    }

    // ═══════════════════════════════════════════════════════════
    // 2. Attendance Analytics Endpoints
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// إحصائيات الحضور والغياب لفترة محددة
    /// </summary>
    [HttpGet("analytics/attendance")]
    public async Task<ActionResult<Result<AttendanceStatsDto>>> GetAttendanceStats([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var data = await _reportingService.GetAttendanceStatsAsync(startDate, endDate);
        return Ok(Result<AttendanceStatsDto>.Success(data));
    }

    // ═══════════════════════════════════════════════════════════
    // 3. Payroll Analytics Endpoints
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// إحصائيات الرواتب لشهر وسنة محددة
    /// </summary>
    [HttpGet("analytics/payroll")]
    public async Task<ActionResult<Result<PayrollStatsDto>>> GetPayrollStats([FromQuery] int month, [FromQuery] int year)
    {
        var data = await _reportingService.GetPayrollStatsAsync(month, year);
        return Ok(Result<PayrollStatsDto>.Success(data));
    }

    // ═══════════════════════════════════════════════════════════
    // 4. Operational Reports Endpoints
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// تقرير سجل الموظفين الشامل (يمكن تصفيته حسب القسم أو الحالة)
    /// </summary>
    [HttpGet("reports/employee-census")]
    public async Task<ActionResult<Result<List<EmployeeCensusDto>>>> GetEmployeeCensusReport([FromQuery] int? departmentId, [FromQuery] string? status)
    {
        var data = await _reportingService.GetEmployeeCensusReportAsync(departmentId, status);
        return Ok(Result<List<EmployeeCensusDto>>.Success(data));
    }

    /// <summary>
    /// تقرير الحضور اليومي التفصيلي (سجل الحركات)
    /// </summary>
    [HttpGet("reports/attendance-detailed")]
    public async Task<ActionResult<Result<List<DailyAttendanceDetailsDto>>>> GetDetailedAttendanceReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? departmentId)
    {
        var data = await _reportingService.GetDetailedAttendanceReportAsync(startDate, endDate, departmentId);
        return Ok(Result<List<DailyAttendanceDetailsDto>>.Success(data));
    }

    /// <summary>
    /// تقرير مسير الرواتب الشهري التفصيلي
    /// </summary>
    [HttpGet("reports/payslip-monthly")]
    public async Task<ActionResult<Result<List<MonthlyPayslipReportDto>>>> GetMonthlyPayslipReport([FromQuery] int month, [FromQuery] int year, [FromQuery] int? departmentId)
    {
        var data = await _reportingService.GetMonthlyPayslipReportAsync(month, year, departmentId);
        return Ok(Result<List<MonthlyPayslipReportDto>>.Success(data));
    }

    /// <summary>
    /// تقرير سجل الإجازات وطلبات المغادرة
    /// </summary>
    [HttpGet("reports/leave-history")]
    public async Task<ActionResult<Result<List<LeaveHistoryDto>>>> GetLeaveHistoryReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? departmentId)
    {
        var data = await _reportingService.GetLeaveHistoryReportAsync(startDate, endDate, departmentId);
        return Ok(Result<List<LeaveHistoryDto>>.Success(data));
    }

    /// <summary>
    /// تقرير التوظيف والمرشحين
    /// </summary>
    [HttpGet("reports/recruitment-pipeline")]
    public async Task<ActionResult<Result<List<RecruitmentReportDto>>>> GetRecruitmentReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string? status)
    {
        var data = await _reportingService.GetRecruitmentReportAsync(startDate, endDate, status);
        return Ok(Result<List<RecruitmentReportDto>>.Success(data));
    }

    /// <summary>
    /// تقرير تقييم الأداء
    /// </summary>
    [HttpGet("reports/performance-appraisals")]
    public async Task<ActionResult<Result<List<PerformanceReportDto>>>> GetPerformanceReport([FromQuery] int cycleId, [FromQuery] int? departmentId)
    {
        var data = await _reportingService.GetPerformanceReportAsync(cycleId, departmentId);
        return Ok(Result<List<PerformanceReportDto>>.Success(data));
    }
}
