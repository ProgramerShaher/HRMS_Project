import { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/guards/permission.guard';

export const leavesRoutes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    canActivate: [permissionGuard(['Leaves.View'])],
    loadComponent: () => import('./pages/dashboard/leave-dashboard.component').then(m => m.LeaveDashboardComponent),
    data: { title: 'لوحة التحكم' }
  },
  {
    path: 'my-leaves',
    loadComponent: () => import('./pages/my-leaves/my-leaves.component').then(m => m.MyLeavesComponent),
    data: { title: 'إجازاتي' }
  },
  {
    path: 'approvals',
    canActivate: [permissionGuard(['Leaves.Approve'])],
    loadComponent: () => import('./pages/approvals/approvals.component').then(m => m.ApprovalsComponent),
    data: { title: 'الاعتمادات' }
  },
  {
    path: 'history',
    canActivate: [permissionGuard(['Leaves.View'])],
    loadComponent: () => import('./pages/transaction-history/transaction-history.component').then(m => m.TransactionHistoryComponent),
    data: { title: 'سجل الحركات' }
  },
  {
    path: 'setup',
    canActivate: [permissionGuard(['Leaves.Manage'])],
    loadComponent: () => import('./pages/setup/leave-setup.component').then(m => m.LeaveSetupComponent),
    data: { title: 'الإعدادات' }
  }
];
