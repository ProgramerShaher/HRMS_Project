/**
 * Sub-models for Employee DTOs
 * These represent the nested entities within an employee record
 */

export interface Qualification {
  qualificationId?: number;
  employeeId?: number;
  degreeType: string;
  majorAr: string;
  universityAr: string;
  countryId?: number;
  graduationYear: number;
  grade?: string;
  attachmentPath?: string;
}

export interface Certification {
  certificationId?: number;
  employeeId?: number;
  certificationName: string;
  issuingOrganization: string;
  issueDate: Date | string;
  expiryDate?: Date | string;
  credentialId?: string;
  credentialUrl?: string;
  attachmentPath?: string;
}

export interface Experience {
  experienceId?: number;
  employeeId?: number;
  companyNameAr: string;
  jobTitleAr: string;
  startDate: Date | string;
  endDate?: Date | string;
  responsibilities?: string;
  reasonForLeaving?: string;
  isCurrent: number | boolean;
}

export interface EmergencyContact {
  contactId?: number;
  employeeId?: number;
  contactNameAr: string;
  relationship: string;
  phonePrimary: string;
  phoneSecondary?: string;
  isPrimary?: number | boolean;
}

export interface Dependent {
  dependentId?: number;
  employeeId?: number;
  fullNameAr: string;
  fullNameEn?: string;
  relationship: string; // Spouse, Son, Daughter
  birthDate: Date | string;
  nationalId?: string;
  gender: string;
}

export interface BankAccount {
  employeeBankAccountId?: number;
  employeeId?: number;
  bankId: number;
  accountHolderName: string;
  ibanNumber: string;
  isPrimary: boolean | number;
}

export interface Address {
  addressId?: number;
  employeeId?: number;
  addressType: string; // Permanent, Temporary, Work
  street: string;
  cityId?: number;
  countryId?: number;
  buildingNumber?: string;
  zipCode?: string;
  additionalDetails?: string;
}

export interface EmployeeDocument {
  documentId?: number;
  employeeId?: number;
  documentTypeId: number;
  documentNumber?: string;
  expiryDate?: Date | string;
  filePath?: string;
  fileName?: string;
}

export interface Contract {
  contractId?: number;
  employeeId?: number;
  contractType: string; // Initial, Renewal, Amendment
  startDate: Date | string;
  endDate: Date | string;
  isRenewable: boolean;
  basicSalary: number;
  housingAllowance?: number;
  transportAllowance?: number;
  otherAllowances?: number;
  workingHoursDaily?: number;
  vacationDays?: number;
  contractStatus?: string;
  filePath?: string;
}
