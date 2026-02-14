using HRMS.Application.DTOs.Payroll.Configuration;
using HRMS.Application.Features.Payroll.Configuration.Elements.Commands.CreateSalaryElement;
using HRMS.Application.Features.Payroll.Configuration.Elements.Commands.DeleteSalaryElement;
using HRMS.Application.Features.Payroll.Configuration.Elements.Commands.UpdateSalaryElement;
using HRMS.Application.Features.Payroll.Configuration.Elements.Queries.GetSalaryElements;
using HRMS.Application.Features.Payroll.Configuration.Structure.Commands.SetEmployeeSalaryStructure;
using HRMS.Application.Features.Payroll.Configuration.Structure.Commands.InitializeSalaryFromGrade;
using HRMS.Application.Features.Payroll.Configuration.Structure.Queries.GetEmployeeSalaryStructure;
using HRMS.Application.Features.Payroll.Configuration.Structure.Queries.GetAllStructures;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Payroll;

[Route("api/[controller]")]
[ApiController]
public class PayrollSettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PayrollSettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ═══════════════════════════════════════════════════════════
    // SALARY ELEMENTS
    // ═══════════════════════════════════════════════════════════

    [HttpGet("elements")]
    public async Task<ActionResult<Result<List<SalaryElementDto>>>> GetElements()
    {
        var result = await _mediator.Send(new GetSalaryElementsQuery());
        return Ok(result);
    }

    [HttpPost("elements")]
    public async Task<ActionResult<Result<int>>> CreateElement([FromBody] CreateSalaryElementCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("elements")]
    public async Task<ActionResult<Result<bool>>> UpdateElement([FromBody] UpdateSalaryElementCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("elements/{id}")]
    public async Task<ActionResult<Result<bool>>> DeleteElement(int id)
    {
        var result = await _mediator.Send(new DeleteSalaryElementCommand(id));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // EMPLOYEE STRUCTURE
    // ═══════════════════════════════════════════════════════════

    [HttpGet("employee-structure/{employeeId}")]
    public async Task<ActionResult<Result<EmployeeSalaryStructureDto>>> GetEmployeeStructure(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeSalaryStructureQuery(employeeId));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("update-structure")]
    public async Task<ActionResult<Result<bool>>> UpdateStructure([FromBody] SetEmployeeSalaryStructureCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("initialize-from-grade/{employeeId}")]
    public async Task<ActionResult<Result<bool>>> InitializeFromGrade(int employeeId)
    {
        var result = await _mediator.Send(new InitializeSalaryFromGradeCommand { EmployeeId = employeeId });
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على جدول شامل بهياكل رواتب جميع الموظفين
    /// Get comprehensive table of all employees' salary structures
    /// </summary>
    [HttpGet("all-structures")]
    public async Task<ActionResult<Result<List<EmployeeStructureSummaryDto>>>> GetAllStructures(
        [FromQuery] int? departmentId,
        [FromQuery] string? searchTerm)
    {
        var query = new GetAllStructuresQuery
        {
            DepartmentId = departmentId,
            SearchTerm = searchTerm
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
