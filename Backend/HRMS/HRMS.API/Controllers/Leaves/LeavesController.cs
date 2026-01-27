using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Features.Leaves.Requests.Commands.CreateLeaveRequest;
using HRMS.Application.Features.Leaves.Requests.Commands.ProcessLeaveAction;
using HRMS.Application.Features.Leaves.Requests.Queries.GetMyRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers.Leaves;

[Route("api/[controller]")]
[ApiController]
public class LeavesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeavesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// تقديم طلب إجازة جديد
    /// </summary>
    [HttpPost("apply")]
    public async Task<ActionResult<int>> ApplyForLeave([FromBody] CreateLeaveRequestCommand command)
    {
        // Optional: Auto-fill EmployeeId if using JWT
        // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // if (userId != null) command.EmployeeId = int.Parse(userId);

        var requestId = await _mediator.Send(command);
        return Ok(requestId);
    }

    /// <summary>
    /// الحصول على طلبات الإجازة الخاصة بالموظف الحالي
    /// </summary>
    [HttpGet("my-requests/{employeeId}")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetMyRequests(int employeeId)
    {
        // In real auth scenario, we'd get ID from token
        var query = new GetMyLeaveRequestsQuery { EmployeeId = employeeId };
        var requests = await _mediator.Send(query);
        return Ok(requests);
    }

    /// <summary>
    /// اتخاذ إجراء على طلب إجازة (موافقة/رفض) - للمدراء
    /// </summary>
    [HttpPost("action")]
    // [Authorize(Roles = "Manager,HR")] // Uncomment when Auth is ready
    public async Task<ActionResult<bool>> ProcessAction([FromBody] LeaveActionDto actionDto)
    {
        var command = new ProcessLeaveActionCommand(actionDto);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
