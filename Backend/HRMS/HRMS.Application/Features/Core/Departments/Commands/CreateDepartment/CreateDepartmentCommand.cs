using MediatR;

namespace HRMS.Application.Features.Core.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<int>
    {
        public string DeptNameAr { get; set; }
        public string DeptNameEn { get; set; }
        public int? ParentDeptId { get; set; }
        public int? ManagerId { get; set; }
    }
}
