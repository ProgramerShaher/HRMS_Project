import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LeaveTransaction, ApiResponse } from '../models/leave.models';

@Injectable({
  providedIn: 'root'
})
export class LeaveTransactionService {
  private apiUrl = `${environment.apiUrl}/Leaves/Balances`;

  constructor(private http: HttpClient) {}

  getTransactionHistory(filters: {
    employeeId?: number, 
    fromDate?: string, 
    toDate?: string, 
    leaveTypeId?: number, 
    transactionType?: string
  } = {}): Observable<ApiResponse<LeaveTransaction[]>> {
    let params = new HttpParams();
    
    if (filters.employeeId) params = params.set('employeeId', filters.employeeId.toString());
    if (filters.fromDate) params = params.set('fromDate', filters.fromDate);
    if (filters.toDate) params = params.set('toDate', filters.toDate);
    if (filters.leaveTypeId) params = params.set('leaveTypeId', filters.leaveTypeId.toString());
    if (filters.transactionType) params = params.set('transactionType', filters.transactionType);
    
    return this.http.get<ApiResponse<LeaveTransaction[]>>(`${this.apiUrl}/history`, { params });
  }
}
