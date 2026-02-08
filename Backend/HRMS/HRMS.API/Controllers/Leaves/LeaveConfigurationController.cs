using HRMS.Application.DTOs.Core;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Features.Leaves.LeaveTypes.Commands.CreateLeaveType;
using HRMS.Application.Features.Leaves.LeaveTypes.Commands.DeleteLeaveType; // New
using HRMS.Application.Features.Leaves.LeaveTypes.Commands.UpdateLeaveType;
using HRMS.Application.Features.Leaves.LeaveTypes.Queries.GetAllLeaveTypes;
using HRMS.Application.Features.Leaves.LeaveTypes.Queries.GetLeaveTypeById; // New
using HRMS.Application.Features.Leaves.PublicHolidays.Commands.CreatePublicHoliday; // New
using HRMS.Application.Features.Leaves.PublicHolidays.Commands.DeletePublicHoliday;
using HRMS.Core.Utilities;
using HRMS.Core.Entities.Core;
using HRMS.Core.Entities.Leaves;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRMS.API.Controllers;
using HRMS.Application.Features.Leaves.LeaveBalances.Commands.InitializeYearlyBalance;
using HRMS.Application.Features.Core.SystemSettings.Queries.GetAllSettings;
using HRMS.Application.Features.Core.SystemSettings.Commands.UpdateSetting;
using HRMS.Application.Features.Leaves.PublicHolidays.Queries.GetPublicHolidays;

namespace HRMS.API.Controllers.Leaves;

[ApiController]
[Route("api/[controller]")]
public class LeaveConfigurationController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveConfigurationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ═══════════════════════════════════════════════════════════
    // Leave Types Endpoints
    // ═══════════════════════════════════════════════════════════

    // جلب كافة أنواع الإجازات
    [HttpGet("leave-types")]
    [Authorize]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج

    public async Task<ActionResult<Result<List<LeaveTypeDto>>>> GetAllLeaveTypes()
    {
        var result = await _mediator.Send(new GetAllLeaveTypesQuery());
        return Ok(result);
    }

    // إضافة نوع إجازة جديد
    [HttpPost("leave-types")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج

    public async Task<ActionResult<Result<int>>> CreateLeaveType([FromBody] CreateLeaveTypeCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // تعديل بيانات نوع إجازة
    [HttpPut("leave-types/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج

    public async Task<ActionResult<Result<bool>>> UpdateLeaveType(int id, [FromBody] UpdateLeaveTypeCommand command)
    {
        if (id != command.LeaveTypeId)
            return BadRequest(Result<bool>.Failure("ID mismatch"));

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // جلب نوع إجازة بواسطة المعرف
    [HttpGet("leave-types/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط
    public async Task<ActionResult<Result<LeaveTypeDto>>> GetLeaveTypeById(int id)
    {
        var result = await _mediator.Send(new GetLeaveTypeByIdQuery(id));
        return Ok(result);
    }


    // حذف نوع إجازة (حذف ناعم)
    [HttpDelete("leave-types/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط
    public async Task<ActionResult<Result<bool>>> DeleteLeaveType(int id)
    {
        var result = await _mediator.Send(new DeleteLeaveTypeCommand());
        return Ok(result);
    }

    // ═══════════════════════════════════════════════════════════
    // System Settings Endpoints
    // ═══════════════════════════════════════════════════════════

    // جلب إعدادات النظام
    [HttpGet("settings")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<List<SystemSettingDto>>>> GetSystemSettings()
    {
        var result = await _mediator.Send(new GetAllSettingsQuery());
        return Ok(Result<List<SystemSettingDto>>.Success(result));
    }

    // تحديث إعدادات النظام
    [HttpPut("settings")]
    [Authorize(Roles = "System_Admin")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<bool>>> UpdateSystemSetting([FromBody] List<SystemSettingDto> settings)
    {
        // Iterate and update each setting
        // Note: Ideally sending a specific update command or handling batch update in a separate command
        // For now, loop and send command for each (or create BatchUpdate command)
        
        // Since the prompt requested UpdateSetting (Singular logic) but the placeholder was List
        // The user requirement says: "Feature: UpdateSetting (Command): Update SettingValue for a specific SettingKey."
        // So I should probably expose a singular endpoint or handle the list.
        // Let's support the List as per current signature but iterate.
        
        foreach (var setting in settings)
        {
            await _mediator.Send(new UpdateSettingCommand(setting.SettingKey, setting.SettingValue));
        }

        return Ok(Result<bool>.Success(true, "تم تحديث الإعدادات بنجاح")); 
    }

    // ═══════════════════════════════════════════════════════════
    // Public Holidays Endpoints
    // ═══════════════════════════════════════════════════════════

    // جلب العطل الرسمية
    [HttpGet("public-holidays")]
    [Authorize]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج

    public async Task<ActionResult<Result<List<PublicHolidayDto>>>> GetAllPublicHolidays([FromQuery] short? year)
    {
        var result = await _mediator.Send(new GetPublicHolidaysQuery { Year = year });
        return Ok(result);
    }

    // إضافة عطلة رسمية جديدة
    [HttpPost("public-holidays")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج

    public async Task<ActionResult<Result<int>>> CreatePublicHoliday([FromBody] CreatePublicHolidayCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // حذف عطلة رسمية
    [HttpDelete("public-holidays/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<bool>>> DeletePublicHoliday(int id)
    {
        var result = await _mediator.Send(new DeletePublicHolidayCommand(id));
        return Ok(result);
    }

}
