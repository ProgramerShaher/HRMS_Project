using HRMS.Application.Features.Leaves.Dashboard.Queries.GetApprovalStats;
using HRMS.Application.Features.Leaves.Dashboard.Queries.GetDelayedApprovals;
using HRMS.Application.Features.Leaves.Reports.Queries.GetLeaveTransactionReport;
using HRMS.Application.Features.Leaves.Reports.Queries.GetPayrollIntegrationReport;
using HRMS.Application.Features.Leaves.Requests.Commands.BulkApproveLeaves;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Leaves;

/// <summary>
/// وحدة الموافقات والتقارير المتقدمة للإجازات
/// Approvals & Advanced Reports Controller
/// </summary>
[ApiController]
[Route("api/Leaves/Approvals")]
[Produces("application/json")]
public class ApprovalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApprovalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ═══════════════════════════════════════════════════════════
    // الاعتماد الجماعي
    // Bulk Approval
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Bulk approve multiple leave requests.
    /// Transactional operation with detailed result summary.
    /// </summary>
    [HttpPost("bulk")]
    [Authorize(Roles = "System_Admin,HR_Manager,Department_Manager")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<BulkApproveResultDto>>> BulkApprove([FromBody] BulkApproveLeavesCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // ═══════════════════════════════════════════════════════════
    // لوحة المعلومات والإحصائيات
    // Dashboard & Stats
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Get approval statistics for dashboard.
    /// </summary>
    [HttpGet("stats")]
    [Authorize]
    [AllowAnonymous]
    public async Task<ActionResult<Result<ApprovalStatsDto>>> GetStats()
    {
        var result = await _mediator.Send(new GetApprovalStatsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Get requests pending for more than 48 hours.
    /// </summary>
    [HttpGet("delayed")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<HRMS.Application.DTOs.Leaves.LeaveRequestDto>>>> GetDelayed()
    {
        var result = await _mediator.Send(new GetDelayedApprovalsQuery());
        return Ok(result);
    }

    // ═══════════════════════════════════════════════════════════
    // التقارير
    // Reports
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// detailed leave transaction report with filters.
    /// </summary>
    [HttpGet("reports/transactions")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<LeaveTransactionReportDto>>>> GetTransactionReport(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? employeeId,
        [FromQuery] string? status)
    {
        var result = await _mediator.Send(new GetLeaveTransactionReportQuery(fromDate, toDate, employeeId, status));
        return Ok(result);
    }

    /// <summary>
    /// Payroll integration report for specific month.
    /// Includes deductibility flags.
    /// </summary>
    [HttpGet("reports/payroll")]
    [Authorize(Roles = "System_Admin,HR_Manager,Payroll_Manager")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<List<PayrollLeaveDto>>>> GetPayrollReport(
        [FromQuery] int month,
        [FromQuery] int year)
    {
        var result = await _mediator.Send(new GetPayrollIntegrationReportQuery(month, year));
        return Ok(result);
    }
}
