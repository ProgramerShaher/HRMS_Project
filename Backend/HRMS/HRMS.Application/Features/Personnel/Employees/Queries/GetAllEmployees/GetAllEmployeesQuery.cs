using HRMS.Application.Features.Personnel.Employees.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetAllEmployees
{
    public class GetAllEmployeesQuery : IRequest<List<EmployeeDto>>
    {
    }
}
