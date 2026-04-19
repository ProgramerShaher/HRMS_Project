import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';

export interface ApplicationRole {
    id: number;
    name: string;
    normalizedName: string;
    description?: string;
}

export interface Permission {
    id: number;
    name: string;
    description: string;
}

export interface CreateRoleDto {
    name: string;
    description?: string;
}

export interface UpdateRoleDto {
    name: string;
    description?: string;
}

export interface Result<T> {
    data: T;
    succeeded: boolean;
    errors: string[];
    message: string;
}

@Injectable({
    providedIn: 'root'
})
export class AccessControlService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiUrl}/access-control`;

    // Roles
    getAllRoles(): Observable<Result<ApplicationRole[]>> {
        return this.http.get<Result<ApplicationRole[]>>(`${this.apiUrl}/roles`);
    }

    getRoleById(id: number): Observable<Result<ApplicationRole>> {
        return this.http.get<Result<ApplicationRole>>(`${this.apiUrl}/roles/${id}`);
    }

    createRole(role: CreateRoleDto): Observable<Result<boolean>> {
        return this.http.post<Result<boolean>>(`${this.apiUrl}/roles`, role);
    }

    updateRole(id: number, role: UpdateRoleDto): Observable<Result<boolean>> {
        return this.http.put<Result<boolean>>(`${this.apiUrl}/roles/${id}`, role);
    }

    deleteRole(id: number): Observable<Result<boolean>> {
        return this.http.delete<Result<boolean>>(`${this.apiUrl}/roles/${id}`);
    }

    // Users
    getAllUsers(): Observable<Result<any[]>> {
        return this.http.get<Result<any[]>>(`${this.apiUrl}/users`);
    }

    createUser(request: any): Observable<Result<any>> {
        // Calling Auth controller register endpoint
        return this.http.post<Result<any>>(`${environment.apiUrl}/Auth/register`, request);
    }

    addUserToRole(userId: number, roleName: string): Observable<Result<boolean>> {
        return this.http.post<Result<boolean>>(`${this.apiUrl}/users/${userId}/roles`, `"${roleName}"`, {
            headers: { 'Content-Type': 'application/json' }
        });
    }

    removeUserFromRole(userId: number, roleName: string): Observable<Result<boolean>> {
        return this.http.delete<Result<boolean>>(`${this.apiUrl}/users/${userId}/roles/${roleName}`);
    }

    // Permissions
    getAllPermissions(): Observable<Result<Permission[]>> {
        return this.http.get<Result<Permission[]>>(`${this.apiUrl}/permissions`);
    }

    getRolePermissions(roleId: number): Observable<Result<string[]>> {
        return this.http.get<Result<string[]>>(`${this.apiUrl}/roles/${roleId}/permissions`);
    }

    updateRolePermissions(roleId: number, permissionIds: number[]): Observable<Result<boolean>> {
        return this.http.put<Result<boolean>>(`${this.apiUrl}/roles/${roleId}/permissions`, permissionIds);
    }
}
