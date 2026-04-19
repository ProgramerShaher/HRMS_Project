import { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/guards/permission.guard';

export const leavesRoutes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  // ─── لوحة التحكم الإدارية للإجازات ───────────────────────────────
  {
    path: 'dashboard',
    canActivate: [permissionGuard(['Leaves.View'])],
    loadComponent: () => import('./pages/dashboard/leave-dashboard.component').then(m => m.LeaveDashboardComponent),
    data: { title: 'لوحة تحكم الإجازات' }
  },
  // ─── اعتمادات الطلبات المعلقة ─────────────────────────────────────
  {
    path: 'approvals',
    canActivate: [permissionGuard(['Leaves.Approve'])],
    loadComponent: () => import('./pages/approvals/approvals.component').then(m => m.ApprovalsComponent),
    data: { title: 'الاعتمادات' }
  },
  // ─── جميع طلبات الإجازة (إدارية) ──────────────────────────────────
  {
    path: 'all-requests',
    canActivate: [permissionGuard(['Leaves.View'])],
    loadComponent: () => import('./pages/all-requests/all-requests.component').then(m => m.AllRequestsComponent),
    data: { title: 'جميع طلبات الإجازة' }
  },
  // ─── أرصدة الموظفين ───────────────────────────────────────────────
  {
    path: 'employee-balances',
    canActivate: [permissionGuard(['Leaves.Manage'])],
    loadComponent: () => import('./pages/employee-balances/employee-balances.component').then(m => m.EmployeeBalancesComponent),
    data: { title: 'أرصدة الموظفين' }
  },
  // ─── سجل حركات الإجازات ───────────────────────────────────────────
  {
    path: 'history',
    canActivate: [permissionGuard(['Leaves.View'])],
    loadComponent: () => import('./pages/transaction-history/transaction-history.component').then(m => m.TransactionHistoryComponent),
    data: { title: 'سجل الحركات' }
  },
  // ─── إعدادات أنواع الإجازات والعطل ───────────────────────────────
  {
    path: 'setup',
    canActivate: [permissionGuard(['Leaves.Manage'])],
    loadComponent: () => import('./pages/setup/leave-setup.component').then(m => m.LeaveSetupComponent),
    data: { title: 'الإعدادات' }
  }
];
