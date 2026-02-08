import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { CreateDepartmentCommand, DepartmentDto, UpdateDepartmentCommand } from '../models/department.dto';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Departments`;

  getAll(): Observable<DepartmentDto[]> {
    return this.http.get<any>(this.apiUrl).pipe(
      map(response => {
        // API returns: { success: true, data: { items: [...], totalCount: X }, message: "..." }
        return response?.data?.items || response?.items || response?.data || [];
      })
    );
  }

  getById(id: number): Observable<DepartmentDto> {
    return this.http.get<DepartmentDto>(`${this.apiUrl}/${id}`);
  }

  create(command: CreateDepartmentCommand): Observable<number> {
    return this.http.post<number>(this.apiUrl, command);
  }

  update(command: UpdateDepartmentCommand): Observable<number> {
    return this.http.put<number>(`${this.apiUrl}/${command.deptId}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
