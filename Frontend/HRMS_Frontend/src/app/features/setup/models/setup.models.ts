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
    gradeCode: string; // Added field
    gradeLevel: number; // Added field
    minSalary?: number;
    maxSalary?: number;
    isActive: boolean;
}

export interface CreateJobGradeCommand {
    gradeNameAr: string;
    gradeNameEn: string;
    gradeCode: string; // Added field
    gradeLevel: number; // Added field
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

// Jobs DTOs
export interface Job {
    jobId: number;
    jobTitleAr: string;
    jobTitleEn?: string | null;
    jobCode?: string | null;
    description?: string | null;
    jobGradeId?: number | null;
    gradeNameAr?: string | null;
}

// Document Types DTOs
export interface DocumentType {
    documentTypeId: number;
    documentTypeNameAr: string;
    documentTypeNameEn?: string | null;
    description?: string | null;
    allowedExtensions?: string | null;
    isRequired: boolean;
    hasExpiry: boolean;
    defaultExpiryDays?: number | null;
    maxFileSizeMB?: number | null;
}

export interface CreateDocumentTypeCommand {
    documentTypeNameAr: string;
    documentTypeNameEn?: string | null;
    description?: string | null;
    allowedExtensions?: string | null;
    isRequired: boolean;
    hasExpiry: boolean;
    defaultExpiryDays?: number | null;
    maxFileSizeMB?: number | null;
}

// Attendance Shift Types DTOs
export interface ShiftType {
    shiftId: number;
    shiftNameAr: string;
    startTime: string; // HH:mm
    endTime: string;   // HH:mm
    hoursCount: number;
    isCrossDay: number; // 0 or 1
    gracePeriodMins?: number;
}

export interface CreateShiftTypeCommand {
    shiftNameAr: string;
    startTime: string;
    endTime: string;
    isCrossDay: number;
}

// Attendance Policy DTOs
export interface AttendancePolicy {
    policyId: number;
    policyNameAr: string;
    deptId?: number;
    jobId?: number;
    lateGraceMins: number;
    overtimeMultiplier: number;
    weekendOtMultiplier: number;
    departmentName?: string;
    jobTitle?: string;
}

export interface CreateAttendancePolicyCommand {
    policyNameAr: string;
    deptId?: number;
    jobId?: number;
    lateGraceMins: number;
    overtimeMultiplier: number;
    weekendOtMultiplier: number;
}

// Leave Types DTOs
export interface LeaveType {
    leaveTypeId: number;
    leaveTypeNameAr: string;
    nameEn?: string | null;
    defaultDays: number;
    isDeductible: number; // 0 or 1
    requiresAttachment: number; // 0 or 1
}

export interface CreateLeaveTypeCommand {
    leaveTypeNameAr: string;
    defaultDays: number;
    isDeductible: number;
    requiresAttachment: number;
}

// Public Holidays DTOs
export interface PublicHoliday {
    holidayId: number;
    holidayNameAr: string;
    holidayNameEn?: string | null;
    startDate: string; // Date string
    endDate: string;   // Date string
    daysCount: number;
}

export interface CreatePublicHolidayCommand {
    holidayNameAr: string;
    holidayNameEn?: string | null;
    startDate: string;
    endDate: string;
}

// Payroll Elements DTOs
export interface PayrollElement {
    elementId: number;
    elementNameAr: string;
    elementType: string; // 'EARNING' or 'DEDUCTION'
    isTaxable: boolean;
    isGosiBase: boolean;
    isRecurring: boolean;
    isBasic: boolean;
    defaultPercentage: number;
}

export interface CreatePayrollElementCommand {
    elementNameAr: string;
    elementType: string;
    isTaxable: boolean;
    isGosiBase: boolean;
    isRecurring: boolean;
    isBasic: boolean;
    defaultPercentage: number;
}
