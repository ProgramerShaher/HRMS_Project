using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Features.Leaves.Requests.Commands.ApproveLeaveRequest;
using HRMS.Application.Features.Leaves.Requests.Commands.CreateLeaveRequest;
using HRMS.Application.Features.Leaves.Requests.Commands.RejectLeaveRequest;
using HRMS.Application.Features.Leaves.Requests.Commands.CancelLeaveRequest;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRMS.Application.Features.Leaves.Requests.Queries.GetEmployeeLeaveRequests;
using HRMS.Application.Features.Leaves.Requests.Queries.GetPendingRequests;
using HRMS.Application.Features.Leaves.Dashboard.Queries.GetEmployeeLeaveStats;
using HRMS.Application.Features.Leaves.Dashboard.Queries.GetManagerLeaveStats;

namespace HRMS.API.Controllers.Leaves;


/// <summary>
/// Leave Requests Management Controller.
/// Handles leave request submission, approval, rejection, and queries.
/// </summary>
[ApiController]
[Route("api/Leaves/Requests")]
[Produces("application/json")]
public class LeaveRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ═══════════════════════════════════════════════════════════
    // تقديم طلب إجازة جديد
    // Submit New Leave Request
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Submit a new leave request.
    /// Validates balance availability and checks for overlapping requests.
    /// </summary>
    /// <param name="command">Leave request details</param>
    /// <returns>New request ID</returns>
    [HttpPost]
    [Authorize]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<int>>> CreateLeaveRequest([FromBody] CreateLeaveRequestCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.Succeeded)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }

    // ═══════════════════════════════════════════════════════════
    // اعتماد طلب إجازة
    // Approve Leave Request
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Approve a leave request.
    /// Uses database transaction to atomically update status and deduct balance.
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <param name="command">Approval details</param>
    /// <returns>Success status</returns>
    [HttpPut("{id}/approve")]
    [Authorize(Roles = "System_Admin,HR_Manager,Department_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<bool>>> ApproveRequest(int id, [FromBody] ApproveLeaveRequestCommand command)
    {
        if (id != command.RequestId)
            return BadRequest(Result<bool>.Failure("معرف الطلب غير متطابق", 400));

        var result = await _mediator.Send(command);
        
        if (result.Succeeded)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }

    // ═══════════════════════════════════════════════════════════
    // رفض طلب إجازة
    // Reject Leave Request
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Reject a leave request with mandatory reason.
    /// No balance deduction occurs.
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <param name="command">Rejection details including reason</param>
    /// <returns>Success status</returns>
    [HttpPut("{id}/reject")]
    [Authorize(Roles = "System_Admin,HR_Manager,Department_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<bool>>> RejectRequest(int id, [FromBody] RejectLeaveRequestCommand command)
    {
        if (id != command.RequestId)
            return BadRequest(Result<bool>.Failure("معرف الطلب غير متطابق", 400));

        var result = await _mediator.Send(command);
        
        if (result.Succeeded)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }

    // ═══════════════════════════════════════════════════════════
    // إلغاء طلب إجازة
    // Cancel Leave Request
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Cancel a leave request.
    /// Reverses balance deduction if the request was already posted.
    /// </summary>
    /// <param name="id">Request ID</param>
    /// <param name="command">Cancellation details</param>
    /// <returns>Success status</returns>
    [HttpPut("{id}/cancel")]
    [Authorize]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<bool>>> CancelRequest(int id, [FromBody] CancelLeaveRequestCommand command)
    {
        if (id != command.RequestId)
            return BadRequest(Result<bool>.Failure("معرف الطلب غير متطابق", 400));

        var result = await _mediator.Send(command);
        
        if (result.Succeeded)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }

    // ═══════════════════════════════════════════════════════════
    // جلب طلبات موظف محدد
    // Get Employee Requests
    // ═══════════════════════════════════════════════════════════


    /// <summary>
    /// Get leave request history for a specific employee.
    /// Returns all requests ordered by date descending.
    /// </summary>
    /// <param name="employeeId">Employee ID</param>
    /// <returns>List of leave requests</returns>
    [HttpGet("employee/{employeeId}")]
    [Authorize]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<List<LeaveRequestDto>>>> GetEmployeeRequests(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeLeaveRequestsQuery(employeeId));
        return Ok(result);
    }

    // ═══════════════════════════════════════════════════════════
    // جلب الطلبات المعلقة (للمديرين)
    // Get Pending Requests (For Managers)
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Get all pending leave requests awaiting approval.
    /// For managers and HR to review.
    /// </summary>
    /// <returns>List of pending requests</returns>
    [HttpGet("pending")]
    [Authorize(Roles = "System_Admin,HR_Manager,Department_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<List<LeaveRequestDto>>>> GetPendingRequests()
    {
        var result = await _mediator.Send(new GetPendingRequestsQuery());
        return Ok(result);
    }

    // ═══════════════════════════════════════════════════════════
    // جلب كل الطلبات (للمديرين - جميع الحالات)
    // Get All Requests (For Managers)
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Get all leave requests for admin/HR overview.
    /// </summary>
    /// <returns>List of all requests</returns>
    [HttpGet("all")]
    [Authorize(Roles = "System_Admin,HR_Manager,Department_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<List<LeaveRequestDto>>>> GetAllRequests()
    {
        var result = await _mediator.Send(new HRMS.Application.Features.Leaves.Requests.Queries.GetAllRequests.GetAllRequestsQuery());
        return Ok(result);
    }

    // ═══════════════════════════════════════════════════════════
    // إحصائيات لوحة التحكم
    // Dashboard Statistics
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Get leave dashboard stats for a specific employee.
    /// </summary>
    [HttpGet("stats/employee/{id}")]
    [Authorize]
    [AllowAnonymous] // 🔓 للتطوير فقط
    public async Task<ActionResult<Result<LeaveDashboardStatsDto>>> GetEmployeeStats(int id)
    {
        var result = await _mediator.Send(new GetEmployeeLeaveStatsQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Get leave dashboard stats for a manager (Global/Team view).
    /// </summary>
    [HttpGet("stats/manager")]
    [Authorize(Roles = "System_Admin,HR_Manager,Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط
    public async Task<ActionResult<Result<LeaveDashboardStatsDto>>> GetManagerStats()
    {
        var result = await _mediator.Send(new GetManagerLeaveStatsQuery());
        return Ok(result);
    }
}
