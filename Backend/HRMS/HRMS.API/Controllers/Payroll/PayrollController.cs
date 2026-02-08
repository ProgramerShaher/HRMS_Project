using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.Features.Payroll.Processing.Commands.ProcessPayrun;
using HRMS.Application.Features.Payroll.Processing.Commands.Rollback;
using HRMS.Application.Features.Payroll.Processing.Commands.PostPayrollToGL;
using HRMS.Application.Features.Payroll.Processing.Queries.CalculateMonthlySalary;
using HRMS.Application.Features.Payroll.Processing.Queries.ExportBankFile;
using HRMS.Application.Features.Payroll.Processing.Services;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Payroll;

[Route("api/[controller]")]
[ApiController]
public class PayrollController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly BankFileExportService _bankFileExportService;

    public PayrollController(IMediator mediator, BankFileExportService bankFileExportService)
    {
        _mediator = mediator;
        _bankFileExportService = bankFileExportService;
    }

    [HttpPost("process-month")]
    public async Task<ActionResult<Result<int>>> ProcessMonth([FromQuery] int month, [FromQuery] int year)
    {
        var result = await _mediator.Send(new ProcessPayrunCommand { Month = month, Year = year });
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// التراجع عن مسير رواتب
    /// Rollback Payroll Run
    /// </summary>
    [HttpPost("rollback/{runId}")]
    public async Task<ActionResult<Result<bool>>> RollbackPayroll(int runId)
    {
        var result = await _mediator.Send(new RollbackPayrollCommand { RunId = runId });
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("payslip/{employeeId}/{month}/{year}")]
    public async Task<ActionResult<Result<MonthlySalaryCalculationDto>>> GetPayslip(int employeeId, int month, int year)
    {
        var result = await _mediator.Send(new CalculateMonthlySalaryQuery { EmployeeId = employeeId, Month = month, Year = year });
        return Ok(result);
    }

    [HttpGet("export-bank-file/{month}/{year}")]
    public async Task<IActionResult> ExportBankFile(int month, int year)
    {
        try
        {
            // Direct Service Call (Queries DB internaly as per requirements)
            var excelBytes = await _bankFileExportService.ExportPayrollToExcelAsync(month, year);
            
            var fileName = $"Bank_Transfer_{month:D2}_{year}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(Result<int>.Failure($"Export Failed: {ex.Message}"));
        }
    }

    /// <summary>
    /// ترحيل مسير الرواتب إلى دليل الحسابات
    /// Post Payroll Run to General Ledger
    /// </summary>
    /// <param name="runId">معرف مسير الرواتب</param>
    [HttpPost("post-to-gl/{runId}")]
    public async Task<ActionResult<Result<long>>> PostPayrollToGL(int runId)
    {
        var result = await _mediator.Send(new PostPayrollToGLCommand { RunId = runId });
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
