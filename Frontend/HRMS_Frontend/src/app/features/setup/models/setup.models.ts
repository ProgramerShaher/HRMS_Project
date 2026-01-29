// Departments DTOs
export interface Department {
    deptId: number;
    deptNameAr: string;
    deptNameEn: string;
    parentDeptId?: number | null;
    parentDeptNameAr?: string | null;
    branchId: number;
    costCenterCode?: string | null;
    managerId?: number | null;
    isActive: boolean;
}

export interface CreateDepartmentCommand {
    deptNameAr: string;
    deptNameEn: string;
    parentDeptId?: number | null;
    costCenterCode?: string | null;
    managerId?: number | null;
}

// Countries DTOs
export interface Country {
    countryId: number;
    citizenshipNameAr: string;
    citizenshipNameEn?: string | null;
    countryNameAr: string;
    countryNameEn?: string | null;
    isoCode: string;
    isActive: boolean;
    citiesCount?: number;
}

export interface CreateCountryCommand {
    countryNameAr: string;
    countryNameEn: string;
    citizenshipNameAr: string;
    citizenshipNameEn?: string | null;
    isoCode: string;
}

// Cities DTOs
export interface City {
    cityId: number;
    cityNameAr: string;
    cityNameEn: string;
    countryId: number;
    countryNameAr?: string; // For display
    isActive: boolean;
}

export interface CreateCityCommand {
    cityNameAr: string;
    cityNameEn: string;
    countryId: number;
}

// Banks DTOs
export interface Bank {
    bankId: number;
    bankNameAr: string;
    bankNameEn: string;
    swiftCode?: string | null;
    isActive: boolean;
}

export interface CreateBankCommand {
    bankNameAr: string;
    bankNameEn: string;
    swiftCode?: string | null;
}

// JobGrades DTOs
export interface JobGrade {
    jobGradeId: number;
    gradeNameAr: string;
    gradeNameEn: string;
    minSalary?: number;
    maxSalary?: number;
    isActive: boolean;
}

export interface CreateJobGradeCommand {
    gradeNameAr: string;
    gradeNameEn: string;
    minSalary?: number;
    maxSalary?: number;
}

// Universal API Response Wrapper
export interface PaginatedResult<T> {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
}
