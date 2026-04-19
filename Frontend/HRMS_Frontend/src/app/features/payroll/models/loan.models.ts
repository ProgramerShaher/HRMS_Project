/**
 * نماذج بيانات السلف (Loans)
 * Loan Data Models
 */

export interface Loan {
  loanId: number;
  employeeId: number;
  employeeName?: string;
  loanAmount: number;
  requestDate: Date | string;
  installmentCount: number;
  status: LoanStatus;
  approvalDate?: Date | string;
  approvedBy?: number;
  approvedByName?: string;
  settlementDate?: Date | string;
  settlementNotes?: string;
  installments?: LoanInstallment[];
  remainingAmount: number;
  paidAmount: number;
}

export interface LoanInstallment {
  installmentId: number;
  loanId: number;
  employeeId: number;
  employeeName?: string;
  totalLoanAmount: number;
  installmentNumber: number;
  installmentAmount: number;
  dueDate: Date | string;
  amount: number;
  status: InstallmentStatus;
  isPaid: boolean;
  paidDate?: Date | string;
  paidInPayrollRun?: number;
  settlementNotes?: string;
}

export interface CreateLoanRequest {
  employeeId: number;
  loanAmount: number;
  installmentCount: number;
  startDate: Date | string;
}

export interface ChangeLoanStatusRequest {
  loanId: number;
  newStatus: string;
}

export interface EarlySettlementRequest {
  loanId: number;
  settlementNotes?: string;
}

export type LoanStatus = 'PENDING' | 'ACTIVE' | 'SETTLED' | 'CLOSED';
export type InstallmentStatus = 'UNPAID' | 'PAID' | 'SETTLED_MANUALLY';

export interface LoanSummary {
  totalLoans: number;
  activeLoans: number;
  totalAmount: number;
  totalRemaining: number;
  monthlyDeduction: number;
}
