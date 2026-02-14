import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, shareReplay, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

/**
 * Lookup/Reference Data Models
 */
export interface Department {
  deptId: number;
  deptNameAr: string;
  deptNameEn: string;
  isActive: boolean;
}

export interface Job {
  jobId: number;
  jobTitleAr: string;
  jobTitleEn: string;
  departmentId?: number;
  jobGradeId?: number;
  isActive: boolean;
}

export interface JobGrade {
  jobGradeId: number;
  gradeNameAr: string;
  gradeNameEn: string;
  gradeLevel: number;
}

export interface Bank {
  bankId: number;
  bankNameAr: string;
  bankNameEn: string;
  swiftCode?: string;
}

export interface Country {
  countryId: number;
  countryNameAr: string;
  countryNameEn: string;
  countryCode: string;
}

export interface City {
  cityId: number;
  countryId: number;
  cityNameAr: string;
  cityNameEn: string;
}

export interface DocumentType {
  documentTypeId: number;
  documentTypeNameAr: string;
  documentTypeNameEn: string;
}

export interface EmployeeListItem {
  employeeId: number;
  employeeNumber: string;
  fullNameAr: string;
  fullNameEn: string;
  deptNameAr?: string;
  jobTitleAr?: string;
}

/**
 * Centralized Lookup Service
 * Fetches all reference/lookup data from Setup module with caching
 */
@Injectable({
  providedIn: 'root'
})
export class LookupService {
  private apiUrl = environment.apiUrl;
  private cache = new Map<string, { data: Observable<any>, timestamp: number }>();
  private cacheDuration = 5 * 60 * 1000; // 5 minutes

  constructor(private http: HttpClient) {}

  /**
   * Get all departments
   */
  getDepartments(): Observable<Department[]> {
    return this.getCachedData<Department[]>(
      'departments',
      `${this.apiUrl}/Departments`
    );
  }

  /**
   * Get all jobs
   */
  getJobs(): Observable<Job[]> {
    return this.getCachedData<Job[]>(
      'jobs',
      `${this.apiUrl}/Jobs`
    );
  }

  /**
   * Get job grades
   */
  getJobGrades(): Observable<JobGrade[]> {
    return this.getCachedData<JobGrade[]>(
      'jobGrades',
      `${this.apiUrl}/JobGrades`
    );
  }

  /**
   * Get all banks
   */
  getBanks(): Observable<Bank[]> {
    return this.getCachedData<Bank[]>(
      'banks',
      `${this.apiUrl}/Banks`
    );
  }

  /**
   * Get all countries/nationalities
   */
  getCountries(): Observable<Country[]> {
    return this.getCachedData<Country[]>(
      'countries',
      `${this.apiUrl}/Countries`
    );
  }

  /**
   * Get cities (optionally filtered by country)
   */
  getCities(countryId?: number): Observable<City[]> {
    const url = countryId 
      ? `${this.apiUrl}/Cities/by-country/${countryId}` 
      : `${this.apiUrl}/Cities`;
    const cacheKey = countryId ? `cities_${countryId}` : 'cities_all';
    
    return this.getCachedData<City[]>(cacheKey, url);
  }

  /**
   * Get document types
   */
  getDocumentTypes(): Observable<DocumentType[]> {
    return this.getCachedData<DocumentType[]>(
      'documentTypes',
      `${this.apiUrl}/DocumentTypes`
    );
  }

  /**
   * Get active employees (for manager selection)
   */
  getActiveEmployees(): Observable<EmployeeListItem[]> {
    return this.http.get<any>(`${this.apiUrl}/Employees?pageSize=1000&isActive=true`)
      .pipe(
        tap(response => console.log('Active Employees raw:', response)),
        map(response => this.mapResponse(response))
      );
  }

  /**
   * Generic cached data fetcher
   */
  private getCachedData<T>(key: string, url: string): Observable<T> {
    const cached = this.cache.get(key);
    const now = Date.now();

    if (cached && (now - cached.timestamp) < this.cacheDuration) {
      return cached.data as Observable<T>;
    }

    const data$ = this.http.get<any>(url).pipe(
      tap(response => console.log(`Lookup data for ${key}:`, response)),
      map(response => this.mapResponse(response) as any as T),
      shareReplay(1),
      tap(() => console.log(`Loaded ${key} from API`))
    );

    this.cache.set(key, { data: data$, timestamp: now });
    return data$;
  }

  private mapResponse(response: any): any[] {
    // Handle different response formats
    const root = response?.result || response?.data || response;
    
    if (root?.items && Array.isArray(root.items)) return root.items;
    
    if (Array.isArray(root)) return root;
    
    const firstArrayProp = Object.values(response || {}).find(val => Array.isArray(val)) as any[];
    if (firstArrayProp) return firstArrayProp;
    
    return [];
  }

  /**
   * Clear all cached data
   */
  clearCache(): void {
    this.cache.clear();
    console.log('Lookup cache cleared');
  }

  /**
   * Clear specific cached item
   */
  clearCacheItem(key: string): void {
    this.cache.delete(key);
    console.log(`Cleared cache for: ${key}`);
  }

  /**
   * Refresh specific cached item
   */
  refreshCacheItem(key: string): void {
    this.clearCacheItem(key);
  }
}
