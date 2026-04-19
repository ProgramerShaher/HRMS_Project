import { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/guards/permission.guard';

export const ATTENDANCE_ROUTES: Routes = [
    {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
    },
    {
        path: 'dashboard',
        canActivate: [permissionGuard(['Attendance.View'])],
        loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent)
    },
    {
        path: 'my-roster',
        loadComponent: () => import('./my-roster/my-roster.component').then(m => m.MyRosterComponent)
    },
    {
        path: 'reports',
        canActivate: [permissionGuard(['Attendance.Reports', 'Attendance.ViewReports'])],
        loadComponent: () => import('./reports/attendance-reports.component').then(m => m.AttendanceReportsComponent)
    },
    {
        path: 'punch',
        canActivate: [permissionGuard(['Attendance.Punch'])],
        loadComponent: () => import('./punch/punch.component').then(m => m.PunchComponent)
    },
    {
        path: 'device',
        canActivate: [permissionGuard(['Attendance.Device'])],
        loadComponent: () => import('./punch/device-punch/device-punch.component').then(m => m.DevicePunchComponent)
    },
    {
        path: 'requests',
        loadComponent: () => import('./requests/requests.component').then(m => m.RequestsComponent)
    },
    {
        path: 'settings',
        canActivate: [permissionGuard(['Attendance.Manage', 'Setup.Manage'])],
        children: [
            { path: 'shifts', loadComponent: () => import('./settings/shift-management/shift-management.component').then(m => m.ShiftManagementComponent) },
            { path: 'roster', loadComponent: () => import('./settings/roster-management/roster-management.component').then(m => m.RosterManagementComponent) },
            { path: 'approvals', canActivate: [permissionGuard(['Attendance.Approve'])], loadComponent: () => import('./settings/approvals/approvals.component').then(m => m.ApprovalsComponent) }
        ]
    }
];
