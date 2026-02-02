using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Features.Leaves.LeaveBalances.Commands.AdjustBalance;
using HRMS.Application.Features.Leaves.LeaveBalances.Commands.InitializeBalances;
using HRMS.Application.Features.Leaves.LeaveBalances.Queries.GetEmployeeBalance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Leaves;

/// <summary>
/// Leave Balance Management Controller.
/// Handles balance initialization, adjustments, and queries.
/// </summary>
[ApiController]
[Route("api/Leaves/Balances")]
[Produces("application/json")]
public class LeaveBalanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveBalanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ═══════════════════════════════════════════════════════════
    // تهيئة الأرصدة السنوية الذكية
    // Smart Initialize Yearly Balances
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Initialize yearly leave balances with smart proration logic.
    /// Supports filtering by leave type and department, with optional proration for mid-year hires.
    /// </summary>
    /// <param name="dto">Initialization parameters</param>
    /// <returns>Success result with creation/update counts</returns>
    [HttpPost("initialize")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<bool>>> InitializeBalances([FromBody] InitializeBalancesDto dto)
    {
        var command = new InitializeBalancesCommand
        {
            LeaveTypeId = dto.LeaveTypeId,
            Year = dto.Year,
            DepartmentId = dto.DepartmentId,
            CustomDays = dto.CustomDays,
            EnableProration = dto.EnableProration
        };

        var result = await _mediator.Send(command);
        
        if (result.Succeeded)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }

    // ═══════════════════════════════════════════════════════════
    // جلب رصيد موظف محدد
    // Get Employee Balance
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Get leave balances for a specific employee.
    /// Returns all leave types with current balances.
    /// </summary>
    /// <param name="employeeId">Employee ID</param>
    /// <param name="year">Optional year filter (defaults to current year)</param>
    /// <returns>List of leave balances</returns>
    [HttpGet("employee/{employeeId}")]
    [Authorize]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<List<LeaveBalanceDto>>>> GetEmployeeBalance(
        int employeeId, 
        [FromQuery] short? year = null)
    {
        var result = await _mediator.Send(new GetEmployeeBalanceQuery 
        { 
            EmployeeId = employeeId, 
            Year = year 
        });
        
        if (result.Succeeded)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }

    // ═══════════════════════════════════════════════════════════
    // تعديل رصيد موظف يدوياً
    // Adjust Employee Balance
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// Manually adjust an employee's leave balance.
    /// Used for corrections, bonuses, or penalties.
    /// </summary>
    /// <param name="command">Adjustment details including reason</param>
    /// <returns>Success result with new balance</returns>
    [HttpPost("adjust")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [AllowAnonymous] // 🔓 للتطوير فقط - احذف هذا السطر في الإنتاج
    public async Task<ActionResult<Result<bool>>> AdjustBalance([FromBody] AdjustBalanceCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.Succeeded)
            return Ok(result);
        
        return StatusCode(result.StatusCode, result);
    }
}
