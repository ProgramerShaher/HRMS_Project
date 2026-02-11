import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { 
  Result, 
  RegisterPunchCommand, 
  UpdateShiftSwapCommand, 
  RevokeShiftSwapCommand, 
  UpdateOvertimeCommand, 
  AttendanceStatsDto, 
  TimesheetDayDto, 
  LiveStatusDto, 
  AttendanceExceptionDto, 
  ManualCorrectionCommand, 
  PayrollAttendanceSummaryDto, 
  CreatePermissionRequestCommand, 
  ApproveRejectPermissionRequestCommand, 
  MyRosterDto 
} from '../models/attendance.models';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {
  private apiUrl = `${environment.apiUrl}/Attendance`;
  private settingsUrl = `${environment.apiUrl}/AttendanceSettings`; // Needed for creation endpoints if they live there

  constructor(private http: HttpClient) { }

  // Punch
  punch(command: RegisterPunchCommand): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/punch`, command);
  }

  // Swap Requests
  applySwap(command: any): Observable<number> {
      return this.http.post<number>(`${this.settingsUrl}/apply-swap`, command);
  }

  updateSwapRequest(command: UpdateShiftSwapCommand): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/swap-requests`, command);
  }

  cancelSwapRequest(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/swap-requests/${id}`);
  }

  revokeSwapRequest(command: RevokeShiftSwapCommand): Observable<boolean> {
    return this.http.post<boolean>(`${this.apiUrl}/swap-requests/revoke`, command);
  }

  // Overtime Requests
  applyOvertime(command: any): Observable<number> {
      return this.http.post<number>(`${this.settingsUrl}/apply-overtime`, command);
  }

  updateOvertimeRequest(command: UpdateOvertimeCommand): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/overtime-requests`, command);
  }

  cancelOvertimeRequest(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/overtime-requests/${id}`);
  }

  // Stats & Dashboard
  getStats(employeeId: number, month: number, year: number): Observable<Result<AttendanceStatsDto>> {
    const params = new HttpParams()
      .set('employeeId', employeeId)
      .set('month', month)
      .set('year', year);
    return this.http.get<Result<AttendanceStatsDto>>(`${this.apiUrl}/stats`, { params });
  }

  getTimesheet(employeeId: number, month: number, year: number): Observable<Result<TimesheetDayDto[]>> {
    const params = new HttpParams()
      .set('employeeId', employeeId)
      .set('month', month)
      .set('year', year);
    return this.http.get<Result<TimesheetDayDto[]>>(`${this.apiUrl}/timesheet`, { params });
  }

  getLiveStatus(): Observable<Result<LiveStatusDto>> {
    return this.http.get<Result<LiveStatusDto>>(`${this.apiUrl}/dashboard/live`);
  }

  getExceptions(): Observable<Result<AttendanceExceptionDto[]>> {
    return this.http.get<Result<AttendanceExceptionDto[]>>(`${this.apiUrl}/dashboard/exceptions`);
  }

  // Correction
  manualCorrection(command: ManualCorrectionCommand): Observable<Result<boolean>> {
    return this.http.post<Result<boolean>>(`${this.apiUrl}/correction`, command);
  }

  // Reports
  getPayrollSummary(month: number, year: number): Observable<Result<PayrollAttendanceSummaryDto[]>> {
    const params = new HttpParams()
      .set('month', month)
      .set('year', year);
    return this.http.get<Result<PayrollAttendanceSummaryDto[]>>(`${this.apiUrl}/reports/payroll-summary`, { params });
  }

  // Permissions
  applyPermission(command: CreatePermissionRequestCommand): Observable<Result<number>> {
    return this.http.post<Result<number>>(`${this.apiUrl}/permissions`, command);
  }

  actionPermission(command: ApproveRejectPermissionRequestCommand): Observable<Result<boolean>> {
    return this.http.post<Result<boolean>>(`${this.apiUrl}/permissions/action`, command);
  }

  // Roster
  getMyRoster(): Observable<Result<MyRosterDto[]>> {
    return this.http.get<Result<MyRosterDto[]>>(`${this.apiUrl}/my-roster`);
  }
}
