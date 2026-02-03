import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Employee } from '../models/employee.model';
import { CreateEmployeeDto } from '../models/create-employee.dto';
import { EmployeeProfile } from '../models/employee-profile.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private apiUrl = `${environment.apiUrl}/Employees`;

  constructor(private http: HttpClient) { }

  /**
   * Get paginated employees
   */
  getAll(pageNumber: number = 1, pageSize: number = 10, search?: string): Observable<any> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<any>(this.apiUrl, { params });
  }

  /**
   * Get employee basic details by ID
   */
  getById(id: number): Observable<EmployeeProfile> {
    return this.http.get<EmployeeProfile>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get full employee profile (Aggregated view)
   */
  getFullProfile(id: number): Observable<EmployeeProfile> {
    return this.http.get<EmployeeProfile>(`${this.apiUrl}/${id}/full-profile`);
  }

  /**
   * Create a new employee with full details
   */
  create(employee: CreateEmployeeDto): Observable<number> {
    return this.http.post<number>(this.apiUrl, employee);
  }

  /**
   * Update employee main info
   */
  update(id: number, employee: Partial<CreateEmployeeDto>): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, employee);
  }

  /**
   * Upload an employee document
   */
  uploadDocument(employeeId: number, file: File, documentTypeId: number, documentNumber?: string, expiryDate?: string): Observable<any> {
    const formData = new FormData();
    formData.append('EmployeeId', employeeId.toString());
    formData.append('DocumentTypeId', documentTypeId.toString());
    formData.append('File', file);
    
    if (documentNumber) formData.append('DocumentNumber', documentNumber);
    if (expiryDate) formData.append('ExpiryDate', expiryDate);

    return this.http.post(`${this.apiUrl}/${employeeId}/documents`, formData);
  }
}
