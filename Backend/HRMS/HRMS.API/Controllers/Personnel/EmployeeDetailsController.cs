//using HRMS.Application.DTOs.Personnel;
//using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.EmergencyContacts;
//using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Experiences;
//using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Qualifications;
//using HRMS.Application.Features.Personnel.EmployeeDetails.Queries.EmergencyContacts;
//using HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Experiences;
//using HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Qualifications;
//using HRMS.Application.Features.Personnel.Employees.DTOs;
//using HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeFullProfile;
//using HRMS.Application.Features.Personnel.Contracts.Commands.CreateContract;
//using HRMS.Application.Features.Personnel.Contracts.Commands.RenewContract;
//using HRMS.Application.Features.Personnel.Contracts.Queries.GetEmployeeContracts;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;

//namespace HRMS.API.Controllers.Personnel;

//[Route("api/employees/{employeeId}")]
//[ApiController]
//public class EmployeeDetailsController : ControllerBase
//{
//    private readonly IMediator _mediator;

//    public EmployeeDetailsController(IMediator mediator)
//    {
//        _mediator = mediator;
//    }

//    // ----------------------------------------------------------------------------------
//    // Full Profile
//    // ----------------------------------------------------------------------------------

//    [HttpGet("full-profile")]
//    public async Task<ActionResult<DetailedEmployeeProfileDto>> GetFullProfile(int employeeId)
//    {
//        var result = await _mediator.Send(new GetEmployeeFullProfileQuery(employeeId));
//        return Ok(result);
//    }

//    // ----------------------------------------------------------------------------------
//    // Qualifications
//    // ----------------------------------------------------------------------------------

//    [HttpGet("qualifications")]
//    public async Task<ActionResult<List<EmployeeQualificationDto>>> GetQualifications(int employeeId)
//    {
//        var result = await _mediator.Send(new GetEmployeeQualificationsQuery(employeeId));
//        return Ok(result);
//    }

//    [HttpPost("qualifications")]
//    public async Task<ActionResult<int>> AddQualification(int employeeId, [FromBody] EmployeeQualificationDto dto)
//    {
//        var command = new AddQualificationCommand(employeeId, dto);
//        var result = await _mediator.Send(command);
//        return Ok(result);
//    }

//    [HttpPut("qualifications/{qualificationId}")]
//    public async Task<ActionResult> UpdateQualification(int employeeId, int qualificationId, [FromBody] EmployeeQualificationDto dto)
//    {
//        if (dto.QualificationId != null && dto.QualificationId != qualificationId)
//            return BadRequest("Qualification ID Mismatch");

//        await _mediator.Send(new UpdateQualificationCommand(employeeId, qualificationId, dto));
//        return NoContent();
//    }

//    [HttpDelete("qualifications/{qualificationId}")]
//    public async Task<ActionResult> DeleteQualification(int employeeId, int qualificationId)
//    {
//        await _mediator.Send(new DeleteQualificationCommand(employeeId, qualificationId));
//        return NoContent();
//    }

//    // ----------------------------------------------------------------------------------
//    // Experiences
//    // ----------------------------------------------------------------------------------

//    [HttpGet("experiences")]
//    public async Task<ActionResult<List<EmployeeExperienceDto>>> GetExperiences(int employeeId)
//    {
//        var result = await _mediator.Send(new GetEmployeeExperiencesQuery(employeeId));
//        return Ok(result);
//    }

//    [HttpPost("experiences")]
//    public async Task<ActionResult<int>> AddExperience(int employeeId, [FromBody] EmployeeExperienceDto dto)
//    {
//        var command = new AddExperienceCommand(employeeId, dto);
//        var result = await _mediator.Send(command);
//        return Ok(result);
//    }

//    [HttpPut("experiences/{experienceId}")]
//    public async Task<ActionResult> UpdateExperience(int employeeId, int experienceId, [FromBody] EmployeeExperienceDto dto)
//    {
//        if (dto.ExperienceId != null && dto.ExperienceId != experienceId)
//            return BadRequest("Experience ID Mismatch");

//        await _mediator.Send(new UpdateExperienceCommand(employeeId, experienceId, dto));
//        return NoContent();
//    }

//    [HttpDelete("experiences/{experienceId}")]
//    public async Task<ActionResult> DeleteExperience(int employeeId, int experienceId)
//    {
//        await _mediator.Send(new DeleteExperienceCommand(employeeId, experienceId));
//        return NoContent();
//    }

//    // ----------------------------------------------------------------------------------
//    // Emergency Contacts
//    // ----------------------------------------------------------------------------------

//    [HttpGet("emergency-contacts")]
//    public async Task<ActionResult<List<EmergencyContactDto>>> GetEmergencyContacts(int employeeId)
//    {
//        var result = await _mediator.Send(new GetEmployeeEmergencyContactsQuery(employeeId));
//        return Ok(result);
//    }

//    [HttpPost("emergency-contacts")]
//    public async Task<ActionResult<int>> AddEmergencyContact(int employeeId, [FromBody] EmergencyContactDto dto)
//    {
//        var command = new AddEmergencyContactCommand(employeeId, dto);
//        var result = await _mediator.Send(command);
//        return Ok(result);
//    }

//    [HttpPut("emergency-contacts/{contactId}")]
//    public async Task<ActionResult> UpdateEmergencyContact(int employeeId, int contactId, [FromBody] EmergencyContactDto dto)
//    {
//        if (dto.ContactId != null && dto.ContactId != contactId)
//            return BadRequest("Contact ID Mismatch");

//        await _mediator.Send(new UpdateEmergencyContactCommand(employeeId, contactId, dto));
//        return NoContent();
//    }

//    [HttpDelete("emergency-contacts/{contactId}")]
//    public async Task<ActionResult> DeleteEmergencyContact(int employeeId, int contactId)
//    {
//        await _mediator.Send(new DeleteEmergencyContactCommand(employeeId, contactId));
//        return NoContent();
//    }

//    // ----------------------------------------------------------------------------------
//    // Contracts
//    // ----------------------------------------------------------------------------------

//    [HttpGet("contracts")]
//    public async Task<ActionResult<List<ContractDto>>> GetContracts(int employeeId)
//    {
//        var result = await _mediator.Send(new GetEmployeeContractsQuery(employeeId));
//        return Ok(result);
//    }

//    [HttpPost("contracts")]
//    public async Task<ActionResult<int>> AddContract(int employeeId, [FromBody] CreateContractDto dto)
//    {
//        if (employeeId != dto.EmployeeId)
//            return BadRequest("Employee ID Mismatch");

//        var result = await _mediator.Send(new CreateContractCommand(dto));
//        return Ok(result);
//    }

//    [HttpPost("contracts/renew")]
//    public async Task<ActionResult<int>> RenewContract(int employeeId, [FromBody] RenewContractDto dto)
//    {
//        // Ideally verify contract owner here, or trust the command logic to fail if mismatch
//        var result = await _mediator.Send(new RenewContractCommand(dto));
//        return Ok(result);
//    }
//}
