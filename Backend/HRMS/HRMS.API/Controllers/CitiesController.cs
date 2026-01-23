using HRMS.Application.Features.Core.Cities.Commands.CreateCity;
using HRMS.Application.Features.Core.Cities.Queries.GetAllCities;
using HRMS.Core.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CitiesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<City>>> GetAll()
        {
            return Ok(await _mediator.Send(new GetAllCitiesQuery()));
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateCityCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id }, id);
        }
    }
}
