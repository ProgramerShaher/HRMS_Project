/**
 * نماذج بيانات معالجة الرواتب
 * Payroll Processing Data Models
 */

export interface MonthlySalaryCalculation {
  employeeId: number;
  employeeName: string;
  basicSalary: number;
  totalAllowances: number;
  totalStructureDeductions: number;
  loanDeductions: number;
  paidInstallmentIds: number[];
  attendancePenalties: number;
  absenceDays: number;
  totalLateMinutes: number;
  totalOvertimeMinutes: number;
  overtimeEarnings: number;
  warnings: string[];
  totalViolations: number;
  otherDeductions: number;
  netSalary: number;
  details: SalaryDetailItem[];
}

export interface SalaryDetailItem {
  nameAr: string;
  nameEn: string;
  amount: number;
  type: 'ALLOWANCE' | 'DEDUCTION';
  reference: string;
  elementId?: number;
}

export interface PayrollRun {
  runId: number;
  year: number;
  month: number;
  processDate: string;
  status: PayrollRunStatus;
  employeeCount: number;
  totalGross: number;
  totalNet: number;
  processedBy?: string | null;
  approvedDate?: string | null;
  approvedBy?: string | null;
}

export interface ProcessPayrunRequest {
  month: number;
  year: number;
}

export interface RollbackPayrollRequest {
  runId: number;
}

export interface PostPayrollToGLRequest {
  runId: number;
}

export type PayrollRunStatus = 'DRAFT' | 'COMPLETED' | 'APPROVED' | 'POSTED';

export interface Payslip {
  payslipId: number;
  runId: number;
  employeeId: number;
  employeeName?: string;
  month?: number;
  year?: number;
  basicSalary: number;
  totalAllowances: number;
  totalDeductions: number;
  netSalary: number;
  totalViolations: number;
  otherDeductions: number;
  totalLateMinutes: number;
  absenceDays: number;
  totalOvertimeMinutes: number;
  overtimeEarnings: number;
  details: PayslipDetail[];
}

export interface PayslipDetail {
  detailId: number;
  payslipId: number;
  elementId?: number;
  elementNameAr: string;
  amount: number;
  type: 'ALLOWANCE' | 'DEDUCTION';
}

export interface BankFileRecord {
  employeeNameAr: string;
  employeeNameEn: string;
  accountNumber: string;
  iban?: string;
  bankName: string;
  netSalary: number;
  currency: string;
  paymentReference: string;
}

export interface PayrollDashboardStats {
  currentMonthNetSalary: number;
  totalDeductions: number;
  activeLoansCount: number;
  pendingInstallments: number;
  lastPayslip?: Payslip;
  upcomingInstallments: number;
  totalEarnings: number;
  totalAllowances: number;
}
