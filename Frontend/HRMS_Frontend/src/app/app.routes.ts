import { Routes } from '@angular/router';
import { authGuard } from './core/auth/guards/auth.guard';

export const routes: Routes = [
    {
        path: 'auth',
        children: [
            { path: 'login', loadComponent: () => import('./features/auth/pages/login/login.component').then(m => m.LoginComponent) },
            { path: 'register', loadComponent: () => import('./features/auth/pages/register/register.component').then(m => m.RegisterComponent) },
            { path: 'forgot-password', loadComponent: () => import('./features/auth/pages/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent) },
            { path: '', redirectTo: 'login', pathMatch: 'full' }
        ]
    },
    // Protected Dashboard Routes
    { path: 'dashboard', canActivate: [authGuard], loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    { path: 'setup', canActivate: [authGuard], loadChildren: () => import('./features/setup/setup.routes').then(m => m.SETUP_ROUTES) },
    { path: 'employees', canActivate: [authGuard], loadChildren: () => import('./features/personnel/personnel.routes').then(m => m.PERSONNEL_ROUTES) },
    { path: 'leaves', canActivate: [authGuard], loadChildren: () => import('./features/leaves/leaves.routes').then(m => m.leavesRoutes) },
    { path: 'attendance', canActivate: [authGuard], loadChildren: () => import('./features/attendance/attendance.routes').then(m => m.ATTENDANCE_ROUTES) },
    { path: 'payroll', canActivate: [authGuard], loadChildren: () => import('./features/payroll/payroll.routes').then(m => m.PAYROLL_ROUTES) },
    { path: 'performance', canActivate: [authGuard], loadChildren: () => import('./features/performance/performance.routes').then(m => m.PERFORMANCE_ROUTES) },
];
