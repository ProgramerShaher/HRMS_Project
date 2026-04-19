import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response';
import {
  JobGrade, LookupItem,
  Candidate, CreateCandidateCommand, UpdateCandidateCommand,
  Vacancy, CreateVacancyCommand, UpdateVacancyCommand,
  JobApplication, SubmitApplicationCommand, ChangeApplicationStatusCommand,
  Interview, ScheduleInterviewCommand, RecordInterviewResultCommand,
  JobOffer, CreateOfferCommand, AcceptOfferCommand,
  RecruitmentReports
} from '../models/recruitment.models';

@Injectable({ providedIn: 'root' })
export class RecruitmentService {
  private http = inject(HttpClient);
  private api = `${environment.apiUrl}/Recruitment`;

  // ── Config ────────────────────────────────────────────────
  getJobGrades(): Observable<ApiResponse<JobGrade[]>> {
    return this.http.get<ApiResponse<JobGrade[]>>(`${this.api}/config/job-grades`);
  }
  getInterviewTypes(): Observable<ApiResponse<LookupItem[]>> {
    return this.http.get<ApiResponse<LookupItem[]>>(`${this.api}/config/interview-types`);
  }
  getRejectionReasons(): Observable<ApiResponse<LookupItem[]>> {
    return this.http.get<ApiResponse<LookupItem[]>>(`${this.api}/config/rejection-reasons`);
  }

  // ── Candidates ────────────────────────────────────────────
  getCandidates(): Observable<ApiResponse<Candidate[]>> {
    return this.http.get<ApiResponse<Candidate[]>>(`${this.api}/candidates`);
  }
  createCandidate(formData: FormData): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(`${this.api}/candidates`, formData);
  }
  updateCandidate(id: number, cmd: UpdateCandidateCommand): Observable<ApiResponse<number>> {
    return this.http.put<ApiResponse<number>>(`${this.api}/candidates/${id}`, cmd);
  }
  deleteCandidate(id: number): Observable<ApiResponse<number>> {
    return this.http.delete<ApiResponse<number>>(`${this.api}/candidates/${id}`);
  }

  // ── Vacancies ─────────────────────────────────────────────
  getVacancies(status?: string): Observable<ApiResponse<Vacancy[]>> {
    const params = status ? `?status=${status}` : '';
    return this.http.get<ApiResponse<Vacancy[]>>(`${this.api}/vacancies${params}`);
  }
  getVacancyById(id: number): Observable<ApiResponse<Vacancy>> {
    return this.http.get<ApiResponse<Vacancy>>(`${this.api}/vacancies/${id}`);
  }
  createVacancy(cmd: CreateVacancyCommand): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(`${this.api}/vacancies`, cmd);
  }
  updateVacancy(id: number, cmd: UpdateVacancyCommand): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.api}/vacancies/${id}`, cmd);
  }
  closeVacancy(id: number): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.api}/vacancies/${id}/close`, {});
  }
  deleteVacancy(id: number): Observable<ApiResponse<number>> {
    return this.http.delete<ApiResponse<number>>(`${this.api}/vacancies/${id}`);
  }

  // ── Applications ──────────────────────────────────────────
  getApplications(vacancyId?: number, candidateId?: number, status?: string): Observable<ApiResponse<JobApplication[]>> {
    const p = new URLSearchParams();
    if (vacancyId) p.append('vacancyId', String(vacancyId));
    if (candidateId) p.append('candidateId', String(candidateId));
    if (status) p.append('status', status);
    const q = p.toString() ? `?${p}` : '';
    return this.http.get<ApiResponse<JobApplication[]>>(`${this.api}/applications${q}`);
  }
  getApplicationById(id: number): Observable<ApiResponse<JobApplication>> {
    return this.http.get<ApiResponse<JobApplication>>(`${this.api}/applications/${id}`);
  }
  submitApplication(cmd: SubmitApplicationCommand): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(`${this.api}/applications`, cmd);
  }
  changeApplicationStatus(id: number, cmd: ChangeApplicationStatusCommand): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.api}/applications/${id}/status`, cmd);
  }
  withdrawApplication(id: number): Observable<ApiResponse<number>> {
    return this.http.delete<ApiResponse<number>>(`${this.api}/applications/${id}`);
  }

  // ── Interviews ────────────────────────────────────────────
  getInterviews(appId?: number, interviewerId?: number): Observable<ApiResponse<Interview[]>> {
    const p = new URLSearchParams();
    if (appId) p.append('appId', String(appId));
    if (interviewerId) p.append('interviewerId', String(interviewerId));
    const q = p.toString() ? `?${p}` : '';
    return this.http.get<ApiResponse<Interview[]>>(`${this.api}/interviews${q}`);
  }
  scheduleInterview(cmd: ScheduleInterviewCommand): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(`${this.api}/interviews`, cmd);
  }
  recordInterviewResult(id: number, cmd: RecordInterviewResultCommand): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.api}/interviews/${id}/result`, cmd);
  }
  cancelInterview(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.api}/interviews/${id}`, {});
  }

  // ── Offers ────────────────────────────────────────────────
  getOffers(appId?: number, status?: string): Observable<ApiResponse<JobOffer[]>> {
    const p = new URLSearchParams();
    if (appId) p.append('appId', String(appId));
    if (status) p.append('status', status);
    const q = p.toString() ? `?${p}` : '';
    return this.http.get<ApiResponse<JobOffer[]>>(`${this.api}/offers${q}`);
  }
  createOffer(cmd: CreateOfferCommand): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(`${this.api}/offers`, cmd);
  }
  acceptOffer(offerId: number, cmd: AcceptOfferCommand): Observable<ApiResponse<number>> {
    return this.http.post<ApiResponse<number>>(`${this.api}/offers/${offerId}/accept`, cmd);
  }
  withdrawOffer(offerId: number): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.api}/offers/${offerId}/withdraw`, {});
  }

  // ── Stats ─────────────────────────────────────────────────
  getStats(): Observable<ApiResponse<RecruitmentReports>> {
    return this.http.get<ApiResponse<RecruitmentReports>>(`${this.api}/stats`);
  }
}
