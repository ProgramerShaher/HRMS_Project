import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { 
  ShiftTypeDto, 
  CreateShiftTypeCommand, 
  InitializeRosterCommand, 
  AssignShiftCommand, 
  CreateShiftSwapCommand, 
  ActionShiftSwapCommand, 
  ApplyOvertimeCommand, 
  ActionOvertimeCommand, 
  MonthlyClosingResultDto,
  ProcessAttendanceCommand, // Added import
  Result,
} from '../models/attendance.models';

@Injectable({
  providedIn: 'root'
})
export class AttendanceSettingsService {
  private apiUrl = `${environment.apiUrl}/AttendanceSettings`;
  private attendanceApiUrl = `${environment.apiUrl}/Attendance`; // For monthly closing if it's there

  constructor(private http: HttpClient) { }

  // Shifts
  getAllShifts(): Observable<ShiftTypeDto[]> {
    return this.http.get<ShiftTypeDto[]>(`${this.apiUrl}/shifts`);
  }

  createShift(command: CreateShiftTypeCommand): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/shifts`, command);
  }

  updateShift(id: number, command: CreateShiftTypeCommand): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/shifts/${id}`, { ...command, shiftId: id });
  }

  deleteShift(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/shifts/${id}`);
  }

  // Roster & Process
  initializeRoster(command: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/initialize-roster`, command);
  }

  getEmployeeRoster(employeeId: number): Observable<any> {
      return this.http.get(`${this.apiUrl}/roster/${employeeId}`);
  }

  updateRosterDay(command: any): Observable<any> {
      return this.http.post(`${this.apiUrl}/roster/update-day`, command);
  }

  assignShift(command: AssignShiftCommand): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/roster/assign`, command);
  }

  processAttendance(command: ProcessAttendanceCommand): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/process-attendance`, command);
  }

  // Swap Actions
  applySwap(command: CreateShiftSwapCommand): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/apply-swap`, command);
  }

  actionSwapRequest(command: ActionShiftSwapCommand): Observable<boolean> {
    return this.http.post<boolean>(`${this.apiUrl}/action-swap-request`, command);
  }

  // Overtime Actions
  applyOvertime(command: ApplyOvertimeCommand): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/apply-overtime`, command);
  }

  actionOvertime(command: ActionOvertimeCommand): Observable<boolean> {
    return this.http.post<boolean>(`${this.apiUrl}/action-overtime`, command);
  }
  
  // Monthly Closing (Located in AttendanceController in backend but related to settings/admin)
  processMonthlyClosing(year: number, month: number, closedByUserId: number): Observable<MonthlyClosingResultDto> {
    return this.http.post<MonthlyClosingResultDto>(`${this.attendanceApiUrl}/monthly-closing`, { year, month, closedByUserId });
  }
}
