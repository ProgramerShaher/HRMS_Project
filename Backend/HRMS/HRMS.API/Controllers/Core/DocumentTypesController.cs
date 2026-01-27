using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using HRMS.Application.Features.Core.DocumentTypes.Commands.CreateDocumentType;
using HRMS.Application.Features.Core.DocumentTypes.Commands.UpdateDocumentType;
using HRMS.Application.Features.Core.DocumentTypes.Commands.DeleteDocumentType;
using HRMS.Application.Features.Core.DocumentTypes.Queries.GetAllDocumentTypes;
using HRMS.Application.Features.Core.DocumentTypes.Queries.GetDocumentTypeById;
using HRMS.Application.DTOs.Core;
using HRMS.Core.Utilities;

namespace HRMS.API.Controllers.Core;

/// <summary>
/// تحكم أنواع الوثائق - إدارة أنواع الوثائق المطلوبة في النظام
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[AllowAnonymous] // 🔓 للتطوير فقط
public class DocumentTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// الحصول على قائمة أنواع الوثائق مع الترقيم
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<DocumentTypeListDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllDocumentTypesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(Result<PagedResult<DocumentTypeListDto>>.Success(result, "تم جلب القائمة بنجاح"));
    }

    /// <summary>
    /// الحصول على نوع وثيقة بمعرفه
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<DocumentTypeDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetDocumentTypeByIdQuery(id));
        
        if (result == null)
            return NotFound(Result<DocumentTypeDto>.Failure("نوع الوثيقة غير موجود", 404));

        return Ok(Result<DocumentTypeDto>.Success(result, "تم جلب البيانات بنجاح"));
    }

    /// <summary>
    /// إنشاء نوع وثيقة جديد
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<int>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateDocumentTypeCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetById), 
            new { id = result }, 
            Result<int>.Success(result, "تم إنشاء نوع الوثيقة بنجاح"));
    }

    /// <summary>
    /// تحديث نوع الوثيقة
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<int>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentTypeCommand command)
    {
        command.DocumentTypeId = id;

        try
        {
            var result = await _mediator.Send(command);
            return Ok(Result<int>.Success(result, "تم تحديث نوع الوثيقة بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<int>.Failure(ex.Message, 404));
        }
    }

    /// <summary>
    /// حذف نوع الوثيقة
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "System_Admin")]
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteDocumentTypeCommand(id));
            return Ok(Result<bool>.Success(result, "تم حذف نوع الوثيقة بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<bool>.Failure(ex.Message, 404));
        }
    }
}
