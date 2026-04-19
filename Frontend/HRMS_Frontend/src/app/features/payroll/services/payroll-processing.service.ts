import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response';
import {
  MonthlySalaryCalculation,
  PayrollRun,
  Payslip,
  ProcessPayrunRequest,
  RollbackPayrollRequest,
  PostPayrollToGLRequest,
  PayrollDashboardStats
} from '../models/payroll-processing.models';

/**
 * خدمة معالجة الرواتب
 * Payroll Processing Service
 */
@Injectable({
  providedIn: 'root'
})
export class PayrollProcessingService {
  private http = inject(HttpClient);
  private payrollUrl = `${environment.apiUrl}/Payroll`;

  /**
   * معالجة رواتب شهر
   * Process monthly payroll
   */
  processMonth(month: number, year: number): Observable<ApiResponse<number>> {
    const params = new HttpParams()
      .set('month', month.toString())
      .set('year', year.toString());
    
    return this.http.post<ApiResponse<number>>(`${this.payrollUrl}/process-month`, null, { params });
  }

  /**
   * التراجع عن مسير رواتب
   * Rollback payroll run
   */
  rollbackPayroll(runId: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.payrollUrl}/rollback/${runId}`, {});
  }

  /**
   * الحصول على قسيمة راتب موظف
   * Get employee payslip
   */
  getPayslip(employeeId: number, month: number, year: number): Observable<ApiResponse<MonthlySalaryCalculation>> {
    return this.http.get<ApiResponse<MonthlySalaryCalculation>>(
      `${this.payrollUrl}/payslip/${employeeId}/${month}/${year}`
    );
  }

  /**
   * تصدير ملف البنك
   * Export bank file
   */
  exportBankFile(month: number, year: number): Observable<Blob> {
    return this.http.get(`${this.payrollUrl}/export-bank-file/${month}/${year}`, {
      responseType: 'blob'
    });
  }

  /**
   * ترحيل الرواتب إلى دليل الحسابات
   * Post payroll to general ledger
   */
  postToGL(runId: number): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(`${this.payrollUrl}/post-to-gl/${runId}`, {});
  }

  /**
   * الحصول على قسائم رواتب موظف
   * Get employee payslips (custom implementation)
   */
  getEmployeePayslips(employeeId: number, year?: number): Observable<ApiResponse<Payslip[]>> {
    // Note: This endpoint might need to be added to the backend
    // For now, we'll use a workaround
    return new Observable(observer => {
      observer.next({
        succeeded: true,
        data: [],
        message: 'Not implemented yet',
        errors: null,
        statusCode: 200
      } as ApiResponse<Payslip[]>);
      observer.complete();
    });
  }

  /**
   * الحصول على جميع مسيرات الرواتب
   * Get all payroll runs
   */
  getPayrollRuns(year?: number, status?: string): Observable<ApiResponse<PayrollRun[]>> {
    let params = new HttpParams();
    if (year) params = params.set('year', year.toString());
    if (status) params = params.set('status', status);
    
    return this.http.get<ApiResponse<PayrollRun[]>>(`${this.payrollUrl}/runs`, { params });
  }

  /**
   * الحصول على تفاصيل مسير رواتب محدد
   * Get payroll run details
   */
  getPayrunDetails(runId: number): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.payrollUrl}/runs/${runId}/details`);
  }

  /**
   * حساب إحصائيات لوحة التحكم
   * Calculate dashboard statistics
   */
  getDashboardStats(employeeId: number): Observable<PayrollDashboardStats> {
    const currentDate = new Date();
    const currentMonth = currentDate.getMonth() + 1;
    const currentYear = currentDate.getFullYear();

    return new Observable(observer => {
      this.getPayslip(employeeId, currentMonth, currentYear).subscribe({
        next: (response) => {
          if (response.succeeded && response.data) {
            const calc = response.data;
            const stats: PayrollDashboardStats = {
              currentMonthNetSalary: calc.netSalary,
              totalDeductions: calc.totalStructureDeductions + calc.loanDeductions + calc.attendancePenalties + calc.totalViolations + calc.otherDeductions,
              activeLoansCount: 0, // Will be fetched separately
              pendingInstallments: calc.paidInstallmentIds.length,
              upcomingInstallments: 0,
              totalEarnings: calc.basicSalary + calc.totalAllowances + calc.overtimeEarnings,
              totalAllowances: calc.totalAllowances
            };
            observer.next(stats);
            observer.complete();
          } else {
            // Return empty stats if no payslip found
            observer.next({
              currentMonthNetSalary: 0,
              totalDeductions: 0,
              activeLoansCount: 0,
              pendingInstallments: 0,
              upcomingInstallments: 0,
              totalEarnings: 0,
              totalAllowances: 0
            });
            observer.complete();
          }
        },
        error: (err) => {
          // Return empty stats on error
          observer.next({
            currentMonthNetSalary: 0,
            totalDeductions: 0,
            activeLoansCount: 0,
            pendingInstallments: 0,
            upcomingInstallments: 0,
            totalEarnings: 0,
            totalAllowances: 0
          });
          observer.complete();
        }
      });
    });
  }

  /**
   * الحصول على جميع رواتب الموظفين (للإدارة)
   * Get all employees salaries (for admin)
   */
  getAllEmployeesSalaries(
    departmentId?: number,
    isActive?: boolean,
    searchTerm?: string
  ): Observable<ApiResponse<any[]>> {
    let params = new HttpParams();
    
    if (departmentId) {
      params = params.set('departmentId', departmentId.toString());
    }
    if (isActive !== null && isActive !== undefined) {
      params = params.set('isActive', isActive.toString());
    }
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<ApiResponse<any[]>>(`${this.payrollUrl}/all-employees-salaries`, { params });
  }

  /**
   * الحصول على ملخص شهري للرواتب
   * Get monthly payroll summary report
   */
  getMonthlySummary(month: number, year: number, departmentId?: number): Observable<ApiResponse<any>> {
    let params = new HttpParams()
      .set('month', month.toString())
      .set('year', year.toString());
    
    if (departmentId) {
      params = params.set('departmentId', departmentId.toString());
    }
    
    return this.http.get<ApiResponse<any>>(`${this.payrollUrl}/reports/monthly-summary`, { params });
  }

  /**
   * الحصول على تفاصيل راتب موظف
   * Get employee salary breakdown
   */
  getEmployeeSalaryBreakdown(employeeId: number, month?: number, year?: number): Observable<ApiResponse<any>> {
    let params = new HttpParams();
    if (month) params = params.set('month', month.toString());
    if (year) params = params.set('year', year.toString());
    
    return this.http.get<ApiResponse<any>>(`${this.payrollUrl}/employee-salary-breakdown/${employeeId}`, { params });
  }

  /**
   * الحصول على سجل التتبع
   * Get audit trail logs
   */
  getAuditTrail(pageNumber: number = 1, pageSize: number = 50): Observable<ApiResponse<any[]>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<ApiResponse<any[]>>(`${this.payrollUrl}/reports/audit-trail`, { params });
  }
}
