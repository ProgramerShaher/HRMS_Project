

import { Injectable, inject, computed } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root'
})
export class PermissionService {
    private authService = inject(AuthService);

    /**
     * Get current user's permissions
     */
    private get permissions(): string[] {
        return this.authService.currentUser()?.permissions || [];
    }

    /**
     * Get current user's roles
     */
    private get roles(): string[] {
        return this.authService.currentUser()?.roles || [];
    }

    /**
     * Check if user has a specific permission
     */
    hasPermission(permission: string): boolean {
        if (!this.authService.isAuthenticated()) {
            return false;
        }

        // System Admin has all permissions
        if (this.isSystemAdmin()) {
            return true;
        }

        // Check if user has the permission directly, or has a role with that name (convenience)
        return this.permissions.includes(permission) || this.roles.includes(permission);
    }

    /**
     * Check if user has any of the specified permissions
     */
    hasAnyPermission(permissions: string[]): boolean {
        if (!this.authService.isAuthenticated() || !permissions || permissions.length === 0) {
            return false;
        }

        // System Admin has all permissions
        if (this.isSystemAdmin()) {
            return true;
        }

        return permissions.some(permission => this.permissions.includes(permission) || this.roles.includes(permission));
    }

    /**
     * Check if user has all of the specified permissions
     */
    hasAllPermissions(permissions: string[]): boolean {
        if (!this.authService.isAuthenticated() || !permissions || permissions.length === 0) {
            return false;
        }

        // System Admin has all permissions
        if (this.isSystemAdmin()) {
            return true;
        }

        return permissions.every(permission => this.permissions.includes(permission) || this.roles.includes(permission));
    }

    /**
     * Check if user has a specific role
     */
    hasRole(role: string): boolean {
        if (!this.authService.isAuthenticated()) {
            return false;
        }
        return this.roles.includes(role);
    }

    /**
     * Check if user has any of the specified roles
     */
    hasAnyRole(roles: string[]): boolean {
        if (!this.authService.isAuthenticated() || !roles || roles.length === 0) {
            return false;
        }
        return roles.some(role => this.roles.includes(role));
    }

    /**
     * Check if user is System Admin
     */
    isSystemAdmin(): boolean {
        return this.hasRole('System_Admin');
    }

    /**
     * Check if user is HR Manager
     */
    isHRManager(): boolean {
        return this.hasRole('HR_Manager');
    }

    /**
     * Get all permissions grouped by module
     */
    getPermissionsByModule(): Map<string, string[]> {
        const grouped = new Map<string, string[]>();

        this.permissions.forEach(permission => {
            const module = permission.split('.')[0];
            if (!grouped.has(module)) {
                grouped.set(module, []);
            }
            grouped.get(module)!.push(permission);
        });

        return grouped;
    }

    /**
     * Check if user can access a specific route based on permissions
     */
    canAccessRoute(requiredPermissions: string[]): boolean {
        if (!requiredPermissions || requiredPermissions.length === 0) {
            return true; // No permissions required
        }
        return this.hasAnyPermission(requiredPermissions);
    }
}
