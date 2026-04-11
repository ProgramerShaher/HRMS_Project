import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse, AdjustBalanceCommand, EmployeeLeaveTypeBalance, InitializeBalancesDto, LeaveBalance } from '../models/leave.models';

@Injectable({ providedIn: 'root' })
export class LeaveBalanceService {
  private apiUrl = `${environment.apiUrl}/Leaves/Balances`;

  constructor(private http: HttpClient) {}

  /** GET /api/Leaves/Balances/employee/{id} */
  getEmployeeBalances(employeeId: number, year?: number): Observable<ApiResponse<LeaveBalance[]>> {
    const url = year
      ? `${this.apiUrl}/employee/${employeeId}?year=${year}`
      : `${this.apiUrl}/employee/${employeeId}`;
    return this.http.get<ApiResponse<LeaveBalance[]>>(url);
  }

  /** GET /api/Leaves/Balances/employees */
  getEmployeesBalances(filters: {
    year?: number;
    departmentId?: number;
    employeeId?: number;
    search?: string;
  } = {}): Observable<ApiResponse<EmployeeLeaveTypeBalance[]>> {
    const params = new URLSearchParams();
    if (filters.year)         params.set('year',         String(filters.year));
    if (filters.departmentId) params.set('departmentId', String(filters.departmentId));
    if (filters.employeeId)   params.set('employeeId',   String(filters.employeeId));
    if (filters.search)       params.set('search',       filters.search);

    const qs = params.toString();
    const url = qs ? `${this.apiUrl}/employees?${qs}` : `${this.apiUrl}/employees`;
    return this.http.get<ApiResponse<EmployeeLeaveTypeBalance[]>>(url);
  }

  /** POST /api/Leaves/Balances/initialize */
  initializeBalances(dto: InitializeBalancesDto): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/initialize`, dto);
  }

  /** POST /api/Leaves/Balances/adjust */
  adjustBalance(command: AdjustBalanceCommand): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/adjust`, command);
  }
}
