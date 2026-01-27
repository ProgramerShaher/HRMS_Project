using MediatR;
using HRMS.Application.DTOs.Personnel;

namespace HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommand : IRequest<int>
{
    public CreateEmployeeDto Data { get; set; }

    public CreateEmployeeCommand(CreateEmployeeDto data)
    {
        Data = data;
    }
}
