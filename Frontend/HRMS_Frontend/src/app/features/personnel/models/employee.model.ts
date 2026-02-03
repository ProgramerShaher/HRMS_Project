export interface Employee {
    employeeId: number;
    employeeNumber: string;
    fullNameAr: string;
    fullNameEn: string;
    departmentName: string;
    jobTitle: string;
    mobile: string;
    hireDate: Date | string;
    isActive: boolean;
    // Optional detailed fields for list view if needed
    compensation?: {
        basicSalary: number;
        totalSalary: number;
        housingAllowance: number;
        transportAllowance: number;
        medicalAllowance: number;
    };
}
