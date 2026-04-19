/**
 * نماذج بيانات عناصر الراتب
 * Salary Element Data Models
 */

export interface SalaryElement {
  elementId: number;
  elementNameAr: string;
  elementType: ElementType;
  isTaxable: boolean;
  isGosiBase: boolean;
  isRecurring: boolean;
  isBasic: boolean;
  defaultPercentage: number;
}

export interface CreateSalaryElementRequest {
  elementNameAr: string;
  elementType: ElementType;
  isTaxable: boolean;
  isGosiBase: boolean;
  isRecurring: boolean;
  isBasic: boolean;
  defaultPercentage: number;
}

export interface UpdateSalaryElementRequest extends CreateSalaryElementRequest {
  elementId: number;
}

export type ElementType = 'ALLOWANCE' | 'DEDUCTION';

export interface SalaryElementSummary {
  totalElements: number;
  earningsCount: number;
  deductionsCount: number;
  hasBasicElement: boolean;
}
