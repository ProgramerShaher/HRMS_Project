using HRMS.Application.Features.Core.Branches.Commands.CreateBranch;
using HRMS.Application.Features.Core.Branches.Commands.UpdateBranch;
using HRMS.Application.Features.Core.Branches.Queries.GetAllBranches;
using HRMS.Application.Features.Core.Branches.Queries.GetBranchById;
using HRMS.Core.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BranchesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Branch>>> GetAll()
        {
            var branches = await _mediator.Send(new GetAllBranchesQuery());
            return Ok(branches);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Branch>> GetById(int id)
        {
            var branch = await _mediator.Send(new GetBranchByIdQuery(id));
            if (branch == null) return NotFound();
            return Ok(branch);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateBranchCommand command)
        {
            var branchId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = branchId }, branchId);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateBranchCommand command)
        {
            if (id != command.BranchId)
                return BadRequest();

            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            
            return NoContent();
        }
    }
}
