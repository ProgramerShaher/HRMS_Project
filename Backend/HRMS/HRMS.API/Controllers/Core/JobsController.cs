using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using HRMS.Application.Features.Core.Jobs.Commands.CreateJob;
using HRMS.Application.Features.Core.Jobs.Commands.UpdateJob;
using HRMS.Application.Features.Core.Jobs.Commands.DeleteJob;
using HRMS.Application.Features.Core.Jobs.Queries.GetAllJobs;
using HRMS.Application.Features.Core.Jobs.Queries.GetJobById;
using HRMS.Application.DTOs.Core;
using HRMS.Core.Utilities;

namespace HRMS.API.Controllers.Core;

/// <summary>
/// تحكم الوظائف - إدارة المسميات الوظيفية
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[AllowAnonymous]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// الحصول على قائمة الوظائف
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<JobDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllJobsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(Result<PagedResult<JobDto>>.Success(result, "تم جلب القائمة بنجاح"));
    }

    /// <summary>
    /// الحصول على وظيفة بمعرفها
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<JobDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetJobByIdQuery(id));
        
        if (result == null)
            return NotFound(Result<JobDto>.Failure("الوظيفة غير موجودة", 404));

        return Ok(Result<JobDto>.Success(result, "تم جلب البيانات بنجاح"));
    }

    /// <summary>
    /// إنشاء وظيفة جديدة
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<int>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateJobCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, 
            Result<int>.Success(result, "تم إنشاء الوظيفة بنجاح"));
    }

    /// <summary>
    /// تحديث الوظيفة
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<int>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateJobCommand command)
    {
        command.JobId = id;

        try
        {
            var result = await _mediator.Send(command);
            return Ok(Result<int>.Success(result, "تم تحديث الوظيفة بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<int>.Failure(ex.Message, 404));
        }
    }

    /// <summary>
    /// حذف الوظيفة
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "System_Admin")]
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteJobCommand(id));
            return Ok(Result<bool>.Success(result, "تم حذف الوظيفة بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<bool>.Failure(ex.Message, 404));
        }
    }
}
