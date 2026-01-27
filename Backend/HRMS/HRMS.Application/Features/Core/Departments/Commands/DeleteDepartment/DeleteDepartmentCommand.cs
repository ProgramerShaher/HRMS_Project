using MediatR;

namespace HRMS.Application.Features.Core.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommand : IRequest<bool>
{
    public int DeptId { get; set; }

    public DeleteDepartmentCommand(int deptId)
    {
        DeptId = deptId;
    }
}
