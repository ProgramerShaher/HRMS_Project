import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
    Result,
    EmployeeViolation,
    RegisterViolationCommand,
    EmployeeAppraisal,
    AppraisalCycle,
    CreateCycleCommand,
    KpiLibrary,
    CreateKpiCommand,
    SubmitSelfAppraisalCommand,
    SubmitManagerAppraisalCommand,
    FinalizeAppraisalCommand,
    UpdateViolationCommand,
    InitiateAppraisalCommand
} from '../models/performance.model';

@Injectable({ providedIn: 'root' })
export class PerformanceService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}/Performance`;

    // ── Appraisal Cycles ─────────────────────────────────────────────────────
    getCycles(): Observable<Result<AppraisalCycle[]>> {
        return this.http.get<Result<AppraisalCycle[]>>(`${this.apiUrl}/cycles`);
    }

    createCycle(cmd: CreateCycleCommand): Observable<Result<number>> {
        return this.http.post<Result<number>>(`${this.apiUrl}/cycles`, cmd);
    }

    deleteCycle(id: number): Observable<Result<number>> {
        return this.http.delete<Result<number>>(`${this.apiUrl}/cycles/${id}`);
    }

    // ── KPI Library ──────────────────────────────────────────────────────────
    getKpis(): Observable<Result<KpiLibrary[]>> {
        return this.http.get<Result<KpiLibrary[]>>(`${this.apiUrl}/kpis`);
    }

    createKpi(cmd: CreateKpiCommand): Observable<Result<number>> {
        return this.http.post<Result<number>>(`${this.apiUrl}/kpis`, cmd);
    }

    updateKpi(id: number, cmd: CreateKpiCommand): Observable<Result<number>> {
        return this.http.put<Result<number>>(`${this.apiUrl}/kpis/${id}`, cmd);
    }

    deleteKpi(id: number): Observable<Result<number>> {
        return this.http.delete<Result<number>>(`${this.apiUrl}/kpis/${id}`);
    }

    // ── Appraisals ───────────────────────────────────────────────────────────
    getAppraisals(employeeId?: number, cycleId?: number, phase?: string): Observable<Result<EmployeeAppraisal[]>> {
        let params = new HttpParams();
        if (employeeId) params = params.set('employeeId', employeeId.toString());
        if (cycleId) params = params.set('cycleId', cycleId.toString());
        if (phase) params = params.set('phase', phase);
        return this.http.get<Result<EmployeeAppraisal[]>>(`${this.apiUrl}/appraisals`, { params });
    }

    getAppraisalById(id: number): Observable<Result<EmployeeAppraisal>> {
        return this.http.get<Result<EmployeeAppraisal>>(`${this.apiUrl}/appraisals/${id}`);
    }

    initiateAppraisal(cmd: InitiateAppraisalCommand): Observable<Result<number>> {
        return this.http.post<Result<number>>(`${this.apiUrl}/appraisals/initiate`, cmd);
    }

    submitSelfAppraisal(id: number, cmd: SubmitSelfAppraisalCommand): Observable<Result<number>> {
        return this.http.put<Result<number>>(`${this.apiUrl}/appraisals/${id}/self`, cmd);
    }

    submitManagerAppraisal(id: number, cmd: SubmitManagerAppraisalCommand): Observable<Result<number>> {
        return this.http.put<Result<number>>(`${this.apiUrl}/appraisals/${id}/manager`, cmd);
    }

    finalizeAppraisal(id: number, cmd: FinalizeAppraisalCommand): Observable<Result<number>> {
        return this.http.put<Result<number>>(`${this.apiUrl}/appraisals/${id}/finalize`, cmd);
    }

    deleteAppraisal(id: number): Observable<Result<number>> {
        return this.http.delete<Result<number>>(`${this.apiUrl}/appraisals/${id}`);
    }

    // ── Violations ───────────────────────────────────────────────────────────
    getViolations(employeeId?: number, status?: string): Observable<Result<EmployeeViolation[]>> {
        let params = new HttpParams();
        if (employeeId) params = params.set('employeeId', employeeId.toString());
        if (status) params = params.set('status', status);
        return this.http.get<Result<EmployeeViolation[]>>(`${this.apiUrl}/violations`, { params });
    }

    registerViolation(cmd: RegisterViolationCommand): Observable<Result<number>> {
        return this.http.post<Result<number>>(`${this.apiUrl}/violations`, cmd);
    }

    approveViolation(id: number): Observable<Result<number>> {
        return this.http.put<Result<number>>(`${this.apiUrl}/violations/${id}/approve`, {});
    }

    updateViolation(id: number, cmd: UpdateViolationCommand): Observable<Result<number>> {
        return this.http.put<Result<number>>(`${this.apiUrl}/violations/${id}`, cmd);
    }

    deleteViolation(id: number): Observable<Result<number>> {
        return this.http.delete<Result<number>>(`${this.apiUrl}/violations/${id}`);
    }
}
