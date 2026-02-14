import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response';
import {
  Loan,
  LoanInstallment,
  CreateLoanRequest,
  ChangeLoanStatusRequest,
  EarlySettlementRequest,
  LoanSummary
} from '../models/loan.models';

/**
 * خدمة إدارة السلف
 * Loan Management Service
 */
@Injectable({
  providedIn: 'root'
})
export class LoanService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Loan`;

  /**
   * إنشاء سلفة جديدة
   * Create new loan
   */
  createLoan(request: CreateLoanRequest): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(this.apiUrl, request);
  }

  /**
   * الحصول على تفاصيل سلفة
   * Get loan by ID
   */
  getLoanById(id: number): Observable<ApiResponse<Loan>> {
    return this.http.get<ApiResponse<Loan>>(`${this.apiUrl}/${id}`);
  }

  /**
   * تغيير حالة السلفة
   * Change loan status
   */
  changeLoanStatus(request: ChangeLoanStatusRequest): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/${request.loanId}/status`, request);
  }

  /**
   * الحصول على جميع السلف (للإدارة)
   * Get all loans (Admin)
   */
  getAllLoans(filters?: {
    status?: string;
    employeeId?: number;
    departmentId?: number;
    dateFrom?: string;
    dateTo?: string;
  }): Observable<ApiResponse<Loan[]>> {
    let params = new HttpParams();
    if (filters) {
      if (filters.status) params = params.set('status', filters.status);
      if (filters.employeeId) params = params.set('employeeId', filters.employeeId.toString());
      if (filters.departmentId) params = params.set('departmentId', filters.departmentId.toString());
      if (filters.dateFrom) params = params.set('dateFrom', filters.dateFrom);
      if (filters.dateTo) params = params.set('dateTo', filters.dateTo);
    }
    return this.http.get<ApiResponse<Loan[]>>(`${this.apiUrl}/all`, { params });
  }

  /**
   * تسوية مبكرة للسلفة
   * Early settlement
   */
  earlySettlement(id: number, notes: string): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/${id}/settle`, { settlementNotes: notes });
  }

  /**
   * الحصول على سلف موظف
   * Get employee loans
   */
  getEmployeeLoans(employeeId: number, status?: string): Observable<ApiResponse<Loan[]>> {
    let params = new HttpParams();
    if (status) {
      params = params.set('status', status);
    }
    return this.http.get<ApiResponse<Loan[]>>(`${this.apiUrl}/employee/${employeeId}`, { params });
  }

  /**
   * الحصول على أقساط شهر معين
   * Get monthly installments
   */
  getMonthlyInstallments(month: number, year: number, employeeId?: number): Observable<ApiResponse<LoanInstallment[]>> {
    let params = new HttpParams()
      .set('month', month.toString())
      .set('year', year.toString());
    
    if (employeeId) {
      params = params.set('employeeId', employeeId.toString());
    }
    
    return this.http.get<ApiResponse<LoanInstallment[]>>(`${this.apiUrl}/installments`, { params });
  }

  /**
   * الحصول على جدول أقساط موظف
   * Get employee installment schedule
   */
  getEmployeeSchedule(employeeId: number): Observable<ApiResponse<LoanInstallment[]>> {
    return this.http.get<ApiResponse<LoanInstallment[]>>(`${this.apiUrl}/employee-schedule/${employeeId}`);
  }

  /**
   * حساب ملخص السلف لموظف
   * Calculate loan summary for employee
   */
  getLoanSummary(employeeId: number): Observable<LoanSummary> {
    return new Observable(observer => {
      this.getEmployeeLoans(employeeId).subscribe({
        next: (response) => {
          if (response.succeeded && response.data) {
            const loans = response.data;
            const activeLoans = loans.filter(l => l.status === 'ACTIVE');
            
            const summary: LoanSummary = {
              totalLoans: loans.length,
              activeLoans: activeLoans.length,
              totalAmount: loans.reduce((sum, l) => sum + l.loanAmount, 0),
              totalRemaining: loans.reduce((sum, l) => sum + l.remainingAmount, 0),
              monthlyDeduction: activeLoans.reduce((sum, l) => {
                const unpaidInstallments = l.installments?.filter(i => !i.isPaid) || [];
                return sum + (unpaidInstallments[0]?.amount || 0);
              }, 0)
            };
            
            observer.next(summary);
            observer.complete();
          } else {
            observer.error(response.message);
          }
        },
        error: (err) => observer.error(err)
      });
    });
  }
}
