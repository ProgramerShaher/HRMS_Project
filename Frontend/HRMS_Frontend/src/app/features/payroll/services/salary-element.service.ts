import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response';
import {
  SalaryElement,
  CreateSalaryElementRequest,
  UpdateSalaryElementRequest,
  SalaryElementSummary
} from '../models/salary-element.models';

/**
 * خدمة إدارة عناصر الراتب
 * Salary Element Management Service
 */
@Injectable({
  providedIn: 'root'
})
export class SalaryElementService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/PayrollSettings/elements`;

  /**
   * الحصول على جميع عناصر الراتب
   * Get all salary elements
   */
  getElements(): Observable<ApiResponse<SalaryElement[]>> {
    return this.http.get<ApiResponse<SalaryElement[]>>(this.apiUrl);
  }

  /**
   * الحصول على عنصر راتب بالمعرف
   * Get salary element by ID
   */
  getElementById(id: number): Observable<ApiResponse<SalaryElement>> {
    return this.http.get<ApiResponse<SalaryElement>>(`${this.apiUrl}/${id}`);
  }

  /**
   * إنشاء عنصر راتب جديد
   * Create new salary element
   */
  createElement(request: CreateSalaryElementRequest): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(this.apiUrl, request);
  }

  /**
   * تحديث عنصر راتب
   * Update salary element
   */
  updateElement(request: UpdateSalaryElementRequest): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(this.apiUrl, request);
  }

  /**
   * حذف عنصر راتب
   * Delete salary element
   */
  deleteElement(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }

  /**
   * حساب ملخص عناصر الراتب
   * Calculate salary elements summary
   */
  getElementsSummary(): Observable<SalaryElementSummary> {
    return new Observable(observer => {
      this.getElements().subscribe({
        next: (response) => {
          if (response.succeeded && response.data) {
            const elements = response.data;
            const summary: SalaryElementSummary = {
              totalElements: elements.length,
              earningsCount: elements.filter(e => e.elementType === 'ALLOWANCE').length,
              deductionsCount: elements.filter(e => e.elementType === 'DEDUCTION').length,
              hasBasicElement: elements.some(e => e.isBasic)
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
