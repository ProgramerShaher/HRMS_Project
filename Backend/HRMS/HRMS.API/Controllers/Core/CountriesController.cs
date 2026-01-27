using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using HRMS.Application.Features.Core.Countries.Commands.CreateCountry;
using HRMS.Application.Features.Core.Countries.Commands.UpdateCountry;
using HRMS.Application.Features.Core.Countries.Commands.DeleteCountry;
using HRMS.Application.Features.Core.Countries.Queries.GetCountryById;
using HRMS.Application.Features.Core.Countries.Queries.GetAllCountries;
using HRMS.Application.Features.Core.Countries.Queries.SearchCountries;
using HRMS.Application.DTOs.Core;
using HRMS.Core.Utilities;

namespace HRMS.API.Controllers.Core;

/// <summary>
/// تحكم الدول - إدارة بيانات الدول والجنسيات
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
public class CountriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CountriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// الحصول على قائمة الدول مع الترقيم
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<CountryListDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllCountriesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(Result<PagedResult<CountryListDto>>.Success(result, "تم جلب قائمة الدول بنجاح"));
    }

    /// <summary>
    /// البحث المتقدم في الدول
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(Result<PagedResult<CountryListDto>>), 200)]
    public async Task<IActionResult> Search([FromQuery] SearchCountriesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(Result<PagedResult<CountryListDto>>.Success(result, "تم البحث بنجاح"));
    }

    /// <summary>
    /// الحصول على دولة بمعرفها
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<CountryDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetCountryByIdQuery(id));
        
        if (result == null)
            return NotFound(Result<CountryDto>.Failure("الدولة غير موجودة", 404));

        return Ok(Result<CountryDto>.Success(result, "تم جلب بيانات الدولة بنجاح"));
    }

    /// <summary>
    /// إضافة دولة جديدة
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [ProducesResponseType(typeof(Result<int>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateCountryCommand command)
    {
        var countryId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = countryId }, 
            Result<int>.Success(countryId, "تم إضافة الدولة بنجاح"));
    }

    /// <summary>
    /// تحديث بيانات دولة
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [ProducesResponseType(typeof(Result<int>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCountryDto dto)
    {
        var command = new UpdateCountryCommand
        {
            CountryId = id,
            CountryNameAr = dto.CountryNameAr,
            CountryNameEn = dto.CountryNameEn,
            CitizenshipNameAr = dto.CitizenshipNameAr,
            IsoCode = dto.IsoCode,
            IsActive = dto.IsActive
        };

        try
        {
            var result = await _mediator.Send(command);
            return Ok(Result<int>.Success(result, "تم تحديث بيانات الدولة بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<int>.Failure(ex.Message, 404));
        }
    }

    /// <summary>
    /// حذف دولة
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
            var result = await _mediator.Send(new DeleteCountryCommand(id));
            return Ok(Result<bool>.Success(result, "تم حذف الدولة بنجاح"));
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
