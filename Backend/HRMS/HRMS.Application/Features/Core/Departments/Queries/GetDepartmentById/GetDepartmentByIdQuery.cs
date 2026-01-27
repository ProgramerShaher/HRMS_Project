using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Departments.Queries.GetDepartmentById;

public class GetDepartmentByIdQuery : IRequest<DepartmentDto?>
{
    public int DeptId { get; set; }

    public GetDepartmentByIdQuery(int deptId)
    {
        DeptId = deptId;
    }
}
