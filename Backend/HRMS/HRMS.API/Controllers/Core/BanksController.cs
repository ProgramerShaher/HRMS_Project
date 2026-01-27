using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using HRMS.Application.Features.Core.Banks.Commands.CreateBank;
using HRMS.Application.Features.Core.Banks.Commands.UpdateBank;
using HRMS.Application.Features.Core.Banks.Commands.DeleteBank;
using HRMS.Application.Features.Core.Banks.Queries.GetBankById;
using HRMS.Application.Features.Core.Banks.Queries.GetAllBanks;
using HRMS.Application.DTOs.Core;
using HRMS.Core.Utilities;

namespace HRMS.API.Controllers.Core;

/// <summary>
/// تحكم البنوك - إدارة بيانات البنوك المعتمدة في النظام
/// </summary>
/// <remarks>
/// يوفر هذا المتحكم عمليات CRUD كاملة لإدارة البنوك المستخدمة في تحويل الرواتب
/// </remarks>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
public class BanksController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// المنشئ
    /// </summary>
    /// <param name="mediator">وسيط MediatR</param>
    public BanksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// الحصول على قائمة البنوك مع الترقيم
    /// </summary>
    /// <param name="query">معايير البحث والترقيم</param>
    /// <returns>قائمة مرقمة من البنوك</returns>
    /// <response code="200">تم جلب القائمة بنجاح</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<BankListDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllBanksQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// الحصول على بنك بمعرفه
    /// </summary>
    /// <param name="id">معرف البنك</param>
    /// <returns>بيانات البنك</returns>
    /// <response code="200">تم جلب البيانات بنجاح</response>
    /// <response code="404">البنك غير موجود</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<BankDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetBankByIdQuery(id));
        
        if (result == null)
            return NotFound(Result<BankDto>.Failure("البنك غير موجود", 404));

        return Ok(Result<BankDto>.Success(result, "تم جلب بيانات البنك بنجاح"));
    }

    /// <summary>
    /// إنشاء بنك جديد
    /// </summary>
    /// <param name="command">بيانات البنك الجديد</param>
    /// <returns>معرف البنك الجديد</returns>
    /// <response code="201">تم إنشاء البنك بنجاح</response>
    /// <response code="400">بيانات غير صحيحة</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<int>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateBankCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetById), 
            new { id = result }, 
            Result<int>.Success(result, "تم إنشاء البنك بنجاح"));
    }

    /// <summary>
    /// تحديث بيانات بنك
    /// </summary>
    /// <param name="id">معرف البنك</param>
    /// <param name="dto">بيانات التحديث</param>
    /// <returns>معرف البنك المحدث</returns>
    /// <response code="200">تم التحديث بنجاح</response>
    /// <response code="404">البنك غير موجود</response>
    /// <response code="400">بيانات غير صحيحة</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [ProducesResponseType(typeof(Result<int>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBankDto dto)
    {
        var command = new UpdateBankCommand
        {
            BankId = id,
            BankNameAr = dto.BankNameAr,
            BankNameEn = dto.BankNameEn,
            SwiftCode = dto.SwiftCode,
            BankCode = dto.BankCode,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email
        };

        try
        {
            var result = await _mediator.Send(command);
            return Ok(Result<int>.Success(result, "تم تحديث بيانات البنك بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<int>.Failure(ex.Message, 404));
        }
    }

    /// <summary>
    /// حذف بنك
    /// </summary>
    /// <param name="id">معرف البنك</param>
    /// <returns>نتيجة الحذف</returns>
    /// <response code="200">تم الحذف بنجاح</response>
    /// <response code="404">البنك غير موجود</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "System_Admin")]
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteBankCommand(id));
            return Ok(Result<bool>.Success(result, "تم حذف البنك بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<bool>.Failure(ex.Message, 404));
        }
    }
}
