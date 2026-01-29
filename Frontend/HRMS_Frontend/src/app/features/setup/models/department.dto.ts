// Matches HRMS.Application.DTOs.Core.DepartmentDto
export interface DepartmentDto {
  deptId: number;
  deptNameAr: string | null;
  deptNameEn: string | null;
  parentDeptId: number | null;
  parentDeptNameAr?: string | null;
  branchId: number;
  costCenterCode: string | null;
  managerId: number | null;
  isActive: number; // byte in C# -> number (0 or 1)
}

// Matches HRMS.Application.Features.Core.Departments.Commands.CreateDepartment.CreateDepartmentCommand
export interface CreateDepartmentCommand {
  deptNameAr: string;
  deptNameEn?: string | null;
  parentDeptId?: number | null;
  costCenterCode?: string | null;
  managerId?: number | null;
}

// Matches HRMS.Application.Features.Core.Departments.Commands.UpdateDepartment.UpdateDepartmentCommand
export interface UpdateDepartmentCommand {
  deptId: number;
  deptNameAr: string;
  deptNameEn?: string | null;
  parentDeptId?: number | null;
  costCenterCode?: string | null;
  managerId?: number | null;
  isActive: number;
}
