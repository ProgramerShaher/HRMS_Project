using HRMS.Application.DTOs.Payroll;
using HRMS.Application.Features.Payroll.Loans.Commands.CreateLoan;
using HRMS.Application.Features.Payroll.Loans.Commands.ChangeStatus;
using HRMS.Application.Features.Payroll.Loans.Commands.Settle;
using HRMS.Application.Features.Payroll.Loans.Queries.GetById;
using HRMS.Application.Features.Payroll.Loans.Queries.GetEmployeeLoans;
using HRMS.Application.Features.Payroll.Loans.Queries.GetMonthlyInstallments;
using HRMS.Application.Features.Payroll.Loans.Queries.GetEmployeeInstallments;
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

    /// <summary>
    /// الحصول على تفاصيل قرض
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<LoanDto>>> GetLoanById(int id)
    {
        var result = await _mediator.Send(new GetLoanByIdQuery { LoanId = id });
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تغيير حالة القرض (PENDING → ACTIVE)
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<Result<bool>>> ChangeLoanStatus(int id, [FromBody] ChangeLoanStatusCommand command)
    {
        command.LoanId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تسوية مبكرة للقرض
    /// </summary>
    [HttpPost("{id}/settle")]
    public async Task<ActionResult<Result<bool>>> SettleLoan(int id, [FromBody] EarlySettlementCommand command)
    {
        command.LoanId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على قروض موظف
    /// </summary>
    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<Result<List<LoanDto>>>> GetEmployeeLoans(int employeeId, [FromQuery] string? status)
    {
        var result = await _mediator.Send(new GetEmployeeLoansQuery { EmployeeId = employeeId, Status = status });
        return Ok(result);
    }

    [HttpGet("installments")]
    public async Task<ActionResult<Result<List<LoanInstallmentDto>>>> GetInstallments([FromQuery] int month, [FromQuery] int year, [FromQuery] int? employeeId)
    {
        var query = new GetMonthlyInstallmentsQuery { Month = month, Year = year, EmployeeId = employeeId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("employee-schedule/{employeeId}")]
    public async Task<ActionResult<Result<List<LoanInstallmentDto>>>> GetEmployeeSchedule(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeInstallmentsQuery { EmployeeId = employeeId });
        return Ok(result);
    }
}
