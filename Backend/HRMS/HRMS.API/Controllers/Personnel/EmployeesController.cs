using HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee;
using HRMS.Application.Features.Personnel.Employees.Commands.UpdateEmployee;
using HRMS.Application.Features.Personnel.Employees.Commands.DeleteEmployee;
using HRMS.Application.Features.Personnel.Employees.Commands.UploadProfilePicture;
using HRMS.Application.Features.Personnel.Employees.Commands.UpdateStatus;
using HRMS.Application.Features.Personnel.Employees.Queries.GetAllEmployees;
using HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeProfile;
using HRMS.Application.Features.Personnel.Employees.Queries.GetAuditHistory;
using HRMS.Application.DTOs.Personnel;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Personnel;

/// <summary>
/// إدارة الموظفين (البيانات الأساسية)
/// Employees Core Management
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// جلب قائمة الموظفين (مع البحث والفلترة)
    /// Get All Employees (Paged & Filtered)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<List<EmployeeListDto>>>> GetAll([FromQuery] GetAllEmployeesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// جلب بيانات موظف محدد (البيانات الأساسية فقط)
    /// Get Single Employee (Basic Info)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<EmployeeProfileDto>>> GetById(int id)
    {
        var result = await _mediator.Send(new GetEmployeeProfileQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// إنشاء موظف جديد
    /// Create New Employee
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreateEmployeeDto dto)
    {
        var command = new CreateEmployeeCommand(dto);
        var result = await _mediator.Send(command);
        return Ok(Result<int>.Success(result, "Employee created successfully"));
    }

    /// <summary>
    /// تحديث البيانات الأساسية للموظف
    /// Update Employee Basic Info
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Result<bool>>> Update(int id, [FromBody] CreateEmployeeDto dto)
    {
        var command = new UpdateEmployeeCommand(id, dto);
        await _mediator.Send(command);
        return Ok(Result<bool>.Success(true, "Employee updated successfully"));
    }

    /// <summary>
    /// حذف موظف (حذف منطقي)
    /// Soft Delete Employee
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteEmployeeCommand(id));
        return Ok(Result<bool>.Success(result, "Employee deleted successfully"));
    }

    /// <summary>
    /// رفع صورة الملف الشخصي
    /// Upload Profile Picture
    /// </summary>
    [HttpPut("{id}/photo")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<string>>> UploadPhoto(int id, IFormFile photo)
    {
        var command = new UploadProfilePictureCommand 
        { 
            EmployeeId = id, 
            ProfilePicture = photo 
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// تحديث حالة الموظف (تفعيل/تعطيل/إنهاء)
    /// Update Employee Status
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<Result<bool>>> UpdateStatus(int id, [FromBody] UpdateEmployeeStatusCommand command)
    {
        if (id != command.EmployeeId)
            command.EmployeeId = id;

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// جلب سجل التعديلات للموظف
    /// Get Audit History
    /// </summary>
    [HttpGet("{id}/audit")]
    public async Task<ActionResult<Result<List<AuditHistoryDto>>>> GetAudit(int id)
    {
        var result = await _mediator.Send(new GetEmployeeAuditHistoryQuery { EmployeeId = id });
        return Ok(result);
    }
}
