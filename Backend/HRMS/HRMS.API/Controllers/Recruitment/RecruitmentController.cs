using HRMS.Application.Features.Recruitment.Offers.Commands.AcceptJobOffer;
using HRMS.Application.Features.Recruitment.Offers.Commands.Create;
using HRMS.Application.Features.Recruitment.Offers.Queries.GetAll;
using HRMS.Application.Features.Recruitment.Vacancies.Commands.Create;
using HRMS.Application.Features.Recruitment.Vacancies.Commands.Update;
using HRMS.Application.Features.Recruitment.Vacancies.Commands.Close;
using HRMS.Application.Features.Recruitment.Vacancies.Queries.GetAll;
using HRMS.Application.Features.Recruitment.Vacancies.Queries.GetById;
using HRMS.Application.Features.Recruitment.Candidates.Commands.Create;
using HRMS.Application.Features.Recruitment.Applications.Commands.Submit;
using HRMS.Application.Features.Recruitment.Applications.Commands.ChangeStatus;
using HRMS.Application.Features.Recruitment.Applications.Queries.GetAll;
using HRMS.Application.Features.Recruitment.Interviews.Commands.Schedule;
using HRMS.Application.Features.Recruitment.Interviews.Commands.RecordResult;
using HRMS.Application.Features.Recruitment.Interviews.Queries.GetAll;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Recruitment;

/// <summary>
/// وحدة تحكم التوظيف - نظام ERP متكامل
/// </summary>
/// <remarks>
/// توفر نقاط نهاية لإدارة دورة التوظيف الكاملة:
/// - المرشحين (Candidates Pool)
/// - الوظائف الشاغرة (Job Vacancies)
/// - طلبات التوظيف (Applications)
/// - المقابلات (Interviews)
/// - عروض العمل (Job Offers)
/// - **التوظيف التلقائي (Auto-Hire)** مع إنشاء هيكل الراتب
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecruitmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecruitmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ═══════════════════════════════════════════════════════════
    // JOB OFFERS - عروض العمل
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// قبول عرض توظيف - تحويل المرشح إلى موظف (Full Hiring Cycle)
    /// </summary>
    /// <param name="offerId">معرف العرض</param>
    /// <param name="command">بيانات الالتحاق</param>
    /// <returns>معرف الموظف الجديد</returns>
    /// <remarks>
    /// **التكامل ERP الكامل**:
    /// 
    /// 1️⃣ **تحديث العرض**: Status = "ACCEPTED"
    /// 
    /// 2️⃣ **التوظيف التلقائي (Auto-Hire)**:
    ///    - إنشاء سجل موظف جديد في `HR_PERSONNEL.EMPLOYEES`
    ///    - نسخ البيانات الشخصية من `CANDIDATES`:
    ///      - الاسم الكامل (عربي/إنجليزي)
    ///      - البريد الإلكتروني
    ///      - رقم الجوال
    ///      - الجنسية
    ///    - تعيين البيانات الوظيفية من `VACANCY`:
    ///      - الوظيفة (JobId)
    ///      - القسم (DepartmentId)
    ///      - تاريخ التعيين (HireDate)
    ///    - توليد رقم وظيفي فريد تلقائياً (نمط: EMP-YYYY-NNNN)
    /// 
    /// 3️⃣ **إعداد الهيكل المالي (Financial Setup)**:
    ///    - إنشاء `EMPLOYEE_SALARY_STRUCTURE` تلقائياً:
    ///      - **الراتب الأساسي** (Basic Salary من العرض)
    ///      - **بدل السكن** (Housing Allowance إن وجد)
    ///      - **بدل النقل** (Transport Allowance إن وجد)
    ///    - ربط العناصر بـ `SALARY_ELEMENTS` الموجودة في النظام
    /// 
    /// 4️⃣ **Transaction**: كل العمليات في معاملة واحدة (Atomicity)
    /// 
    /// **مثال JSON**:
    /// ```json
    /// {
    ///   "joiningDate": "2026-03-01",
    ///   "employeeNumber": "EMP-2026-0001"
    /// }
    /// ```
    /// 
    /// **النتيجة**:
    /// - ✅ موظف جديد في النظام
    /// - ✅ هيكل راتب كامل
    /// - ✅ جاهز لتشغيل الرواتب في أول payrun
    /// </remarks>
    [HttpPost("offers/{offerId}/accept")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<int>>> AcceptJobOffer(int offerId, [FromBody] AcceptJobOfferCommand command)
    {
        command.OfferId = offerId; // تعيين OfferId من الـ route
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// الحصول على جميع عروض العمل
    /// </summary>
    /// <param name="status">تصفية حسب الحالة (اختياري)</param>
    /// <returns>قائمة العروض</returns>
    /// <remarks>
    /// **TODO**: يحتاج تنفيذ GetJobOffersQuery
    /// سيعرض:
    /// - بيانات المرشح
    /// - الوظيفة المعروضة
    /// - تفاصيل الراتب والبدلات
    /// - حالة العرض (DRAFT, SENT, ACCEPTED, REJECTED)
    /// </remarks>
    [HttpGet("offers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobOffers([FromQuery] int? appId, [FromQuery] string? status)
    {
        var query = new GetOffersQuery { AppId = appId, Status = status };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// إنشاء عرض عمل جديد
    /// </summary>
    [HttpPost("offers")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> CreateJobOffer([FromBody] CreateOfferCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // CANDIDATES - المرشحين
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// الحصول على جميع المرشحين (Talent Pool)
    /// </summary>
    /// <returns>قائمة المرشحين</returns>
    /// <remarks>
    /// **TODO**: يحتاج تنفيذ GetCandidatesQuery
    /// </remarks>
    [HttpGet("candidates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCandidates()
    {
        // TODO: Implement GetCandidatesQuery
        return Ok(new { Message = "Endpoint requires implementation" });
    }

    /// <summary>
    /// إضافة مرشح جديد
    /// </summary>
    [HttpPost("candidates")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> CreateCandidate([FromBody] CreateCandidateCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // VACANCIES - الوظائف الشاغرة
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// الحصول على الوظائف الشاغرة
    /// </summary>
    [HttpGet("vacancies")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobVacancies([FromQuery] string? status)
    {
        var query = new GetVacanciesQuery { Status = status };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// إنشاء وظيفة شاغرة جديدة
    /// </summary>
    [HttpPost("vacancies")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> CreateJobVacancy([FromBody] CreateVacancyCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تحديث وظيفة شاغرة
    /// </summary>
    [HttpPut("vacancies/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> UpdateVacancy(int id, [FromBody] UpdateVacancyCommand command)
    {
        command.VacancyId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// إغلاق وظيفة شاغرة
    /// </summary>
    [HttpPut("vacancies/{id}/close")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> CloseVacancy(int id)
    {
        var command = new CloseVacancyCommand { VacancyId = id };
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على تفاصيل وظيفة شاغرة
    /// </summary>
    [HttpGet("vacancies/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVacancyById(int id)
    {
        var query = new GetVacancyByIdQuery { VacancyId = id };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // APPLICATIONS - طلبات التوظيف
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// الحصول على طلبات التوظيف
    /// </summary>
    [HttpGet("applications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobApplications([FromQuery] int? vacancyId, [FromQuery] int? candidateId, [FromQuery] string? status)
    {
        var query = new GetApplicationsQuery { VacancyId = vacancyId, CandidateId = candidateId, Status = status };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تقديم طلب توظيف
    /// </summary>
    [HttpPost("applications")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> SubmitJobApplication([FromBody] SubmitApplicationCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تغيير حالة طلب توظيف
    /// </summary>
    [HttpPut("applications/{id}/status")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> ChangeApplicationStatus(int id, [FromBody] ChangeApplicationStatusCommand command)
    {
        command.AppId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // ═══════════════════════════════════════════════════════════
    // INTERVIEWS - المقابلات
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// جدولة مقابلة لمرشح
    /// </summary>
    [HttpPost("interviews")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> ScheduleInterview([FromBody] ScheduleInterviewCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على المقابلات المجدولة
    /// </summary>
    [HttpGet("interviews")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInterviews([FromQuery] int? appId, [FromQuery] int? interviewerId)
    {
        var query = new GetInterviewsQuery { AppId = appId, InterviewerId = interviewerId };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تسجيل نتيجة مقابلة
    /// </summary>
    [HttpPut("interviews/{id}/result")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> RecordInterviewResult(int id, [FromBody] RecordInterviewResultCommand command)
    {
        command.InterviewId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
