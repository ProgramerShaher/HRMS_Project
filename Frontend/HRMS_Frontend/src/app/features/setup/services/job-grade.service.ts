import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CreateJobGradeCommand, JobGradeDto, UpdateJobGradeCommand } from '../models/job-grade.dto';

@Injectable({
  providedIn: 'root'
})
export class JobGradeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/JobGrades`;

  getAll(): Observable<JobGradeDto[]> {
    return this.http.get<JobGradeDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<JobGradeDto> {
    return this.http.get<JobGradeDto>(`${this.apiUrl}/${id}`);
  }

  create(command: CreateJobGradeCommand): Observable<number> {
    return this.http.post<number>(this.apiUrl, command);
  }

  update(command: UpdateJobGradeCommand): Observable<number> {
    return this.http.put<number>(`${this.apiUrl}/${command.jobGradeId}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
