import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';

export const routes: Routes = [
    {
        path: '',
        component: MainLayoutComponent,
        children: [
            // Dashboard Route
            { path: '', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
            // Future feature lazy loading:
            // { path: 'employees', loadChildren: () => import('./features/employees/employees.routes').then(m => m.EMPLOYEE_ROUTES) }
        ]
    }
];
