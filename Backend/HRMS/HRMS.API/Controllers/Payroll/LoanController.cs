using HRMS.Application.DTOs.Payroll;
using HRMS.Application.Features.Payroll.Loans.Commands.CreateLoan;
using HRMS.Application.Features.Payroll.Loans.Queries.GetMonthlyInstallments;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Payroll;

[Route("api/[controller]")]
[ApiController]
public class LoanController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoanController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Result<int>>> CreateLoan([FromBody] CreateLoanCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("installments")]
    public async Task<ActionResult<Result<List<LoanInstallmentDto>>>> GetInstallments([FromQuery] int month, [FromQuery] int year, [FromQuery] int? employeeId)
    {
        var query = new GetMonthlyInstallmentsQuery { Month = month, Year = year, EmployeeId = employeeId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
