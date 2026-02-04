//using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Certifications;
//using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Qualifications;
//using HRMS.Application.Features.Personnel.Employees.Commands.UpdateStatus;
//using HRMS.Application.Features.Personnel.Employees.Commands.UploadProfilePicture;
//using HRMS.Application.Features.Personnel.Employees.Queries.GetAuditHistory;
//using HRMS.Core.Utilities;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace HRMS.API.Controllers.Personnel;

///// <summary>
///// واجهة برمجية لإدارة ملفات الموظفين ودورة حياتهم
///// Employee File Management and Lifecycle API
///// </summary>
//[Route("api/[controller]")]
//[ApiController]
//public class EmployeeManagementController : ControllerBase
//{
//    private readonly IMediator _mediator;

//    public EmployeeManagementController(IMediator mediator)
//    {
//        _mediator = mediator;
//    }

//    /// <summary>
//    /// رفع صورة الملف الشخصي للموظف
//    /// Upload Employee Profile Picture
//    /// </summary>
//    /// <param name="id">معرف الموظف</param>
//    /// <param name="profilePicture">ملف الصورة (jpg, png فقط)</param>
//    [HttpPut("{id}/profile-picture")]
//    [Consumes("multipart/form-data")]
//    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> UploadProfilePicture(
//        int id,
//        IFormFile profilePicture)
//    {
//        var command = new UploadProfilePictureCommand
//        {
//            EmployeeId = id,
//            ProfilePicture = profilePicture
//        };

//        var result = await _mediator.Send(command);
//        return result.Succeeded ? Ok(result) : BadRequest(result);
//    }

//    /// <summary>
//    /// إضافة مؤهل علمي مع مرفق
//    /// Add Qualification with Attachment
//    /// </summary>
//    [HttpPost("{id}/qualifications")]
//    [Consumes("multipart/form-data")]
//    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> AddQualification(
//        int id,
//        [FromForm] string degreeType,
//        [FromForm] string majorAr,
//        [FromForm] string? universityAr,
//        [FromForm] int? countryId,
//        [FromForm] short? graduationYear,
//        [FromForm] string? grade,
//        IFormFile? attachment)
//    {
//        var command = new AddQualificationWithAttachmentCommand
//        {
//            EmployeeId = id,
//            DegreeType = degreeType,
//            MajorAr = majorAr,
//            UniversityAr = universityAr,
//            CountryId = countryId,
//            GraduationYear = graduationYear,
//            Grade = grade,
//            Attachment = attachment
//        };

//        var result = await _mediator.Send(command);
//        return result.Succeeded ? Ok(result) : BadRequest(result);
//    }

//    /// <summary>
//    /// إضافة شهادة مهنية مع مرفق
//    /// Add Certification with Attachment
//    /// </summary>
//    [HttpPost("{id}/certifications")]
//    [Consumes("multipart/form-data")]
//    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> AddCertification(
//        int id,
//        [FromForm] string certNameAr,
//        [FromForm] string? issuingAuthority,
//        [FromForm] DateTime? issueDate,
//        [FromForm] DateTime? expiryDate,
//        [FromForm] string? certNumber,
//        [FromForm] byte isMandatory,
//        IFormFile? attachment)
//    {
//        var command = new AddCertificationCommand
//        {
//            EmployeeId = id,
//            CertNameAr = certNameAr,
//            IssuingAuthority = issuingAuthority,
//            IssueDate = issueDate,
//            ExpiryDate = expiryDate,
//            CertNumber = certNumber,
//            IsMandatory = isMandatory,
//            Attachment = attachment
//        };

//        var result = await _mediator.Send(command);
//        return result.Succeeded ? Ok(result) : BadRequest(result);
//    }

//    /// <summary>
//    /// تحديث حالة الموظف (تفعيل/تعطيل/إنهاء خدمة)
//    /// Update Employee Status (Activate/Deactivate/Terminate)
//    /// </summary>
//    /// <param name="id">معرف الموظف</param>
//    /// <param name="command">بيانات تحديث الحالة</param>
//    [HttpPut("{id}/status")]
//    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> UpdateStatus(
//        int id,
//        [FromBody] UpdateEmployeeStatusCommand command)
//    {
//        command.EmployeeId = id;
//        var result = await _mediator.Send(command);
//        return result.Succeeded ? Ok(result) : BadRequest(result);
//    }

//    /// <summary>
//    /// جلب سجل التدقيق للموظف
//    /// Get Employee Audit History
//    /// </summary>
//    /// <param name="id">معرف الموظف</param>
//    [HttpGet("{id}/audit-history")]
//    [ProducesResponseType(typeof(Result<List<AuditHistoryDto>>), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(Result<List<AuditHistoryDto>>), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> GetAuditHistory(int id)
//    {
//        var query = new GetEmployeeAuditHistoryQuery { EmployeeId = id };
//        var result = await _mediator.Send(query);
//        return result.Succeeded ? Ok(result) : BadRequest(result);
//    }
//}
