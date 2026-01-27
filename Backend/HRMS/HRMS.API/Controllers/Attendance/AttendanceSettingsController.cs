using HRMS.Application.Features.Attendance.Configuration;
using HRMS.Application.Features.Attendance.Roster.Commands.InitializeRoster;
using HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;
using HRMS.Application.Features.Attendance.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Attendance;

[Route("api/[controller]")]
[ApiController]
public class AttendanceSettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttendanceSettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ═══════════════════════════════════════════════════════════
    // Shift Types Endpoints
    // ═══════════════════════════════════════════════════════════
    
    [HttpGet("shifts")]
    public async Task<ActionResult<List<ShiftTypeDto>>> GetAllShifts()
    {
        var result = await _mediator.Send(new GetShiftTypesQuery());
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost("shifts")]
    public async Task<ActionResult<int>> CreateShift([FromBody] CreateShiftTypeCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPut("shifts/{id}")]
    public async Task<ActionResult<bool>> UpdateShift(int id, [FromBody] UpdateShiftTypeCommand command)
    {
        if (id != command.ShiftId) return BadRequest("ID Mismatch");
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpDelete("shifts/{id}")]
    public async Task<ActionResult<bool>> DeleteShift(int id)
    {
        var result = await _mediator.Send(new DeleteShiftTypeCommand(id));
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    // ═══════════════════════════════════════════════════════════
    // Roster & Process Endpoints
    // ═══════════════════════════════════════════════════════════
    
    [HttpPost("initialize-roster")]
    public async Task<ActionResult<bool>> InitializeRoster([FromBody] InitializeRosterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("process-attendance")]
    public async Task<ActionResult<int>> ProcessAttendance([FromBody] ProcessAttendanceCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // ═══════════════════════════════════════════════════════════
    // Shift Swap Endpoints
    // ═══════════════════════════════════════════════════════════

    [HttpPost("apply-swap")]
    public async Task<ActionResult<int>> ApplySwap([FromBody] CreateSwapRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost("action-swap-request")]
    public async Task<ActionResult<bool>> ActionSwapRequest([FromBody] ActionSwapRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    // ═══════════════════════════════════════════════════════════
    // Overtime Endpoints
    // ═══════════════════════════════════════════════════════════

    [HttpPost("apply-overtime")]
    public async Task<ActionResult<int>> ApplyOvertime([FromBody] ApplyOvertimeCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }

    [HttpPost("action-overtime")]
    public async Task<ActionResult<bool>> ActionOvertime([FromBody] ActionOvertimeCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
    }
}
