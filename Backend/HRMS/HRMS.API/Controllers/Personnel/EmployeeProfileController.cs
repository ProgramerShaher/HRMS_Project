using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Addresses;
using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.BankAccounts;
using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Certifications;
using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Dependents;
using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.EmergencyContacts;
using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Experiences;
using HRMS.Application.Features.Personnel.EmployeeDetails.Commands.Qualifications;
using HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Addresses;
using HRMS.Application.Features.Personnel.EmployeeDetails.Queries.BankAccounts;
using HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Certifications;
using HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Dependents;
using HRMS.Application.Features.Personnel.EmployeeDetails.Queries.EmergencyContacts;
using HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Experiences;
using HRMS.Application.Features.Personnel.EmployeeDetails.Queries.Qualifications;
using HRMS.Application.Features.Personnel.Employees.Commands.DeleteEmployeeDocument;
using HRMS.Application.Features.Personnel.Employees.Commands.UploadEmployeeDocument;
using HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeDocuments;
using HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeFullProfile;
using HRMS.Application.Features.Personnel.Contracts.Commands.CreateContract;
using HRMS.Application.Features.Personnel.Contracts.Commands.RenewContract;
using HRMS.Application.Features.Personnel.Contracts.Queries.GetEmployeeContracts;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HRMS.Application.Features.Personnel.Employees.DTOs;

namespace HRMS.API.Controllers.Personnel;

/// <summary>
/// إدارة الملف الشخصي للموظف (المؤهلات، الخبرات، المستندات، العائلة، إلخ)
/// Employee Profile Details & Sub-Entities Management
/// </summary>
[Route("api/employee-profile/{employeeId}")]
[ApiController]
public class EmployeeProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeeProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ----------------------------------------------------------------------------------
    // Full Profile
    // ----------------------------------------------------------------------------------

    /// <summary>
    /// جلب الملف الشخصي الكامل للموظف
    /// Get Full Profile (All details)
    /// </summary>
    [HttpGet("full-profile")]
    public async Task<ActionResult<Result<DetailedEmployeeProfileDto>>> GetFullProfile(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeFullProfileQuery(employeeId));
        // Note: Query returns DTO directly, assume we wrap it or strict API requires Result<T>. 
        // If Query returns DTO, we wrap it here.
        return Ok(Result<DetailedEmployeeProfileDto>.Success(result));
    }

    // ----------------------------------------------------------------------------------
    // Qualifications
    // ----------------------------------------------------------------------------------

    [HttpGet("qualifications")]
    public async Task<ActionResult<Result<List<EmployeeQualificationDto>>>> GetQualifications(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeQualificationsQuery(employeeId));
        return Ok(Result<List<EmployeeQualificationDto>>.Success(result));
    }

    [HttpPost("qualifications")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<int>>> AddQualification(int employeeId, [FromForm] AddQualificationWithAttachmentCommand command)
    {
        command.EmployeeId = employeeId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("qualifications/{id}")]
    public async Task<ActionResult<Result<bool>>> UpdateQualification(int employeeId, int id, [FromBody] EmployeeQualificationDto dto)
    {
        // Note: If using FromForm for updates with file, need separate Command.
        // Assuming JSON update for data only for now as requested "PUT (FromForm)" was distinct.
        // Remapping DTO to Command logic inside Handler or using specific UpdateCommand.
        var command = new UpdateQualificationCommand(employeeId, id, dto);
        await _mediator.Send(command);
        return Ok(Result<bool>.Success(true));
    }

    [HttpDelete("qualifications/{id}")]
    public async Task<ActionResult<Result<bool>>> DeleteQualification(int employeeId, int id)
    {
        await _mediator.Send(new DeleteQualificationCommand(employeeId, id));
        return Ok(Result<bool>.Success(true));
    }

    // ----------------------------------------------------------------------------------
    // Certifications
    // ----------------------------------------------------------------------------------

    [HttpGet("certifications")]
    public async Task<ActionResult<Result<List<EmployeeCertificationDto>>>> GetCertifications(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeCertificationsQuery { EmployeeId = employeeId });
        return Ok(result);
    }

    [HttpPost("certifications")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<int>>> AddCertification(int employeeId, [FromForm] AddCertificationCommand command)
    {
        command.EmployeeId = employeeId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("certifications/{id}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<bool>>> UpdateCertification(int employeeId, int id, [FromForm] UpdateCertificationCommand command)
    {
        command.EmployeeId = employeeId;
        command.CertificationId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("certifications/{id}")]
    public async Task<ActionResult<Result<bool>>> DeleteCertification(int employeeId, int id)
    {
        var result = await _mediator.Send(new DeleteCertificationCommand { EmployeeId = employeeId, CertificationId = id });
        return Ok(result);
    }

    // ----------------------------------------------------------------------------------
    // Documents
    // ----------------------------------------------------------------------------------

    [HttpGet("documents")]
    public async Task<ActionResult<Result<List<EmployeeDocumentDto>>>> GetDocuments(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeDocumentsQuery { EmployeeId = employeeId });
        return Ok(result);
    }

    [HttpPost("documents")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<int>>> AddDocument(int employeeId, [FromForm] UploadEmployeeDocumentCommand command)
    {
        command.EmployeeId = employeeId;
        var result = await _mediator.Send(command);
        return Ok(Result<int>.Success(result));
    }

    [HttpDelete("documents/{id}")]
    public async Task<ActionResult<Result<bool>>> DeleteDocument(int employeeId, int id)
    {
        var result = await _mediator.Send(new DeleteEmployeeDocumentCommand { EmployeeId = employeeId, DocumentId = id });
        return Ok(result);
    }

    // ----------------------------------------------------------------------------------
    // Experiences
    // ----------------------------------------------------------------------------------

    [HttpGet("experiences")]
    public async Task<ActionResult<Result<List<EmployeeExperienceDto>>>> GetExperiences(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeExperiencesQuery(employeeId));
        return Ok(Result<List<EmployeeExperienceDto>>.Success(result));
    }

    [HttpPost("experiences")]
    public async Task<ActionResult<Result<int>>> AddExperience(int employeeId, [FromBody] EmployeeExperienceDto dto)
    {
        var result = await _mediator.Send(new AddExperienceCommand(employeeId, dto));
        return Ok(Result<int>.Success(result));
    }

    [HttpPut("experiences/{id}")]
    public async Task<ActionResult<Result<bool>>> UpdateExperience(int employeeId, int id, [FromBody] EmployeeExperienceDto dto)
    {
        await _mediator.Send(new UpdateExperienceCommand(employeeId, id, dto));
        return Ok(Result<bool>.Success(true));
    }

    [HttpDelete("experiences/{id}")]
    public async Task<ActionResult<Result<bool>>> DeleteExperience(int employeeId, int id)
    {
        await _mediator.Send(new DeleteExperienceCommand(employeeId, id));
        return Ok(Result<bool>.Success(true));
    }

    // ----------------------------------------------------------------------------------
    // Emergency Contacts
    // ----------------------------------------------------------------------------------

    [HttpGet("emergency-contacts")]
    public async Task<ActionResult<Result<List<EmergencyContactDto>>>> GetEmergencyContacts(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeEmergencyContactsQuery(employeeId));
        return Ok(Result<List<EmergencyContactDto>>.Success(result));
    }

    [HttpPost("emergency-contacts")]
    public async Task<ActionResult<Result<int>>> AddEmergencyContact(int employeeId, [FromBody] EmergencyContactDto dto)
    {
        var result = await _mediator.Send(new AddEmergencyContactCommand(employeeId, dto));
        return Ok(Result<int>.Success(result));
    }

    [HttpPut("emergency-contacts/{id}")]
    public async Task<ActionResult<Result<bool>>> UpdateEmergencyContact(int employeeId, int id, [FromBody] EmergencyContactDto dto)
    {
        await _mediator.Send(new UpdateEmergencyContactCommand(employeeId, id, dto));
        return Ok(Result<bool>.Success(true));
    }

    [HttpDelete("emergency-contacts/{id}")]
    public async Task<ActionResult<Result<bool>>> DeleteEmergencyContact(int employeeId, int id)
    {
        await _mediator.Send(new DeleteEmergencyContactCommand(employeeId, id));
        return Ok(Result<bool>.Success(true));
    }

    // ----------------------------------------------------------------------------------
    // Dependents
    // ----------------------------------------------------------------------------------

    [HttpGet("dependents")]
    public async Task<ActionResult<Result<List<DependentDto>>>> GetDependents(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeDependentsQuery { EmployeeId = employeeId });
        return Ok(result);
    }

    [HttpPost("dependents")]
    public async Task<ActionResult<Result<int>>> AddDependent(int employeeId, [FromBody] AddDependentCommand command)
    {
        command.EmployeeId = employeeId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("dependents/{id}")]
    public async Task<ActionResult<Result<bool>>> UpdateDependent(int employeeId, int id, [FromBody] UpdateDependentCommand command)
    {
        command.EmployeeId = employeeId;
        command.DependentId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("dependents/{id}")]
    public async Task<ActionResult<Result<bool>>> DeleteDependent(int employeeId, int id)
    {
        var result = await _mediator.Send(new DeleteDependentCommand { EmployeeId = employeeId, DependentId = id });
        return Ok(result);
    }

    // ----------------------------------------------------------------------------------
    // Bank Accounts
    // ----------------------------------------------------------------------------------

    [HttpGet("bank-accounts")]
    public async Task<ActionResult<Result<List<EmployeeBankAccountDto>>>> GetBankAccounts(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeBankAccountsQuery { EmployeeId = employeeId });
        return Ok(result);
    }

    [HttpPost("bank-accounts")]
    public async Task<ActionResult<Result<int>>> AddBankAccount(int employeeId, [FromBody] AddBankAccountCommand command)
    {
        command.EmployeeId = employeeId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("bank-accounts/{id}")]
    public async Task<ActionResult<Result<bool>>> UpdateBankAccount(int employeeId, int id, [FromBody] UpdateBankAccountCommand command)
    {
        command.EmployeeId = employeeId;
        command.AccountId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("bank-accounts/{id}")]
    public async Task<ActionResult<Result<bool>>> DeleteBankAccount(int employeeId, int id)
    {
        var result = await _mediator.Send(new DeleteBankAccountCommand { EmployeeId = employeeId, AccountId = id });
        return Ok(result);
    }

    // ----------------------------------------------------------------------------------
    // Addresses
    // ----------------------------------------------------------------------------------

    [HttpGet("addresses")]
    public async Task<ActionResult<Result<List<EmployeeAddressDto>>>> GetAddresses(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeAddressesQuery { EmployeeId = employeeId });
        return Ok(result);
    }

    [HttpPost("addresses")]
    public async Task<ActionResult<Result<int>>> AddAddress(int employeeId, [FromBody] AddAddressCommand command)
    {
        command.EmployeeId = employeeId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("addresses/{id}")]
    public async Task<ActionResult<Result<bool>>> UpdateAddress(int employeeId, int id, [FromBody] UpdateAddressCommand command)
    {
        command.EmployeeId = employeeId;
        command.AddressId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("addresses/{id}")]
    public async Task<ActionResult<Result<bool>>> DeleteAddress(int employeeId, int id)
    {
        var result = await _mediator.Send(new DeleteAddressCommand { EmployeeId = employeeId, AddressId = id });
        return Ok(result);
    }

    // ----------------------------------------------------------------------------------
    // Contracts
    // ----------------------------------------------------------------------------------

    [HttpGet("contracts")]
    public async Task<ActionResult<Result<List<ContractDto>>>> GetContracts(int employeeId)
    {
        var result = await _mediator.Send(new GetEmployeeContractsQuery(employeeId));
        return Ok(Result<List<ContractDto>>.Success(result));
    }

    [HttpPost("contracts")]
    public async Task<ActionResult<Result<int>>> AddContract(int employeeId, [FromBody] CreateContractDto dto)
    {
        if (employeeId != dto.EmployeeId)
            return BadRequest("Employee ID mismatch");
            
        var result = await _mediator.Send(new CreateContractCommand(dto));
        return Ok(Result<int>.Success(result));
    }

    [HttpPut("contracts/renew")]
    public async Task<ActionResult<Result<int>>> RenewContract(int employeeId, [FromBody] RenewContractDto dto)
    {
        // Assuming RenewContract logic handles security check or redundant
       var result = await _mediator.Send(new RenewContractCommand(dto));
       return Ok(Result<int>.Success(result));
    }
}
