using HRMS.Application.Features.Personnel.Contracts.Commands.SyncExistingContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Personnel;

[Route("api/[controller]")]
[ApiController]
public class ContractsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContractsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// مزامنة جميع العقود النشطة الموجودة مع هيكل الراتب
    /// </summary>
    /// <remarks>
    /// يستخدم هذا الـ endpoint لمرة واحدة لمزامنة العقود القديمة
    /// </remarks>
    [HttpPost("sync-existing")]
    public async Task<IActionResult> SyncExistingContracts()
    {
        var result = await _mediator.Send(new SyncExistingContractsCommand());
        
        if (result.Succeeded)
            return Ok(result);
        
        return BadRequest(result);
    }
}
