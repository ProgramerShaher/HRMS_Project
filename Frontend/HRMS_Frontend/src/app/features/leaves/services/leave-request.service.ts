import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse, CreateLeaveRequestDto, LeaveDashboardStats, LeaveRequest } from '../models/leave.models';

@Injectable({ providedIn: 'root' })
export class LeaveRequestService {
  private apiUrl = `${environment.apiUrl}/Leaves/Requests`;

  constructor(private http: HttpClient) {}

  /** GET /api/Leaves/Requests/employee/{id} */
  getEmployeeRequests(employeeId: number): Observable<ApiResponse<LeaveRequest[]>> {
    return this.http.get<ApiResponse<LeaveRequest[]>>(`${this.apiUrl}/employee/${employeeId}`);
  }

  /** GET /api/Leaves/Requests/all */
  getAllRequests(): Observable<ApiResponse<LeaveRequest[]>> {
    return this.http.get<ApiResponse<LeaveRequest[]>>(`${this.apiUrl}/all`);
  }

  /** GET /api/Leaves/Requests/pending */
  getPendingRequests(): Observable<ApiResponse<LeaveRequest[]>> {
    return this.http.get<ApiResponse<LeaveRequest[]>>(`${this.apiUrl}/pending`);
  }

  /** POST /api/Leaves/Requests */
  createRequest(request: CreateLeaveRequestDto): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(this.apiUrl, request);
  }

  /** PUT /api/Leaves/Requests/{id}/approve */
  approveRequest(requestId: number, comments?: string): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(
      `${this.apiUrl}/${requestId}/approve`,
      { requestId, approverComments: comments ?? '' }
    );
  }

  /** PUT /api/Leaves/Requests/{id}/reject */
  rejectRequest(requestId: number, reason: string): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(
      `${this.apiUrl}/${requestId}/reject`,
      { requestId, rejectionReason: reason }
    );
  }

  /** PUT /api/Leaves/Requests/{id}/cancel */
  cancelRequest(requestId: number): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(
      `${this.apiUrl}/${requestId}/cancel`,
      { requestId }
    );
  }

  /** GET /api/Leaves/Requests/stats/employee/{id} */
  getEmployeeStats(employeeId: number): Observable<ApiResponse<LeaveDashboardStats>> {
    return this.http.get<ApiResponse<LeaveDashboardStats>>(`${this.apiUrl}/stats/employee/${employeeId}`);
  }

  /** GET /api/Leaves/Requests/stats/manager */
  getManagerStats(): Observable<ApiResponse<LeaveDashboardStats>> {
    return this.http.get<ApiResponse<LeaveDashboardStats>>(`${this.apiUrl}/stats/manager`);
  }
}
