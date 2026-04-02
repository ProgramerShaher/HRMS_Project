import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ComprehensiveDashboardDto } from '../models/reports.models';
import { ApiResponse } from '../models/api-response';

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  private apiUrl = `${environment.apiUrl}/Reports`;

  constructor(private http: HttpClient) { }

  getComprehensiveDashboard(): Observable<ApiResponse<ComprehensiveDashboardDto>> {
    return this.http.get<ApiResponse<ComprehensiveDashboardDto>>(`${this.apiUrl}/dashboard/comprehensive`);
  }
}
