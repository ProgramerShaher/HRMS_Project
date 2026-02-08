using HRMS.Application.Features.Performance.Violations.Commands.RegisterViolation;
using HRMS.Application.Features.Performance.Appraisals.Commands.SubmitAppraisal;
using HRMS.Application.Features.Performance.ViolationTypes.Commands.Create;
using HRMS.Application.Features.Performance.ViolationTypes.Commands.Update;
using HRMS.Application.Features.Performance.ViolationTypes.Commands.Delete;
using HRMS.Application.Features.Performance.ViolationTypes.Queries.GetAll;
using HRMS.Application.Features.Performance.ViolationTypes.Queries.GetById;
using HRMS.Application.Features.Performance.DisciplinaryActions.Commands.Create;
using HRMS.Application.Features.Performance.DisciplinaryActions.Commands.Update;
using HRMS.Application.Features.Performance.DisciplinaryActions.Commands.Delete;
using HRMS.Application.Features.Performance.DisciplinaryActions.Queries.GetAll;
using HRMS.Application.Features.Performance.DisciplinaryActions.Queries.GetById;
using HRMS.Application.Features.Performance.KpiLibrary.Commands.Create;
using HRMS.Application.Features.Performance.KpiLibrary.Commands.Update;
using HRMS.Application.Features.Performance.KpiLibrary.Commands.Delete;
using HRMS.Application.Features.Performance.KpiLibrary.Queries.GetAll;
using HRMS.Application.Features.Performance.KpiLibrary.Queries.GetById;
using HRMS.Application.Features.Performance.AppraisalCycles.Commands.Create;
using HRMS.Application.Features.Performance.AppraisalCycles.Commands.Update;
using HRMS.Application.Features.Performance.AppraisalCycles.Commands.Delete;
using HRMS.Application.Features.Performance.AppraisalCycles.Queries.GetAll;
using HRMS.Application.Features.Performance.AppraisalCycles.Queries.GetById;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Performance;

/// <summary>
/// وحدة تحكم الأداء والجزاءات - نظام ERP متكامل
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PerformanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public PerformanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ═══════════════════════════════════════════════════════════
    // VIOLATIONS - المخالفات الإدارية
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// تسجيل مخالفة إدارية مع حساب الخصم المالي تلقائياً
    /// </summary>
    [HttpPost("violations")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> RegisterViolation([FromBody] RegisterViolationCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // APPRAISALS - تقييمات الأداء
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// تسجيل تقييم أداء موظف
    /// </summary>
    [HttpPost("appraisals")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> SubmitAppraisal([FromBody] SubmitAppraisalCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // CONFIGURATION: VIOLATION TYPES - أنواع المخالفات
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// إنشاء نوع مخالفة جديد (Admin Only)
    /// </summary>
    [HttpPost("config/violation-types")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> CreateViolationType([FromBody] CreateViolationTypeCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تحديث نوع مخالفة (Admin Only)
    /// </summary>
    [HttpPut("config/violation-types/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> UpdateViolationType(int id, [FromBody] UpdateViolationTypeCommand command)
    {
        command.ViolationTypeId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// حذف نوع مخالفة (Admin Only)
    /// </summary>
    [HttpDelete("config/violation-types/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> DeleteViolationType(int id)
    {
        var command = new DeleteViolationTypeCommand { ViolationTypeId = id };
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على جميع أنواع المخالفات
    /// </summary>
    [HttpGet("config/violation-types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetViolationTypes()
    {
        var query = new GetViolationTypesQuery();
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على نوع مخالفة بمعرفه
    /// </summary>
    [HttpGet("config/violation-types/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetViolationTypeById(int id)
    {
        var query = new GetViolationTypeByIdQuery { ViolationTypeId = id };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // CONFIGURATION: DISCIPLINARY ACTIONS - الإجراءات التأديبية
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// إنشاء إجراء تأديبي جديد (Admin Only)
    /// </summary>
    [HttpPost("config/disciplinary-actions")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> CreateDisciplinaryAction([FromBody] CreateDisciplinaryActionCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تحديث إجراء تأديبي (Admin Only)
    /// </summary>
    [HttpPut("config/disciplinary-actions/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> UpdateDisciplinaryAction(int id, [FromBody] UpdateDisciplinaryActionCommand command)
    {
        command.ActionId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// حذف إجراء تأديبي (Admin Only)
    /// </summary>
    [HttpDelete("config/disciplinary-actions/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> DeleteDisciplinaryAction(int id)
    {
        var command = new DeleteDisciplinaryActionCommand { ActionId = id };
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على جميع الإجراءات التأديبية
    /// </summary>
    [HttpGet("config/disciplinary-actions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDisciplinaryActions()
    {
        var query = new GetDisciplinaryActionsQuery();
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على إجراء تأديبي بمعرفه
    /// </summary>
    [HttpGet("config/disciplinary-actions/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDisciplinaryActionById(int id)
    {
        var query = new GetDisciplinaryActionByIdQuery { ActionId = id };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // CONFIGURATION: KPI LIBRARY - مكتبة مؤشرات الأداء
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// إنشاء مؤشر أداء جديد (Admin Only)
    /// </summary>
    [HttpPost("config/kpis")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> CreateKpi([FromBody] CreateKpiCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تحديث مؤشر أداء (Admin Only)
    /// </summary>
    [HttpPut("config/kpis/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> UpdateKpi(int id, [FromBody] UpdateKpiCommand command)
    {
        command.KpiId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// حذف مؤشر أداء (Admin Only)
    /// </summary>
    [HttpDelete("config/kpis/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> DeleteKpi(int id)
    {
        var command = new DeleteKpiCommand { KpiId = id };
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على جميع مؤشرات الأداء
    /// </summary>
    [HttpGet("config/kpis")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKpis()
    {
        var query = new GetKpisQuery();
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على مؤشر أداء بمعرفه
    /// </summary>
    [HttpGet("config/kpis/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKpiById(int id)
    {
        var query = new GetKpiByIdQuery { KpiId = id };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // CONFIGURATION: APPRAISAL CYCLES - فترات التقييم
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// إنشاء فترة تقييم جديدة (Admin Only)
    /// </summary>
    [HttpPost("config/appraisal-cycles")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> CreateAppraisalCycle([FromBody] CreateAppraisalCycleCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تحديث فترة تقييم (Admin Only)
    /// </summary>
    [HttpPut("config/appraisal-cycles/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> UpdateAppraisalCycle(int id, [FromBody] UpdateAppraisalCycleCommand command)
    {
        command.CycleId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// حذف فترة تقييم (Admin Only)
    /// </summary>
    [HttpDelete("config/appraisal-cycles/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager,Admin")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> DeleteAppraisalCycle(int id)
    {
        var command = new DeleteAppraisalCycleCommand { CycleId = id };
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على جميع فترات التقييم
    /// </summary>
    [HttpGet("config/appraisal-cycles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppraisalCycles()
    {
        var query = new GetAppraisalCyclesQuery();
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على فترة تقييم بمعرفها
    /// </summary>
    [HttpGet("config/appraisal-cycles/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppraisalCycleById(int id)
    {
        var query = new GetAppraisalCycleByIdQuery { CycleId = id };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
