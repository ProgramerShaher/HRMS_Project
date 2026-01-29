import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { CityDto, CityListDto, CreateCityCommand, UpdateCityCommand } from '../models/city.dto';

@Injectable({
  providedIn: 'root'
})
export class CityService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Cities`;

  getAll(): Observable<CityListDto[]> {
    return this.http.get<CityListDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<CityDto> {
    return this.http.get<CityDto>(`${this.apiUrl}/${id}`);
  }

  create(command: CreateCityCommand): Observable<number> {
    return this.http.post<number>(this.apiUrl, command);
  }

  update(command: UpdateCityCommand): Observable<number> {
    return this.http.put<number>(`${this.apiUrl}/${command.cityId}`, command);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
