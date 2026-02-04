using HRMS.Application.Features.Attendance.Punch.RegisterPunch;
using HRMS.Application.Features.Attendance.Requests.UpdateShiftSwap;
using HRMS.Application.Features.Attendance.Requests.CancelShiftSwap;
using HRMS.Application.Features.Attendance.Requests.RevokeShiftSwap;
using HRMS.Application.Features.Attendance.Requests.UpdateOvertime;
using HRMS.Application.Features.Attendance.Requests.CancelOvertime;
using HRMS.Application.Features.Attendance.Commands.ProcessMonthlyClosing;
using HRMS.Application.DTOs.Attendance;
using HRMS.Application.Features.Attendance.Queries.GetAttendanceStats;
using HRMS.Application.Features.Attendance.Queries.GetDailyTimesheet;
using HRMS.Application.Features.Attendance.Dashboard.Queries.GetLiveAttendanceStatus;
using HRMS.Application.Features.Attendance.Dashboard.Queries.GetAttendanceExceptions;
using HRMS.Application.Features.Attendance.Commands.ManualCorrection;
using HRMS.Application.Features.Attendance.Reports.Queries.GetMonthlyPayrollSummary;
using HRMS.Application.Features.Attendance.Requests.Permissions.Commands.CreatePermissionRequest;
using HRMS.Application.Features.Attendance.Requests.Permissions.Commands.ApproveRejectPermissionRequest;
using HRMS.Application.Features.Attendance.Roster.Queries.GetMyRoster;
using HRMS.Core.Utilities; // For Result<T>
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Attendance;

[Route("api/[controller]")]
[ApiController]
public class AttendanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttendanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ═══════════════════════════════════════════════════════════
    // PUNCH
    // ═══════════════════════════════════════════════════════════
    [HttpPost("punch")]
    public async Task<ActionResult<long>> RegisterPunch([FromBody] RegisterPunchCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    // ═══════════════════════════════════════════════════════════
    // SWAP LIFECYCLE
    // ═══════════════════════════════════════════════════════════
    [HttpPut("swap-requests")]
    public async Task<ActionResult<bool>> UpdateSwapRequest([FromBody] UpdateShiftSwapCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpDelete("swap-requests/{id}")]
    public async Task<ActionResult<bool>> CancelSwapRequest(int id)
    {
        var result = await _mediator.Send(new CancelShiftSwapCommand(id));
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost("swap-requests/revoke")]
    public async Task<ActionResult<bool>> RevokeApprovedSwap([FromBody] RevokeShiftSwapCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    // ═══════════════════════════════════════════════════════════
    // OVERTIME LIFECYCLE
    // ═══════════════════════════════════════════════════════════
    [HttpPut("overtime-requests")]
    public async Task<ActionResult<bool>> UpdateOvertimeRequest([FromBody] UpdateOvertimeCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpDelete("overtime-requests/{id}")]
    public async Task<ActionResult<bool>> CancelOvertimeRequest(int id)
    {
        var result = await _mediator.Send(new CancelOvertimeCommand(id));
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    // ═══════════════════════════════════════════════════════════
    // ANALYTICAL & DASHBOARD
    // ═══════════════════════════════════════════════════════════

    [HttpGet("stats")]
    public async Task<ActionResult<Result<AttendanceStatsDto>>> GetStats([FromQuery] int employeeId, [FromQuery] int month, [FromQuery] int year)
    {
        var result = await _mediator.Send(new GetAttendanceStatsQuery(employeeId, month, year));
        return Ok(result);
    }

    [HttpGet("timesheet")]
    public async Task<ActionResult<Result<List<TimesheetDayDto>>>> GetTimesheet([FromQuery] int employeeId, [FromQuery] int month, [FromQuery] int year)
    {
        var result = await _mediator.Send(new GetDailyTimesheetQuery(employeeId, month, year));
        return Ok(result);
    }

    [HttpGet("dashboard/live")]
    public async Task<ActionResult<Result<LiveStatusDto>>> GetLiveStatus()
    {
        var result = await _mediator.Send(new GetLiveAttendanceStatusQuery());
        return Ok(result);
    }

    [HttpGet("dashboard/exceptions")]
    public async Task<ActionResult<Result<List<AttendanceExceptionDto>>>> GetExceptions()
    {
        var result = await _mediator.Send(new GetAttendanceExceptionsQuery());
        return Ok(result);
    }

    // ═══════════════════════════════════════════════════════════
    // ADMINISTRATION
    // ═══════════════════════════════════════════════════════════

    [HttpPost("correction")]
    public async Task<ActionResult<Result<bool>>> ManualCorrection([FromBody] ManualCorrectionCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // REPORTS
    // ═══════════════════════════════════════════════════════════

    [HttpGet("reports/payroll-summary")]
    public async Task<ActionResult<Result<List<PayrollAttendanceSummaryDto>>>> GetPayrollSummary([FromQuery] int month, [FromQuery] int year)
    {
        var result = await _mediator.Send(new GetMonthlyPayrollSummaryQuery(month, year));
        return Ok(result);
    }

    // ═══════════════════════════════════════════════════════════
    // MONTHLY CLOSING (PAYROLL INTEGRATION)
    // ═══════════════════════════════════════════════════════════
    
    /// <summary>
    /// Processes monthly attendance closing with dynamic policy-driven calculations.
    /// Locks the period to prevent further modifications and prepares data for payroll.
    /// </summary>
    /// <param name="command">Command containing year, month, and user ID</param>
    /// <returns>Summary of closing operation including totals and locked period ID</returns>
    [HttpPost("monthly-closing")]
    public async Task<ActionResult<MonthlyClosingResultDto>> ProcessMonthlyClosing(
        [FromBody] ProcessMonthlyAttendanceClosingCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    // ═══════════════════════════════════════════════════════════
    // PERMISSIONS
    // ═══════════════════════════════════════════════════════════
    [HttpPost("permissions")]
    public async Task<ActionResult<Result<int>>> ApplyPermission([FromBody] CreatePermissionRequestCommand command)
    {
        // Set EmployeeId from User Context if not provided or force it for security
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; 
        if (int.TryParse(userId, out int id)) command.EmployeeId = id; // Assuming UserId maps to EmployeeId or fetch via Claims

        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("permissions/action")]
    public async Task<ActionResult<Result<bool>>> ActionPermission([FromBody] ApproveRejectPermissionRequestCommand command)
    {
         var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
         if (int.TryParse(userId, out int approverId)) command.ApproverId = approverId;

        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // ROSTER
    // ═══════════════════════════════════════════════════════════
    [HttpGet("my-roster")]
    public async Task<ActionResult<Result<List<MyRosterDto>>>> GetMyRoster()
    {
         var userId = User.FindFirst("EmployeeId")?.Value; // Assuming EmployeeId claim exists
         if (userId == null) return Unauthorized(Result<List<MyRosterDto>>.Failure("EmployeeId claim not found"));

        var result = await _mediator.Send(new GetMyRosterQuery { EmployeeId = int.Parse(userId) });
        return Ok(result);
    }
}
