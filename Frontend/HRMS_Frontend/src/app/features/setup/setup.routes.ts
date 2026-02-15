import { Routes } from '@angular/router';

export const SETUP_ROUTES: Routes = [
    {
        path: '',
        children: [
            { path: 'departments', loadComponent: () => import('./pages/departments/departments.component').then(m => m.DepartmentsComponent) },
            { path: 'job-grades', loadComponent: () => import('./pages/job-grades/job-grades.component').then(m => m.JobGradesComponent) },
            { path: 'jobs', loadComponent: () => import('./pages/jobs/jobs.component').then(m => m.JobsComponent) },
            { path: 'countries', loadComponent: () => import('./pages/countries/countries.component').then(m => m.CountriesComponent) },
            { path: 'cities', loadComponent: () => import('./pages/cities/cities.component').then(m => m.CitiesComponent) },
            { path: 'banks', loadComponent: () => import('./pages/banks/banks.component').then(m => m.BanksComponent) },
            { path: 'document-types', loadComponent: () => import('./pages/document-types/document-types.component').then(m => m.DocumentTypesComponent) },
            { path: 'leave-types', loadComponent: () => import('./pages/leave-types/leave-types.component').then(m => m.LeaveTypesComponent) },
            { path: 'attendance-policies', loadComponent: () => import('./pages/attendance-policies/attendance-policies.component').then(m => m.AttendancePoliciesComponent) },
            { path: 'shift-types', loadComponent: () => import('./pages/shift-types/shift-types.component').then(m => m.ShiftTypesComponent) },
            { path: 'payroll-elements', loadComponent: () => import('./pages/payroll-elements/payroll-elements.component').then(m => m.PayrollElementsComponent) },
            { path: 'violation-types', loadComponent: () => import('./pages/violation-types/violation-types.component').then(m => m.ViolationTypesComponent) },
            { path: 'disciplinary-actions', loadComponent: () => import('./pages/disciplinary-actions/disciplinary-actions.component').then(m => m.DisciplinaryActionsComponent) },
            { path: 'kpi-library', loadComponent: () => import('./pages/kpi-library/kpi-library.component').then(m => m.KpiLibraryComponent) },
            { path: 'appraisal-cycles', loadComponent: () => import('./pages/appraisal-cycles/appraisal-cycles.component').then(m => m.AppraisalCyclesComponent) },
            { path: 'access-control', loadComponent: () => import('./access-control/pages/access-control.component').then(m => m.AccessControlComponent) },

            // Add placeholders for other possible setup pages here or redirect to default
            { path: '', redirectTo: 'departments', pathMatch: 'full' }
        ]
    }
];
