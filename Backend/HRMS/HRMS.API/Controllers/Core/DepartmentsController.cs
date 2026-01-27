using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using HRMS.Application.Features.Core.Departments.Commands.CreateDepartment;
using HRMS.Application.Features.Core.Departments.Commands.UpdateDepartment;
using HRMS.Application.Features.Core.Departments.Commands.DeleteDepartment;
using HRMS.Application.Features.Core.Departments.Queries.GetAllDepartments;
using HRMS.Application.Features.Core.Departments.Queries.GetDepartmentById;
using HRMS.Application.DTOs.Core;
using HRMS.Core.Utilities;

namespace HRMS.API.Controllers.Core;

/// <summary>
/// تحكم الأقسام - إدارة الهيكل التنظيمي للأقسام
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[AllowAnonymous]
public class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepartmentsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// الحصول على قائمة الأقسام
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<DepartmentDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllDepartmentsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(Result<PagedResult<DepartmentDto>>.Success(result, "تم جلب القائمة بنجاح"));
    }

    /// <summary>
    /// الحصول على قسم بمعرفه
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<DepartmentDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetDepartmentByIdQuery(id));
        
        if (result == null)
            return NotFound(Result<DepartmentDto>.Failure("القسم غير موجود", 404));

        return Ok(Result<DepartmentDto>.Success(result, "تم جلب البيانات بنجاح"));
    }

    /// <summary>
    /// إنشاء قسم جديد
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<int>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, 
            Result<int>.Success(result, "تم إنشاء القسم بنجاح"));
    }

    /// <summary>
    /// تحديث القسم
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<int>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentCommand command)
    {
        command.DeptId = id;

        try
        {
            var result = await _mediator.Send(command);
            return Ok(Result<int>.Success(result, "تم تحديث القسم بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<int>.Failure(ex.Message, 404));
        }
    }

    /// <summary>
    /// حذف القسم
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
            var result = await _mediator.Send(new DeleteDepartmentCommand(id));
            return Ok(Result<bool>.Success(result, "تم حذف القسم بنجاح"));
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
