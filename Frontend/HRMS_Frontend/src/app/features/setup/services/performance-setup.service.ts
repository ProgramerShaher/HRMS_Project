import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Result, ViolationType, DisciplinaryAction, Kpi, AppraisalCycle } from '../models/performance-setup.model';

@Injectable({
  providedIn: 'root'
})
export class PerformanceSetupService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Performance/config`;

  // 1. Violation Types
  getViolationTypes(): Observable<Result<ViolationType[]>> {
    return this.http.get<Result<ViolationType[]>>(`${this.apiUrl}/violation-types`);
  }

  getViolationTypeById(id: number): Observable<Result<ViolationType>> {
    return this.http.get<Result<ViolationType>>(`${this.apiUrl}/violation-types/${id}`);
  }

  createViolationType(data: Partial<ViolationType>): Observable<Result<number>> {
    return this.http.post<Result<number>>(`${this.apiUrl}/violation-types`, data);
  }

  updateViolationType(id: number, data: Partial<ViolationType>): Observable<Result<boolean>> {
    return this.http.put<Result<boolean>>(`${this.apiUrl}/violation-types/${id}`, data);
  }

  deleteViolationType(id: number): Observable<Result<boolean>> {
    return this.http.delete<Result<boolean>>(`${this.apiUrl}/violation-types/${id}`);
  }

  // 2. Disciplinary Actions
  getDisciplinaryActions(): Observable<Result<DisciplinaryAction[]>> {
    return this.http.get<Result<DisciplinaryAction[]>>(`${this.apiUrl}/disciplinary-actions`);
  }

  getDisciplinaryActionById(id: number): Observable<Result<DisciplinaryAction>> {
    return this.http.get<Result<DisciplinaryAction>>(`${this.apiUrl}/disciplinary-actions/${id}`);
  }

  createDisciplinaryAction(data: Partial<DisciplinaryAction>): Observable<Result<number>> {
    return this.http.post<Result<number>>(`${this.apiUrl}/disciplinary-actions`, data);
  }

  updateDisciplinaryAction(id: number, data: Partial<DisciplinaryAction>): Observable<Result<boolean>> {
    return this.http.put<Result<boolean>>(`${this.apiUrl}/disciplinary-actions/${id}`, data);
  }

  deleteDisciplinaryAction(id: number): Observable<Result<boolean>> {
    return this.http.delete<Result<boolean>>(`${this.apiUrl}/disciplinary-actions/${id}`);
  }

  // 3. KPI Library
  getKpis(): Observable<Result<Kpi[]>> {
    return this.http.get<Result<Kpi[]>>(`${this.apiUrl}/kpis`);
  }

  getKpiById(id: number): Observable<Result<Kpi>> {
    return this.http.get<Result<Kpi>>(`${this.apiUrl}/kpis/${id}`);
  }

  createKpi(data: Partial<Kpi>): Observable<Result<number>> {
    return this.http.post<Result<number>>(`${this.apiUrl}/kpis`, data);
  }

  updateKpi(id: number, data: Partial<Kpi>): Observable<Result<boolean>> {
    return this.http.put<Result<boolean>>(`${this.apiUrl}/kpis/${id}`, data);
  }

  deleteKpi(id: number): Observable<Result<boolean>> {
    return this.http.delete<Result<boolean>>(`${this.apiUrl}/kpis/${id}`);
  }

  // 4. Appraisal Cycles
  getAppraisalCycles(): Observable<Result<AppraisalCycle[]>> {
    return this.http.get<Result<AppraisalCycle[]>>(`${this.apiUrl}/appraisal-cycles`);
  }

  getAppraisalCycleById(id: number): Observable<Result<AppraisalCycle>> {
    return this.http.get<Result<AppraisalCycle>>(`${this.apiUrl}/appraisal-cycles/${id}`);
  }

  createAppraisalCycle(data: Partial<AppraisalCycle>): Observable<Result<number>> {
    return this.http.post<Result<number>>(`${this.apiUrl}/appraisal-cycles`, data);
  }

  updateAppraisalCycle(id: number, data: Partial<AppraisalCycle>): Observable<Result<boolean>> {
    return this.http.put<Result<boolean>>(`${this.apiUrl}/appraisal-cycles/${id}`, data);
  }

  deleteAppraisalCycle(id: number): Observable<Result<boolean>> {
    return this.http.delete<Result<boolean>>(`${this.apiUrl}/appraisal-cycles/${id}`);
  }
}
