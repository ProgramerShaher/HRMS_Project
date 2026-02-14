export interface Employee {
  employeeId: number;
  employeeCode: string;
  fullNameAr: string;
  fullNameEn?: string;
  jobTitle?: string;
  departmentName?: string;
  photoUrl?: string;
  email?: string;
  phoneNumber?: string;
  isActive: boolean;
}

export interface EmployeeSearchParams {
  searchTerm?: string;
  departmentId?: number;
  isActive?: boolean;
  pageNumber?: number;
  pageSize?: number;
}
