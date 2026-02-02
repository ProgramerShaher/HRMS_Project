using HRMS.Application.DTOs.Attendance;
using HRMS.Application.Features.Attendance.Configuration.CreateAttendancePolicy;
using HRMS.Application.Features.Attendance.Configuration.UpdateAttendancePolicy;
using HRMS.Application.Features.Attendance.Configuration.DeleteAttendancePolicy;
using HRMS.Application.Features.Attendance.Configuration.GetAttendancePolicies;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Attendance;

/// <summary>
/// Controller for managing attendance policies (CRUD operations).
/// Policies define dynamic rules for late grace periods and overtime calculations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AttendancePolicyController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttendancePolicyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all attendance policies.
    /// </summary>
    /// <returns>List of attendance policies</returns>
    [HttpGet]
    public async Task<ActionResult<Result<List<AttendancePolicyDto>>>> GetAll()
    {
        var result = await _mediator.Send(new GetAttendancePoliciesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Creates a new attendance policy.
    /// </summary>
    /// <param name="command">Policy creation data</param>
    /// <returns>Created policy ID</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/AttendancePolicy
    ///     {
    ///         "policyNameAr": "سياسة الطوارئ",
    ///         "deptId": 5,
    ///         "jobId": null,
    ///         "lateGraceMins": 10,
    ///         "overtimeMultiplier": 2.0,
    ///         "weekendOtMultiplier": 2.5
    ///     }
    ///     
    /// To create a default policy (fallback for all departments), set both deptId and jobId to null.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateAttendancePolicyCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing attendance policy.
    /// </summary>
    /// <param name="command">Policy update data</param>
    /// <returns>Success status</returns>
    [HttpPut]
    public async Task<ActionResult<Result<bool>>> Update([FromBody] UpdateAttendancePolicyCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deletes an attendance policy (soft delete).
    /// </summary>
    /// <param name="policyId">Policy identifier</param>
    /// <returns>Success status</returns>
    [HttpDelete("{policyId}")]
    public async Task<ActionResult<Result<bool>>> Delete(int policyId)
    {
        var result = await _mediator.Send(new DeleteAttendancePolicyCommand(policyId));
        return Ok(result);
    }
}
