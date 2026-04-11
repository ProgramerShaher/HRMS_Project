import { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/guards/permission.guard';

export const PERFORMANCE_ROUTES: Routes = [
    // ── Default: Performance Dashboard ────────────────────────────────────────
    {
        path: '',
        canActivate: [permissionGuard(['Performance.View'])],
        loadComponent: () => import('./pages/dashboard/performance-dashboard.component')
            .then(m => m.PerformanceDashboardComponent)
    },

    // ── KPI Settings (HR_Manager / System_Admin only) ─────────────────────────
    {
        path: 'kpi-settings',
        canActivate: [permissionGuard(['Performance.Configure'])],
        loadComponent: () => import('./pages/kpi-settings/kpi-settings.component')
            .then(m => m.KpiSettingsComponent)
    },

    // ── Evaluation Cycles ─────────────────────────────────────────────────────
    {
        path: 'cycles',
        canActivate: [permissionGuard(['Performance.Configure'])],
        loadComponent: () => import('./pages/evaluation-cycles/evaluation-cycles.component')
            .then(m => m.EvaluationCyclesComponent)
    },

    // ── My Appraisals (Employee Self-View) ────────────────────────────────────
    {
        path: 'my-appraisals',
        loadComponent: () => import('./pages/my-appraisals/my-appraisals.component')
            .then(m => m.MyAppraisalsComponent)
    },

    // ── Team Appraisals (Manager View) ────────────────────────────────────────
    {
        path: 'team-appraisals',
        canActivate: [permissionGuard(['Performance.View'])],
        loadComponent: () => import('./pages/team-appraisals/team-appraisals.component')
            .then(m => m.TeamAppraisalsComponent)
    },

    // ── HR Appraisals Management (Initiate + Finalize) ────────────────────────
    {
        path: 'appraisals',
        canActivate: [permissionGuard(['Performance.View', 'Performance.Evaluate'])],
        loadComponent: () => import('./pages/appraisals/appraisals.component')
            .then(m => m.AppraisalsComponent)
    },

    // ── Individual Appraisal Result Dashboard ─────────────────────────────────
    {
        path: 'appraisals/:id/result',
        canActivate: [permissionGuard(['Performance.View'])],
        loadComponent: () => import('./pages/appraisal-result/appraisal-result.component')
            .then(m => m.AppraisalResultComponent)
    },

    // ── Violations ────────────────────────────────────────────────────────────
    {
        path: 'violations',
        canActivate: [permissionGuard(['Violations.View', 'Performance.View'])],
        loadComponent: () => import('./pages/violations/violations.component')
            .then(m => m.ViolationsComponent)
    },

    // ── Analytics ─────────────────────────────────────────────────────────────
    {
        path: 'analytics',
        canActivate: [permissionGuard(['Performance.View'])],
        loadComponent: () => import('./pages/analytics/analytics.component')
            .then(m => m.PerformanceAnalyticsComponent)
    }
];
