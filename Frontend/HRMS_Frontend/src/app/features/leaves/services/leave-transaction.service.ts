import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LeaveTransaction, ApiResponse } from '../models/leave.models';

@Injectable({
  providedIn: 'root'
})
export class LeaveTransactionService {
  private apiUrl = `${environment.apiUrl}/Leaves/Balances`;

  constructor(private http: HttpClient) {}

  getTransactionHistory(employeeId?: number, year?: number): Observable<ApiResponse<LeaveTransaction[]>> {
    let url = `${this.apiUrl}/history`;
    const params: string[] = [];
    
    if (employeeId) params.push(`employeeId=${employeeId}`);
    if (year) params.push(`year=${year}`);
    
    if (params.length > 0) {
      url += '?' + params.join('&');
    }
    
    return this.http.get<ApiResponse<LeaveTransaction[]>>(url);
  }
}
