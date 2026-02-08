import { Routes } from '@angular/router';

export const leavesRoutes: Routes = [
  {
    path: '',
    redirectTo: 'my-leaves',
    pathMatch: 'full'
  },
  {
    path: 'my-leaves',
    loadComponent: () => import('./pages/my-leaves/my-leaves.component').then(m => m.MyLeavesComponent),
    data: { title: 'إجازاتي' }
  },
  {
    path: 'team-leaves',
    loadComponent: () => import('./pages/team-leaves/team-leaves.component').then(m => m.TeamLeavesComponent),
    data: { title: 'طلبات الفريق' }
  },
  {
    path: 'history',
    loadComponent: () => import('./pages/transaction-history/transaction-history.component').then(m => m.TransactionHistoryComponent),
    data: { title: 'سجل الحركات' }
  }
];
