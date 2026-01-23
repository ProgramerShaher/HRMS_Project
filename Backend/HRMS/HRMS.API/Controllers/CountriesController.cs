using HRMS.Application.Features.Core.Countries.Commands.CreateCountry;
using HRMS.Application.Features.Core.Countries.Queries.GetAllCountries;
using HRMS.Application.Features.Core.Countries.Queries.GetCountryById;
using HRMS.Core.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CountriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Country>>> GetAll()
        {
            var countries = await _mediator.Send(new GetAllCountriesQuery());
            return Ok(countries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetById(int id)
        {
            var country = await _mediator.Send(new GetCountryByIdQuery(id));
            if (country == null) return NotFound();
            return Ok(country);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateCountryCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }
    }
}
