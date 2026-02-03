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

export interface CreateEmployeeDto {
    // Personal Info
    employeeNumber: string;
    firstNameAr: string;
    secondNameAr: string;
    thirdNameAr: string;
    lastNameAr: string;
    fullNameEn: string;
    birthDate: Date | string;
    gender: 'Male' | 'Female' | string;
    mobile: string;
    email?: string;
    nationalityId: number;
    nationalId: string;
    maritalStatus?: string; // Found in one of the POST schemas
    
    // Employment Info
    departmentId: number;
    jobId: number;
    managerId?: number;
    hireDate: Date | string;
    
    // Professional Info
    licenseNumber?: string;
    licenseExpiryDate?: Date | string;
    specialty?: string;
    
    // Financial Info (Direct fields)
    basicSalary: number;
    housingAllowance: number;
    transportAllowance: number;
    medicalAllowance: number;
    
    // Bank Info (Direct fields logic often redundant with BankAccounts array, but present in schema)
    bankId?: number;
    ibanNumber?: string;

    // Nested Lists
    qualifications: Qualification[];
    experiences: Experience[];
    emergencyContacts: EmergencyContact[];
    contracts: Contract[];
    certifications: Certification[];
    bankAccounts: BankAccount[];
    dependents: Dependent[];
    addresses: Address[];
    documents: EmployeeDocument[];
}
