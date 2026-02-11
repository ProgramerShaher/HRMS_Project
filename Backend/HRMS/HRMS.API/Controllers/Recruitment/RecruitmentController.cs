using HRMS.Application.Features.Recruitment.Offers.Commands.AcceptJobOffer;
using HRMS.Application.Features.Recruitment.Offers.Commands.Create;
using HRMS.Application.Features.Recruitment.Offers.Commands.Withdraw;
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
using HRMS.Application.Features.Recruitment.Candidates.Queries.GetAll;
using HRMS.Application.Features.Recruitment.Vacancies.Commands.Delete;
using HRMS.Application.Features.Recruitment.Applications.Queries.GetById;
using HRMS.Application.Features.Recruitment.Applications.Commands.Withdraw;
using HRMS.Application.Features.Recruitment.Interviews.Queries.GetById;
using HRMS.Application.Features.Recruitment.Interviews.Commands.Reschedule;
using HRMS.Application.Features.Recruitment.Interviews.Commands.Cancel;
using HRMS.Application.Features.Recruitment.Offers.Queries.GetById;
using HRMS.Application.Features.Recruitment.Offers.Commands.Update;
using HRMS.Application.Features.Recruitment.Config.Queries.GetInterviewTypes;
using HRMS.Application.Features.Recruitment.Config.Queries.GetJobGrades;
using HRMS.Application.Features.Recruitment.Config.Queries.GetRejectionReasons;
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

    /// <summary>
    /// الحصول على تفاصيل عرض عمل
    /// </summary>
    [HttpGet("offers/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOfferById(int id)
    {
        var query = new GetOfferByIdQuery { OfferId = id };
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// تحديث عرض عمل
    /// </summary>
    [HttpPut("offers/{id}")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> UpdateOffer(int id, [FromBody] UpdateOfferCommand command)
    {
        command.OfferId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// سحب عرض عمل
    /// </summary>
    [HttpPut("offers/{id}/withdraw")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> WithdrawOffer(int id, [FromBody] WithdrawOfferCommand command)
    {
        command.OfferId = id;
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
        var query = new GetCandidatesQuery();
        var result = await _mediator.Send(query);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// إضافة مرشح جديد
    /// </summary>
    [HttpPost("candidates")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> CreateCandidate([FromForm] CreateCandidateCommand command)
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
    /// حذف وظيفة شاغرة
    /// </summary>
    [HttpDelete("vacancies/{id}")]
    [Authorize(Roles = "System_Admin,HR_Manager")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> DeleteVacancy(int id)
    {
        var command = new DeleteVacancyCommand { VacancyId = id };
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
    /// الحصول على تفاصيل طلب توظيف
    /// </summary>
    [HttpGet("applications/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplicationById(int id)
    {
        var query = new GetApplicationByIdQuery { AppId = id };
        var result = await _mediator.Send(query);
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

    /// <summary>
    /// سحب طلب توظيف
    /// </summary>
    [HttpDelete("applications/{id}")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> WithdrawApplication(int id)
    {
        var command = new WithdrawApplicationCommand { AppId = id };
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
    /// الحصول على تفاصيل مقابلة
    /// </summary>
    [HttpGet("interviews/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInterviewById(int id)
    {
        var query = new GetInterviewByIdQuery { InterviewId = id };
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

    /// <summary>
    /// إعادة جدولة مقابلة
    /// </summary>
    [HttpPut("interviews/{id}/reschedule")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> RescheduleInterview(int id, [FromBody] RescheduleInterviewCommand command)
    {
        command.InterviewId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// إلغاء مقابلة
    /// </summary>
    [HttpDelete("interviews/{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<bool>>> CancelInterview(int id, [FromBody] CancelInterviewCommand command)
    {
        command.InterviewId = id;
        var result = await _mediator.Send(command);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
    // ═══════════════════════════════════════════════════════════
    // CONFIGURATION & LOOKUPS - الإعدادات والقوائم
    // ═══════════════════════════════════════════════════════════

    /// <summary>
    /// الحصول على جميع الدرجات الوظيفية
    /// </summary>
    [HttpGet("config/job-grades")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobGrades()
    {
        var result = await _mediator.Send(new GetJobGradesQuery());
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على أنواع المقابلات
    /// </summary>
    [HttpGet("config/interview-types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInterviewTypes()
    {
        var result = await _mediator.Send(new GetInterviewTypesQuery());
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// الحصول على أسباب الرفض
    /// </summary>
    [HttpGet("config/rejection-reasons")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRejectionReasons()
    {
        var result = await _mediator.Send(new GetRejectionReasonsQuery());
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// إحصائيات التوظيف
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetRecruitmentStats()
    {
         // TODO: Implement proper statistics query
        return Ok(new
        {
            data = new
            {
                totalVacancies = 0,
                activeCandidates = 0,
                pendingOffers = 0,
                hiredThisMonth = 0
            },
            succeeded = true,
            message = "تمت العملية بنجاح"
        });
    }
}
