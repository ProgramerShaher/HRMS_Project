import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LeaveType, PublicHoliday, ApiResponse } from '../models/leave.models';

interface CreateLeaveTypePayload {
  leaveTypeNameAr: string;
  defaultDays: number;
  isDeductible: number;
  requiresAttachment: number;
}

interface UpdateLeaveTypePayload extends CreateLeaveTypePayload {
  leaveTypeId: number;
}

@Injectable({
  providedIn: 'root'
})
export class LeaveConfigurationService {
  private apiUrl = `${environment.apiUrl}/LeaveConfiguration`;

  constructor(private http: HttpClient) {}

  // Get all leave types
  getLeaveTypes(): Observable<ApiResponse<LeaveType[]>> {
    return this.http.get<ApiResponse<LeaveType[]>>(`${this.apiUrl}/leave-types`);
  }

  // Get public holidays
  getPublicHolidays(year?: number): Observable<ApiResponse<PublicHoliday[]>> {
    const url = year 
      ? `${this.apiUrl}/public-holidays?year=${year}`
      : `${this.apiUrl}/public-holidays`;
    
    return this.http.get<ApiResponse<PublicHoliday[]>>(url);
  }

  // Create leave type
  createLeaveType(leaveType: Partial<LeaveType>): Observable<ApiResponse<number>> {
    const payload: CreateLeaveTypePayload = {
      leaveTypeNameAr: leaveType.leaveTypeNameAr ?? '',
      defaultDays: Number(leaveType.defaultDays ?? 0),
      isDeductible: Number(leaveType.isDeductible ?? 1),
      requiresAttachment: Number(leaveType.requiresAttachment ?? 0)
    };
    return this.http.post<ApiResponse<number>>(`${this.apiUrl}/leave-types`, payload);
  }

  // Update leave type
  updateLeaveType(id: number, leaveType: Partial<LeaveType>): Observable<ApiResponse<boolean>> {
    const payload: UpdateLeaveTypePayload = {
      leaveTypeId: id,
      leaveTypeNameAr: leaveType.leaveTypeNameAr ?? '',
      defaultDays: Number(leaveType.defaultDays ?? 0),
      isDeductible: Number(leaveType.isDeductible ?? 1),
      requiresAttachment: Number(leaveType.requiresAttachment ?? 0)
    };
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/leave-types/${id}`, payload);
  }

  // Delete leave type
  deleteLeaveType(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/leave-types/${id}`);
  }
}
