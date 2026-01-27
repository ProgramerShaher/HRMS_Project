using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using HRMS.Application.Features.Core.JobGrades.Commands.CreateJobGrade;
using HRMS.Application.Features.Core.JobGrades.Commands.UpdateJobGrade;
using HRMS.Application.Features.Core.JobGrades.Commands.DeleteJobGrade;
using HRMS.Application.Features.Core.JobGrades.Queries.GetAllJobGrades;
using HRMS.Application.Features.Core.JobGrades.Queries.GetJobGradeById;
using HRMS.Application.DTOs.Core;
using HRMS.Core.Utilities;

namespace HRMS.API.Controllers.Core;

/// <summary>
/// تحكم الدرجات الوظيفية - إدارة الدرجات الوظيفية ونطاقات الرواتب
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[AllowAnonymous] // 🔓 للتطوير فقط
public class JobGradesController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobGradesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// الحصول على قائمة الدرجات الوظيفية
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<JobGradeListDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllJobGradesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(Result<PagedResult<JobGradeListDto>>.Success(result, "تم جلب القائمة بنجاح"));
    }

    /// <summary>
    /// الحصول على درجة وظيفية بمعرفها
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<JobGradeDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetJobGradeByIdQuery(id));
        
        if (result == null)
            return NotFound(Result<JobGradeDto>.Failure("الدرجة الوظيفية غير موجودة", 404));

        return Ok(Result<JobGradeDto>.Success(result, "تم جلب البيانات بنجاح"));
    }

    /// <summary>
    /// إنشاء درجة وظيفية جديدة
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<int>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateJobGradeCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, 
            Result<int>.Success(result, "تم إنشاء الدرجة الوظيفية بنجاح"));
    }

    /// <summary>
    /// تحديث الدرجة الوظيفية
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<int>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateJobGradeCommand command)
    {
        command.JobGradeId = id;

        try
        {
            var result = await _mediator.Send(command);
            return Ok(Result<int>.Success(result, "تم تحديث الدرجة الوظيفية بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<int>.Failure(ex.Message, 404));
        }
    }

    /// <summary>
    /// حذف الدرجة الوظيفية
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "System_Admin")]
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteJobGradeCommand(id));
            return Ok(Result<bool>.Success(result, "تم حذف الدرجة الوظيفية بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<bool>.Failure(ex.Message, 404));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(Result<bool>.Failure(ex.Message, 400));
        }
    }
}
