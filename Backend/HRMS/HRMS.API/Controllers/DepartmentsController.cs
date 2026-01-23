using HRMS.Application.Features.Core.Departments.Commands.CreateDepartment;
using HRMS.Application.Features.Core.Departments.Queries.GetAllDepartments;
using HRMS.Core.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DepartmentsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<Department>>> GetAll()
        {
            return Ok(await _mediator.Send(new GetAllDepartmentsQuery()));
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateDepartmentCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id }, id);
        }
    }
}
