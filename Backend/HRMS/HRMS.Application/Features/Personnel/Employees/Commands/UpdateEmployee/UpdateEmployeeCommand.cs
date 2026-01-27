using MediatR;
using HRMS.Application.DTOs.Personnel;

namespace HRMS.Application.Features.Personnel.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommand : IRequest<int>
{
    public int EmployeeId { get; set; }
    public CreateEmployeeDto Data { get; set; } // Reusing DTO for full update (ERP style often allows full edit)

    public UpdateEmployeeCommand(int id, CreateEmployeeDto data)
    {
        EmployeeId = id;
        Data = data;
    }
}
