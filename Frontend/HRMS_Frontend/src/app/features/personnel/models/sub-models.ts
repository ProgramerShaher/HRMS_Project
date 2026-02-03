export interface Qualification {
    qualificationId?: number;
    degreeType: string;
    majorAr: string;
    universityAr: string;
    countryId: number;
    graduationYear: number;
    grade: string;
}

export interface Experience {
    experienceId?: number;
    companyNameAr: string;
    jobTitleAr: string;
    startDate: Date | string;
    endDate?: Date | string;
    isCurrent: number | boolean; // API shows 0, likely mapped to boolean or number
    responsibilities?: string;
    reasonForLeaving?: string;
}

export interface EmergencyContact {
    contactId?: number;
    contactNameAr: string;
    relationship: string;
    phonePrimary: string;
    phoneSecondary?: string;
    isPrimary: number | boolean;
}

export interface Contract {
    contractId?: number;
    employeeId?: number;
    contractType: string;
    startDate: Date | string;
    endDate?: Date | string;
    isRenewable: boolean;
    basicSalary: number;
    housingAllowance: number;
    transportAllowance: number;
    otherAllowances: number;
    vacationDays: number;
    workingHoursDaily: number;
}

export interface Certification {
    certificationId?: number;
    certificationName: string;
    issuingOrganization: string;
    issueDate: Date | string;
    expiryDate?: Date | string;
    credentialId?: string;
    credentialUrl?: string;
}

export interface BankAccount {
    employeeBankAccountId?: number;
    bankId: number;
    accountHolderName: string;
    ibanNumber: string;
    isPrimary: boolean;
}

export interface Dependent {
    dependentId?: number;
    fullNameAr: string;
    fullNameEn: string;
    relationship: string;
    birthDate: Date | string;
    nationalId: string;
    gender: string;
}

export interface Address {
    addressId?: number;
    addressType: string; // e.g., 'National', 'Current'
    cityId: number;
    street: string;
    buildingNumber?: string;
    zipCode?: string;
    additionalDetails?: string;
}

export interface EmployeeDocument {
    documentId?: number;
    documentTypeName: string;
    documentNumber: string;
    expiryDate?: Date | string;
    filePath?: string;
    fileName?: string;
    documentTypeId: number;
    file?: File; // For upload logic
}
