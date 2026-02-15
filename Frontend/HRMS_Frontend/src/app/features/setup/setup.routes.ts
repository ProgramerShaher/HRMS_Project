import { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/guards/permission.guard';

export const SETUP_ROUTES: Routes = [
    {
        path: '',
        children: [
            { path: 'departments', canActivate: [permissionGuard(['Setup.Departments'])], loadComponent: () => import('./pages/departments/departments.component').then(m => m.DepartmentsComponent) },
            { path: 'job-grades', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/job-grades/job-grades.component').then(m => m.JobGradesComponent) },
            { path: 'jobs', canActivate: [permissionGuard(['Setup.Jobs'])], loadComponent: () => import('./pages/jobs/jobs.component').then(m => m.JobsComponent) },
            { path: 'countries', canActivate: [permissionGuard(['Setup.Countries'])], loadComponent: () => import('./pages/countries/countries.component').then(m => m.CountriesComponent) },
            { path: 'cities', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/cities/cities.component').then(m => m.CitiesComponent) },
            { path: 'banks', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/banks/banks.component').then(m => m.BanksComponent) },
            { path: 'document-types', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/document-types/document-types.component').then(m => m.DocumentTypesComponent) },
            { path: 'leave-types', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/leave-types/leave-types.component').then(m => m.LeaveTypesComponent) },
            { path: 'attendance-policies', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/attendance-policies/attendance-policies.component').then(m => m.AttendancePoliciesComponent) },
            { path: 'shift-types', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/shift-types/shift-types.component').then(m => m.ShiftTypesComponent) },
            { path: 'payroll-elements', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/payroll-elements/payroll-elements.component').then(m => m.PayrollElementsComponent) },
            { path: 'violation-types', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/violation-types/violation-types.component').then(m => m.ViolationTypesComponent) },
            { path: 'disciplinary-actions', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/disciplinary-actions/disciplinary-actions.component').then(m => m.DisciplinaryActionsComponent) },
            { path: 'kpi-library', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/kpi-library/kpi-library.component').then(m => m.KpiLibraryComponent) },
            { path: 'appraisal-cycles', canActivate: [permissionGuard(['Setup.Manage'])], loadComponent: () => import('./pages/appraisal-cycles/appraisal-cycles.component').then(m => m.AppraisalCyclesComponent) },
            { path: 'access-control', canActivate: [permissionGuard(['System_Admin'])], loadComponent: () => import('./access-control/pages/access-control.component').then(m => m.AccessControlComponent) },

            // Add placeholders for other possible setup pages here or redirect to default
            { path: '', redirectTo: 'departments', pathMatch: 'full' }
        ]
    }
];
