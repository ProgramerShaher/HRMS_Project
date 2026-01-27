using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Departments.Queries.GetAllDepartments;

/// <summary>
/// استعلام للحصول على قائمة الأقسام
/// </summary>
public class GetAllDepartmentsQuery : IRequest<PagedResult<DepartmentDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
