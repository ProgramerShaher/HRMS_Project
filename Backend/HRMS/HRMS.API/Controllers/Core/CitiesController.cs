using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using HRMS.Application.Features.Core.Cities.Commands.CreateCity;
using HRMS.Application.Features.Core.Cities.Commands.UpdateCity;
using HRMS.Application.Features.Core.Cities.Commands.DeleteCity;
using HRMS.Application.Features.Core.Cities.Queries.GetCityById;
using HRMS.Application.Features.Core.Cities.Queries.GetAllCities;
using HRMS.Application.DTOs.Core;
using HRMS.Core.Utilities;

namespace HRMS.API.Controllers.Core;

/// <summary>
/// تحكم المدن - إدارة بيانات المدن
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
public class CitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// الحصول على قائمة المدن مع الترقيم
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<CityListDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllCitiesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(Result<PagedResult<CityListDto>>.Success(result, "تم جلب قائمة المدن بنجاح"));
    }

    /// <summary>
    /// الحصول على مدن دولة معينة
    /// </summary>
    [HttpGet("by-country/{countryId}")]
    [ProducesResponseType(typeof(Result<PagedResult<CityListDto>>), 200)]
    public async Task<IActionResult> GetByCountry(int countryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var query = new GetAllCitiesQuery 
        { 
            CountryId = countryId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        
        var result = await _mediator.Send(query);
        return Ok(Result<PagedResult<CityListDto>>.Success(result, "تم جلب مدن الدولة بنجاح"));
    }

    /// <summary>
    /// الحصول على مدينة بمعرفها
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<CityDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetCityByIdQuery(id));
        
        if (result == null)
            return NotFound(Result<CityDto>.Failure("المدينة غير موجودة", 404));

        return Ok(Result<CityDto>.Success(result, "تم جلب بيانات المدينة بنجاح"));
    }

    /// <summary>
    /// إضافة مدينة جديدة
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [ProducesResponseType(typeof(Result<int>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateCityCommand command)
    {
        var cityId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = cityId }, 
            Result<int>.Success(cityId, "تم إضافة المدينة بنجاح"));
    }

    /// <summary>
    /// تحديث بيانات مدينة
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [ProducesResponseType(typeof(Result<int>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCityDto dto)
    {
        var command = new UpdateCityCommand
        {
            CityId = id,
            CountryId = dto.CountryId,
            CityNameAr = dto.CityNameAr,
            CityNameEn = dto.CityNameEn,
            IsActive = dto.IsActive
        };

        try
        {
            var result = await _mediator.Send(command);
            return Ok(Result<int>.Success(result, "تم تحديث بيانات المدينة بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<int>.Failure(ex.Message, 404));
        }
    }

    /// <summary>
    /// حذف مدينة
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "System_Admin")]
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteCityCommand(id));
            return Ok(Result<bool>.Success(result, "تم حذف المدينة بنجاح"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(Result<bool>.Failure(ex.Message, 404));
        }
    }
}
