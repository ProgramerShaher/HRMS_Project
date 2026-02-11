import { Routes } from '@angular/router';

export const ATTENDANCE_ROUTES: Routes = [
    {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
    },
    {
        path: 'dashboard',
        loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent)
    },
    {
        path: 'my-roster',
        loadComponent: () => import('./my-roster/my-roster.component').then(m => m.MyRosterComponent)
    },
    {
        path: 'punch',
        loadComponent: () => import('./punch/punch.component').then(m => m.PunchComponent)
    },
    {
        path: 'requests',
        loadComponent: () => import('./requests/requests.component').then(m => m.RequestsComponent)
    },
    {
        path: 'settings',
        children: [
            { path: 'shifts', loadComponent: () => import('./settings/shift-management/shift-management.component').then(m => m.ShiftManagementComponent) },
            { path: 'roster', loadComponent: () => import('./settings/roster-management/roster-management.component').then(m => m.RosterManagementComponent) },
            { path: 'approvals', loadComponent: () => import('./settings/approvals/approvals.component').then(m => m.ApprovalsComponent) }
        ]
    }
];
