using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.DTOs.Payroll.Configuration;
using HRMS.Application.Features.Payroll.Processing.Commands.ProcessPayrun;
using HRMS.Application.Features.Payroll.Processing.Commands.Rollback;
using HRMS.Application.Features.Payroll.Processing.Commands.PostPayrollToGL;
using HRMS.Application.Features.Payroll.Processing.Queries.CalculateMonthlySalary;
using HRMS.Application.Features.Payroll.Processing.Queries.GetAllEmployeesSalaries;
using HRMS.Application.Features.Payroll.Processing.Queries.GetEmployeeSalaryBreakdown;
using HRMS.Application.Features.Payroll.Processing.Queries.GetPayrollRuns;
using HRMS.Application.Features.Payroll.Processing.Queries.GetPayrollRunDetails;
using HRMS.Application.Features.Payroll.Processing.Queries.ExportBankFile;
using HRMS.Application.Features.Payroll.Reports.Queries.GetMonthlySummary;
using HRMS.Application.Features.Payroll.Reports.Queries.GetAuditTrail;
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

    /// <summary>
    /// الحصول على جدول شامل برواتب جميع الموظفين
    /// Get comprehensive table of all employees' salaries
    /// </summary>
    [HttpGet("all-employees-salaries")]
    public async Task<ActionResult<Result<List<EmployeeSalaryDetailDto>>>> GetAllEmployeesSalaries(
        [FromQuery] int? departmentId,
        [FromQuery] bool? isActive,
        [FromQuery] string? searchTerm)
    {
        var query = new GetAllEmployeesSalariesQuery
        {
            DepartmentId = departmentId,
            IsActive = isActive,
            SearchTerm = searchTerm
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// الحصول على تفاصيل دقيقة لراتب موظف محدد
    /// Get detailed salary breakdown for specific employee
    /// </summary>
    [HttpGet("employee-salary-breakdown/{employeeId}")]
    public async Task<ActionResult<Result<SalaryBreakdownDto>>> GetEmployeeSalaryBreakdown(
        int employeeId,
        [FromQuery] int? month,
        [FromQuery] int? year)
    {
        var query = new GetEmployeeSalaryBreakdownQuery
        {
            EmployeeId = employeeId,
            Month = month,
            Year = year
        };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على قائمة بجميع مسيرات الرواتب
    /// Get list of all payroll runs
    /// </summary>
    [HttpGet("runs")]
    public async Task<ActionResult<Result<List<PayrollRunDto>>>> GetPayrollRuns(
        [FromQuery] int? year,
        [FromQuery] string? status)
    {
        var query = new GetPayrollRunsQuery
        {
            Year = year,
            Status = status
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// الحصول على تفاصيل مسير رواتب محدد
    /// Get details of specific payroll run
    /// </summary>
    [HttpGet("runs/{runId}/details")]
    public async Task<ActionResult<Result<PayrollRunDetailsDto>>> GetPayrollRunDetails(int runId)
    {
        var query = new GetPayrollRunDetailsQuery { RunId = runId };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على ملخص شهري للرواتب
    /// Get monthly payroll summary report
    /// </summary>
    [HttpGet("reports/monthly-summary")]
    public async Task<ActionResult<Result<MonthlyPayrollSummaryDto>>> GetMonthlySummary(
        [FromQuery] int month,
        [FromQuery] int year,
        [FromQuery] int? departmentId)
    {
        var query = new GetMonthlySummaryQuery
        {
            Month = month,
            Year = year,
            DepartmentId = departmentId
        };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على سجل التدقيق لعمليات الرواتب
    /// Get audit trail for payroll operations
    /// </summary>
    [HttpGet("reports/audit-trail")]
    public async Task<ActionResult<Result<List<PayrollAuditDto>>>> GetAuditTrail(
        [FromQuery] string? entityName,
        [FromQuery] int? entityId,
        [FromQuery] string? action,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetAuditTrailQuery
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            DateFrom = dateFrom,
            DateTo = dateTo,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
