using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.Features.Payroll.Processing.Commands.ProcessPayrun;
using HRMS.Application.Features.Payroll.Processing.Queries.CalculateMonthlySalary;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Payroll;

[Route("api/[controller]")]
[ApiController]
public class PayrollController : ControllerBase
{
    private readonly IMediator _mediator;

    public PayrollController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("process-month")]
    public async Task<ActionResult<Result<int>>> ProcessMonth([FromQuery] int month, [FromQuery] int year)
    {
        var result = await _mediator.Send(new ProcessPayrunCommand { Month = month, Year = year });
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payslip/{employeeId}/{month}/{year}")]
    public async Task<ActionResult<Result<MonthlySalaryCalculationDto>>> GetPayslip(int employeeId, int month, int year)
    {
        var result = await _mediator.Send(new CalculateMonthlySalaryQuery { EmployeeId = employeeId, Month = month, Year = year });
        return Ok(result);
    }
}
