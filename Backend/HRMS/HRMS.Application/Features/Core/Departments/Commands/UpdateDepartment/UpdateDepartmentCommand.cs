using MediatR;

namespace HRMS.Application.Features.Core.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommand : IRequest<int>
{
    public int DeptId { get; set; }
    public string DeptNameAr { get; set; } = string.Empty;
    public string? DeptNameEn { get; set; }
    public int? ParentDeptId { get; set; }
    public string? CostCenterCode { get; set; }
    public int? ManagerId { get; set; }
    public byte IsActive { get; set; } = 1;
}
