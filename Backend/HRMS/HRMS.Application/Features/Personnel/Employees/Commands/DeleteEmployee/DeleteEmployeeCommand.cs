using MediatR;

namespace HRMS.Application.Features.Personnel.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommand : IRequest<bool>
{
    public int EmployeeId { get; set; }

    public DeleteEmployeeCommand(int id)
    {
        EmployeeId = id;
    }
}
