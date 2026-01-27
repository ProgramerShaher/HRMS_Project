using HRMS.Application.DTOs.Core;
using HRMS.Application.DTOs.Personnel;
using MediatR;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetAllEmployees;

public class GetAllEmployeesQuery : IRequest<PagedResult<EmployeeListDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; } // Name, Number, Mobile
    public int? DepartmentId { get; set; }
    public int? JobId { get; set; }
    public bool? IsActive { get; set; }
}
