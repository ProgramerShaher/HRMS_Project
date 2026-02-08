import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LeaveType, PublicHoliday, ApiResponse } from '../models/leave.models';

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
}
