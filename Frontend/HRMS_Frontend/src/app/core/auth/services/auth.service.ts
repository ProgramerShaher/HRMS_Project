import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.dto';
import { ApiResponse } from '../../models/api-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = `${environment.apiUrl}/Auth`;
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'auth_user';

  // State Signals
  private currentUserSignal = signal<AuthResponse | null>(this.getUserFromStorage());
  
  // Computed Signals
  public currentUser = computed(() => this.currentUserSignal());
  public isAuthenticated = computed(() => !!this.currentUserSignal());

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/login`, request).pipe(
      map(response => {
        if (!response.succeeded || !response.data) {
            throw new Error(response.message || 'Login failed');
        }
        return this.mapToAuthResponse(response.data);
      }),
      tap(response => this.handleAuthSuccess(response))
    );
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/register`, request).pipe(
      map(response => {
        if (!response.succeeded || !response.data) {
            throw new Error(response.message || 'Registration failed');
        }
        return this.mapToAuthResponse(response.data);
      }),
      tap(response => this.handleAuthSuccess(response))
    );
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSignal.set(null);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private handleAuthSuccess(response: AuthResponse) {
    if (!response.token) {
        console.error('Login successful but no token received!', response);
        return;
    }
    localStorage.setItem(this.TOKEN_KEY, response.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(response));
    this.currentUserSignal.set(response);
  }

  private getUserFromStorage(): AuthResponse | null {
    const userStr = localStorage.getItem(this.USER_KEY);
    return userStr ? JSON.parse(userStr) : null;
  }

  // Helper to handle PascalCase from .NET
  private mapToAuthResponse(data: any): AuthResponse {
    return {
        userId: data.userId || data.UserId,
        userName: data.userName || data.UserName,
        email: data.email || data.Email,
        fullName: data.fullName || data.FullName || data.FullNameAr, // Fallback
        token: data.token || data.Token,
        refreshToken: data.refreshToken || data.RefreshToken,
        tokenExpiration: data.tokenExpiration || data.TokenExpiration,
        roles: data.roles || data.Roles || [],
        employeeId: data.employeeId || data.EmployeeId
    };
  }
}
