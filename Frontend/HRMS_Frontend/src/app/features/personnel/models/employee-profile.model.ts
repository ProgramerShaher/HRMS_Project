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

/**
 * Core employee profile information
 */
export interface CoreProfile {
    employeeId: number;
    employeeNumber: string;
    
    // Names
    firstNameAr?: string;
    secondNameAr?: string;
    thirdNameAr?: string;
    hijriLastNameAr?: string | null;
    fullNameAr: string;
    fullNameEn: string;
    
    // Personal
    birthDate?: Date | string;
    gender?: string;
    mobile: string;
    email?: string;
    nationalityId?: number;
    nationalityName?: string;
    nationalId?: string;
    maritalStatus?: string;
    profilePicturePath?: string;
    
    // Job
    departmentName?: string;
    deptId?: number;
    jobTitle?: string;
    jobId?: number;
    joiningDate?: Date | string;
    hireDate?: Date | string;
    status?: string | null;
    isActive?: boolean;
    managerName?: string;
}

/**
 * Compensation details
 */
export interface Compensation {
    basicSalary: number;
    totalSalary: number;
    housingAllowance: number;
    transportAllowance: number;
    medicalAllowance: number;
}

/**
 * Full employee profile with all related data
 */
export interface EmployeeProfile {
    coreProfile: CoreProfile;
    managerName?: string;
    compensation?: Compensation;
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
