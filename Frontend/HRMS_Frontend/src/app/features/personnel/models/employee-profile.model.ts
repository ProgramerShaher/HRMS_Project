import { 
    Address, 
    BankAccount, 
    Certification, 
    Contract, 
    Dependent, 
    EmergencyContact, 
    EmployeeDocument, 
    Experience, 
    Qualification 
} from './sub-models';

export interface EmployeeProfile {
    employeeId: number;
    employeeNumber: string;
    
    // Names
    firstNameAr?: string;
    secondNameAr?: string;
    thirdNameAr?: string;
    lastNameAr?: string;
    fullNameAr: string;
    fullNameEn: string;
    
    // Personal
    birthDate?: Date | string;
    gender?: string;
    mobile: string;
    email?: string;
    nationalityId?: number;
    nationalId?: string;
    maritalStatus?: string;
    
    // Job
    departmentName?: string;
    departmentId?: number;
    jobTitle?: string;
    jobId?: number;
    managerName?: string;
    managerId?: number;
    hireDate: Date | string;
    isActive: boolean;
    
    // Compensation
    compensation?: {
        basicSalary: number;
        totalSalary: number;
        housingAllowance: number;
        transportAllowance: number;
        medicalAllowance: number;
    };

    // Lists
    qualifications?: Qualification[];
    experiences?: Experience[];
    emergencyContacts?: EmergencyContact[];
    contracts?: Contract[];
    certifications?: Certification[];
    bankAccounts?: BankAccount[];
    dependents?: Dependent[];
    addresses?: Address[];
    documents?: EmployeeDocument[];
}
