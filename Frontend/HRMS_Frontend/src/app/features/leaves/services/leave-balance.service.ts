import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LeaveBalance, ApiResponse } from '../models/leave.models';

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
}
