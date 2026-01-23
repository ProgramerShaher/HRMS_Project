using HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee;
using HRMS.Application.Features.Personnel.Employees.DTOs;
using HRMS.Application.Features.Personnel.Employees.Queries.GetAllEmployees;
using HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.API.Controllers.Personnel
{
    [Route("api/personnel/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeDto>>> GetAll()
        {
            var employees = await _mediator.Send(new GetAllEmployeesQuery());
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            var employee = await _mediator.Send(new GetEmployeeByIdQuery(id));
            
            if (employee == null)
                return NotFound();
            
            return Ok(employee);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateEmployeeDto dto)
        {
            var command = new CreateEmployeeCommand
            {
                EmployeeNumber = dto.EmployeeNumber,
                FirstNameAr = dto.FirstNameAr,
                SecondNameAr = dto.SecondNameAr,
                ThirdNameAr = dto.ThirdNameAr,
                HijriLastNameAr = dto.HijriLastNameAr,
                FullNameEn = dto.FullNameEn,
                Gender = dto.Gender,
                BirthDate = dto.BirthDate,
                MaritalStatus = dto.MaritalStatus,
                NationalityId = dto.NationalityId,
                JobId = dto.JobId,
                DeptId = dto.DeptId,
                ManagerId = dto.ManagerId,
                JoiningDate = dto.JoiningDate,
                Email = dto.Email,
                Mobile = dto.Mobile
            };

            var employeeId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = employeeId }, employeeId);
        }
    }
}
