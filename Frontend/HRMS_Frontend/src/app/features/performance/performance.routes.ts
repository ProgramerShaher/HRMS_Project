import { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/guards/permission.guard';

export const PERFORMANCE_ROUTES: Routes = [
    {
        path: '',
        canActivate: [permissionGuard(['Performance.View'])],
        loadComponent: () => import('./pages/dashboard/performance-dashboard.component').then(m => m.PerformanceDashboardComponent)
    },
    {
        path: 'violations',
        canActivate: [permissionGuard(['Violations.View', 'Performance.View'])],
        loadComponent: () => import('./pages/violations/violations.component').then(m => m.ViolationsComponent)
    },
    {
        path: 'appraisals',
        canActivate: [permissionGuard(['Performance.View', 'Performance.Evaluate'])],
        loadComponent: () => import('./pages/appraisals/appraisals.component').then(m => m.AppraisalsComponent)
    }
];
