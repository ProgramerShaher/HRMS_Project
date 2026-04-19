import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { PermissionService } from '../services/permission.service';

/**
 * Permission Guard - للتحكم في الوصول للصفحات بناءً على الصلاحيات
 * @param requiredPermissions الصلاحيات المطلوبة للوصول
 * @returns CanActivateFn
 * 
 * @example
 * // في routes
 * {
 *   path: 'users',
 *   canActivate: [authGuard, permissionGuard(['Users.View'])],
 *   component: UsersListComponent
 * }
 */
export const permissionGuard = (requiredPermissions: string[]): CanActivateFn => {
    return (route, state) => {
        const permissionService = inject(PermissionService);
        const router = inject(Router);

        // Check if user has any of the required permissions
        if (permissionService.hasAnyPermission(requiredPermissions)) {
            return true;
        }

        // Redirect to unauthorized page
        console.warn(`Access denied. Required permissions: ${requiredPermissions.join(', ')}`);
        return router.createUrlTree(['/unauthorized']);
    };
};

/**
 * Role Guard - للتحكم في الوصول للصفحات بناءً على الأدوار
 * @param allowedRoles الأدوار المسموح لها بالوصول
 * @returns CanActivateFn
 * 
 * @example
 * // في routes
 * {
 *   path: 'admin',
 *   canActivate: [authGuard, roleGuard(['System_Admin', 'HR_Manager'])],
 *   component: AdminComponent
 * }
 */
export const roleGuard = (allowedRoles: string[]): CanActivateFn => {
    return (route, state) => {
        const permissionService = inject(PermissionService);
        const router = inject(Router);

        // Check if user has any of the allowed roles
        if (permissionService.hasAnyRole(allowedRoles)) {
            return true;
        }

        // Redirect to unauthorized page
        console.warn(`Access denied. Required roles: ${allowedRoles.join(', ')}`);
        return router.createUrlTree(['/unauthorized']);
    };
};
