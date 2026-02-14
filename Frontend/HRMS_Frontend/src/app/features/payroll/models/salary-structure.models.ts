export interface SalaryStructureSummary {
  employeeId: number;
  employeeCode: string;
  employeeNameAr: string;
  departmentName: string;
  jobTitle: string;
  basicSalary: number;
  allowancesCount: number;
  totalAllowances: number;
  deductionsCount: number;
  totalDeductions: number;
  grossSalary: number;
  netSalary: number;
  hasStructure: boolean;
  lastUpdated: string | null;
}

export interface EmployeeSalaryStructure {
  employeeId: number;
  employeeCode: string;
  employeeNameAr: string;
  departmentName: string;
  jobTitleAr: string;
  gradeNameAr: string;
  elements: EmployeeStructureItem[];
  totalEarnings: number;
  totalDeductions: number;
  netSalary: number;
}

export interface EmployeeStructureItem {
  structureId: number;
  elementId: number;
  elementNameAr: string;
  elementType: 'ALLOWANCE' | 'DEDUCTION';
  amount: number;
  percentage: number;
}

export interface SetEmployeeSalaryStructureRequest {
  employeeId: number;
  items: StructureItemInput[];
}

export interface StructureItemInput {
  elementId: number;
  amount: number;
  percentage: number;
}

export interface InitializeSalaryFromGradeRequest {
  employeeId: number;
}

export interface SalaryBreakdown {
  basicSalary: number;
  allowances: EmployeeStructureItem[];
  deductions: EmployeeStructureItem[];
  totalEarnings: number;
  totalDeductions: number;
  netSalary: number;
}
