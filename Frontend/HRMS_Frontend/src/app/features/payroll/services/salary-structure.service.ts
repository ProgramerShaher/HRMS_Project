import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response';
import {
  EmployeeSalaryStructure,
  SetEmployeeSalaryStructureRequest,
  InitializeSalaryFromGradeRequest,
  SalaryBreakdown,
  SalaryStructureSummary
} from '../models/salary-structure.models';

/**
 * خدمة إدارة هيكل الراتب
 * Salary Structure Management Service
 */
@Injectable({
  providedIn: 'root'
})
export class SalaryStructureService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/PayrollSettings`;

  /**
   * الحصول على جميع هياكل الرواتب
   * Get all salary structures
   */
  getAllStructures(departmentId?: number, searchTerm?: string): Observable<ApiResponse<SalaryStructureSummary[]>> {
    let params = new HttpParams();
    if (departmentId) params = params.set('departmentId', departmentId.toString());
    if (searchTerm) params = params.set('searchTerm', searchTerm);
    
    return this.http.get<ApiResponse<SalaryStructureSummary[]>>(`${this.apiUrl}/all-structures`, { params });
  }

  /**
   * الحصول على هيكل راتب موظف
   * Get employee salary structure
   */
  getEmployeeStructure(employeeId: number): Observable<ApiResponse<EmployeeSalaryStructure>> {
    return this.http.get<ApiResponse<EmployeeSalaryStructure>>(`${this.apiUrl}/employee-structure/${employeeId}`);
  }

  /**
   * تحديث هيكل راتب موظف
   * Update employee salary structure
   */
  updateStructure(request: SetEmployeeSalaryStructureRequest): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/update-structure`, request);
  }

  /**
   * تهيئة راتب موظف من الدرجة الوظيفية
   * Initialize salary from job grade
   */
  initializeFromGrade(employeeId: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/initialize-from-grade/${employeeId}`, {});
  }

  /**
   * حساب تفصيل الراتب
   * Calculate salary breakdown
   */
  getSalaryBreakdown(employeeId: number): Observable<SalaryBreakdown> {
    return new Observable(observer => {
      this.getEmployeeStructure(employeeId).subscribe({
        next: (response) => {
          if (response.succeeded && response.data) {
            const structure = response.data;
            const basicElement = structure.elements.find(e => e.elementType === 'ALLOWANCE' && e.amount > 0);
            
            const breakdown: SalaryBreakdown = {
              basicSalary: basicElement?.amount || 0,
              allowances: structure.elements.filter(e => e.elementType === 'ALLOWANCE' && e.elementId !== basicElement?.elementId),
              deductions: structure.elements.filter(e => e.elementType === 'DEDUCTION'),
              totalEarnings: structure.totalEarnings,
              totalDeductions: structure.totalDeductions,
              netSalary: structure.netSalary
            };
            
            observer.next(breakdown);
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
