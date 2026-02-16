import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
    Result,
    EmployeeViolation,
    RegisterViolationCommand,
    EmployeeAppraisal,
    SubmitAppraisalCommand
} from '../models/performance.model';

@Injectable({
    providedIn: 'root'
})
export class PerformanceService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}/Performance`;

    // 1. Violations
    registerViolation(command: RegisterViolationCommand): Observable<Result<number>> {
        return this.http.post<Result<number>>(`${this.apiUrl}/violations`, command);
    }

    approveViolation(id: number): Observable<Result<number>> {
        return this.http.put<Result<number>>(`${this.apiUrl}/violations/${id}/approve`, {});
    }

    updateViolation(id: number, command: RegisterViolationCommand): Observable<Result<number>> {
        return this.http.put<Result<number>>(`${this.apiUrl}/violations/${id}`, command);
    }

    deleteViolation(id: number): Observable<Result<number>> {
        return this.http.delete<Result<number>>(`${this.apiUrl}/violations/${id}`);
    }

    getViolationById(id: number): Observable<Result<EmployeeViolation>> {
        return this.http.get<Result<EmployeeViolation>>(`${this.apiUrl}/violations/${id}`);
    }

    getViolations(employeeId?: number, status?: string): Observable<Result<EmployeeViolation[]>> {
        let params = new HttpParams();
        if (employeeId) params = params.set('employeeId', employeeId.toString());
        if (status) params = params.set('status', status);

        return this.http.get<Result<EmployeeViolation[]>>(`${this.apiUrl}/violations`, { params });
    }

    getEmployeeViolations(employeeId: number): Observable<Result<EmployeeViolation[]>> {
        return this.http.get<Result<EmployeeViolation[]>>(`${this.apiUrl}/violations/employee/${employeeId}`);
    }

    // 2. Appraisals
    submitAppraisal(command: SubmitAppraisalCommand): Observable<Result<number>> {
        return this.http.post<Result<number>>(`${this.apiUrl}/appraisals`, command);
    }

    updateAppraisal(id: number, command: SubmitAppraisalCommand): Observable<Result<number>> {
        return this.http.put<Result<number>>(`${this.apiUrl}/appraisals/${id}`, command);
    }

    deleteAppraisal(id: number): Observable<Result<number>> {
        return this.http.delete<Result<number>>(`${this.apiUrl}/appraisals/${id}`);
    }

    getAppraisals(employeeId?: number, cycleId?: number): Observable<Result<EmployeeAppraisal[]>> {
        let params = new HttpParams();
        if (employeeId) params = params.set('employeeId', employeeId.toString());
        if (cycleId) params = params.set('cycleId', cycleId.toString());

        return this.http.get<Result<EmployeeAppraisal[]>>(`${this.apiUrl}/appraisals`, { params });
    }

    getAppraisalById(id: number): Observable<Result<EmployeeAppraisal>> {
        return this.http.get<Result<EmployeeAppraisal>>(`${this.apiUrl}/appraisals/${id}`);
    }
}
