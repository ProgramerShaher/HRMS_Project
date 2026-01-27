namespace HRMS.Application.DTOs.Core
{
    public class DepartmentDto
    {
        public int DeptId { get; set; }
        public string? DeptNameAr { get; set; }
        public string? DeptNameEn { get; set; }
        public int? ParentDeptId { get; set; }
        public string? ParentDeptNameAr { get; set; }
        public int BranchId { get; set; }
        public string? CostCenterCode { get; set; }
        public int? ManagerId { get; set; }
        public byte IsActive { get; set; }
    }

    public class CreateDepartmentDto
    {
        public required string DeptNameAr { get; set; }
        public string? DeptNameEn { get; set; }
        public int? ParentDeptId { get; set; }
        public int BranchId { get; set; }
        public string? CostCenterCode { get; set; }
        public int? ManagerId { get; set; }
        public int? IsActive { get; set; }
    }

    public class UpdateDepartmentDto
    {
        public required string DeptNameAr { get; set; }
        public string? DeptNameEn { get; set; }
        public int? ParentDeptId { get; set; }
        public int BranchId { get; set; }
        public string? CostCenterCode { get; set; }
        public int? ManagerId { get; set; }
        public int? IsActive { get; set; }
    }
}
