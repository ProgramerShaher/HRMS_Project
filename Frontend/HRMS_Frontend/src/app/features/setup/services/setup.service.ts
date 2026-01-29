import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class SetupService {
  private http = inject(HttpClient);
  // Assuming environment.apiUrl is defined, e.g., 'https://api.hrms.com/api'
  // If not, we can hardcode a base or use a relative path.
  // Using a fallback for now if environment is not fully set up in this context.
  private apiUrl = environment.apiUrl;

  constructor() {}

  // Generic Get All
  getAll<T>(endpoint: string): Observable<T[]> {
    return this.http.get<T[]>(`${this.apiUrl}/${endpoint}`);
  }

  // Generic Get By ID
  getById<T>(endpoint: string, id: number): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}/${endpoint}/${id}`);
  }

  // Generic Create
  create<T>(endpoint: string, data: Partial<T>): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}/${endpoint}`, data);
  }

  // Generic Update
  update<T>(endpoint: string, id: number, data: Partial<T>): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}/${endpoint}/${id}`, data);
  }

  // Generic Delete
  delete(endpoint: string, id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${endpoint}/${id}`);
  }
}
