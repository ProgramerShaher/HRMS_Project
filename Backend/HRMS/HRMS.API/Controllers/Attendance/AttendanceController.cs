using HRMS.Application.Features.Attendance.Punch;
using HRMS.Application.Features.Attendance.Requests;
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
    public async Task<ActionResult<bool>> UpdateSwapRequest([FromBody] UpdateSwapRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpDelete("swap-requests/{id}")]
    public async Task<ActionResult<bool>> CancelSwapRequest(int id)
    {
        var result = await _mediator.Send(new CancelSwapRequestCommand(id));
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost("swap-requests/revoke")]
    public async Task<ActionResult<bool>> RevokeApprovedSwap([FromBody] RevokeSwapRequestCommand command)
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
}
