import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../core/models/api-response';
import { Employee, EmployeeSearchParams } from '../models/employee.models';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Employees`;

  /**
   * البحث عن الموظفين
   */
  searchEmployees(params: EmployeeSearchParams): Observable<ApiResponse<Employee[]>> {
    let httpParams = new HttpParams();
    
    if (params.searchTerm) {
      httpParams = httpParams.set('searchTerm', params.searchTerm);
    }
    if (params.departmentId) {
      httpParams = httpParams.set('departmentId', params.departmentId.toString());
    }
    if (params.isActive !== undefined) {
      httpParams = httpParams.set('isActive', params.isActive.toString());
    }
    if (params.pageNumber) {
      httpParams = httpParams.set('pageNumber', params.pageNumber.toString());
    }
    if (params.pageSize) {
      httpParams = httpParams.set('pageSize', params.pageSize.toString());
    }

    return this.http.get<ApiResponse<Employee[]>>(`${this.apiUrl}/search`, { params: httpParams });
  }

  /**
   * الحصول على جميع الموظفين النشطين
   */
  getActiveEmployees(): Observable<ApiResponse<Employee[]>> {
    return this.http.get<ApiResponse<Employee[]>>(`${this.apiUrl}`);
  }

  /**
   * الحصول على موظف بالمعرف
   */
  getEmployeeById(id: number): Observable<ApiResponse<Employee>> {
    return this.http.get<ApiResponse<Employee>>(`${this.apiUrl}/${id}`);
  }
}
