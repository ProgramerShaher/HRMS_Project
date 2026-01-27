using HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee;
using HRMS.Application.Features.Personnel.Employees.Commands.UploadEmployeeDocument;
using HRMS.Application.Features.Personnel.Employees.Commands.UpdateEmployee;
using HRMS.Application.Features.Personnel.Employees.Commands.DeleteEmployee;
using HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeProfile;
using HRMS.Application.Features.Personnel.Employees.Queries.GetAllEmployees;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.DTOs.Core;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace HRMS.API.Controllers.Personnel;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeProfileDto>> GetProfile(int id)
    {
        var result = await _mediator.Send(new GetEmployeeProfileQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateEmployeeDto dto)
    {
        var command = new CreateEmployeeCommand(dto);
        var employeeId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProfile), new { id = employeeId }, employeeId);
    }

    [HttpPost("{id}/documents")]
    public async Task<ActionResult<int>> UploadDocument(int id, [FromForm] UploadEmployeeDocumentCommand command)
    {
        if (id != command.EmployeeId) return BadRequest("Employee ID mismatch");
        
        var documentId = await _mediator.Send(command);
        return Ok(documentId);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeListDto>>> GetAll([FromQuery] GetAllEmployeesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] CreateEmployeeDto dto)
    {
        var command = new UpdateEmployeeCommand(id, dto);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteEmployeeCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }
}
