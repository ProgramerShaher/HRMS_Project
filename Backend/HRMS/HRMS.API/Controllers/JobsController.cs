using HRMS.Application.Features.Core.Jobs.Commands.CreateJob;
using HRMS.Application.Features.Core.Jobs.Queries.GetAllJobs;
using HRMS.Core.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public JobsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<Job>>> GetAll()
        {
            return Ok(await _mediator.Send(new GetAllJobsQuery()));
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateJobCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id }, id);
        }
    }
}
