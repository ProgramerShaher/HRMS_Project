import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LeaveBalance, ApiResponse, EmployeeLeaveTypeBalance } from '../models/leave.models';

@Injectable({
  providedIn: 'root'
})
export class LeaveBalanceService {
  private apiUrl = `${environment.apiUrl}/Leaves/Balances`;

  constructor(private http: HttpClient) {}

  // Get employee's leave balances
  getEmployeeBalances(employeeId: number, year?: number): Observable<ApiResponse<LeaveBalance[]>> {
    const url = year 
      ? `${this.apiUrl}/employee/${employeeId}?year=${year}`
      : `${this.apiUrl}/employee/${employeeId}`;
    
    return this.http.get<ApiResponse<LeaveBalance[]>>(url);
  }

  // Get balances for all employees (admin/HR)
  getEmployeesBalances(filters: {
    year?: number;
    departmentId?: number;
    employeeId?: number;
    search?: string;
  } = {}): Observable<ApiResponse<EmployeeLeaveTypeBalance[]>> {
    const params = new URLSearchParams();
    if (filters.year) params.set('year', String(filters.year));
    if (filters.departmentId) params.set('departmentId', String(filters.departmentId));
    if (filters.employeeId) params.set('employeeId', String(filters.employeeId));
    if (filters.search) params.set('search', filters.search);

    const qs = params.toString();
    const url = qs ? `${this.apiUrl}/employees?${qs}` : `${this.apiUrl}/employees`;
    return this.http.get<ApiResponse<EmployeeLeaveTypeBalance[]>>(url);
  }
}
