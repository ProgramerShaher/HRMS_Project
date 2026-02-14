import { Routes } from '@angular/router';

export const PERFORMANCE_ROUTES: Routes = [
    {
        path: '',
        loadComponent: () => import('./pages/dashboard/performance-dashboard.component').then(m => m.PerformanceDashboardComponent)
    },
    {
        path: 'violations',
        loadComponent: () => import('./pages/violations/violations.component').then(m => m.ViolationsComponent)
    },
    {
        path: 'appraisals',
        loadComponent: () => import('./pages/appraisals/appraisals.component').then(m => m.AppraisalsComponent)
    }
];
