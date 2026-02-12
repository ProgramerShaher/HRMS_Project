import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  LeaveRequest,
  CreateLeaveRequestDto,
  ApiResponse,
  LeaveRequestStatus,
  LeaveDashboardStats
} from '../models/leave.models';

@Injectable({
  providedIn: 'root'
})
export class LeaveRequestService {
  private apiUrl = `${environment.apiUrl}/Leaves/Requests`;

  constructor(private http: HttpClient) {}

  // Get employee's leave requests
  getEmployeeRequests(employeeId: number): Observable<ApiResponse<LeaveRequest[]>> {
    return this.http.get<ApiResponse<LeaveRequest[]>>(`${this.apiUrl}/employee/${employeeId}`);
  }

  // Get pending requests (for managers)
  getPendingRequests(): Observable<ApiResponse<LeaveRequest[]>> {
    return this.http.get<ApiResponse<LeaveRequest[]>>(`${this.apiUrl}/pending`);
  }

  // Create new leave request
  createRequest(request: CreateLeaveRequestDto): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(this.apiUrl, request);
  }

  // Approve leave request
  approveRequest(requestId: number, comments?: string): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(
      `${this.apiUrl}/${requestId}/approve`,
      { requestId, approverComments: comments }
    );
  }

  // Reject leave request
  rejectRequest(requestId: number, reason: string): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(
      `${this.apiUrl}/${requestId}/reject`,
      { requestId, rejectionReason: reason }
    );
  }

  // Cancel leave request
  cancelRequest(requestId: number): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(
      `${this.apiUrl}/${requestId}/cancel`,
      { requestId }
    );
  }

  // Get employee dashboard stats
  getEmployeeStats(employeeId: number): Observable<ApiResponse<LeaveDashboardStats>> {
    return this.http.get<ApiResponse<LeaveDashboardStats>>(`${this.apiUrl}/stats/employee/${employeeId}`);
  }

  // Get manager dashboard stats (for team overview)
  getManagerStats(): Observable<ApiResponse<LeaveDashboardStats>> {
    return this.http.get<ApiResponse<LeaveDashboardStats>>(`${this.apiUrl}/stats/manager`);
  }
}
