import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ComprehensiveDashboardDto } from '../models/reports.models';

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  private apiUrl = `${environment.apiUrl}/Reports`;

  constructor(private http: HttpClient) { }

  getComprehensiveDashboard(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/dashboard/comprehensive`);
  }
}
