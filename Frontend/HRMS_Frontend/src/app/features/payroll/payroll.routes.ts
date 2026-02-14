import { Routes } from '@angular/router';

export const PAYROLL_ROUTES: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/payroll-dashboard.component').then(m => m.PayrollDashboardComponent),
    title: 'لوحة تحكم الرواتب'
  },
  
  // ═══════════════════════════════════════════════════════════
  // Salaries - رواتب الموظفين
  // ═══════════════════════════════════════════════════════════
  {
    path: 'salaries',
    children: [
      {
        path: 'all',
        loadComponent: () => import('./pages/admin/salaries/all-employees-salaries/all-employees-salaries.component').then(m => m.AllEmployeesSalariesComponent),
        title: 'جميع رواتب الموظفين'
      },
      {
        path: 'breakdown/:id',
        loadComponent: () => import('./pages/admin/salaries/salary-breakdown/salary-breakdown.component').then(m => m.SalaryBreakdownComponent),
        title: 'تفاصيل راتب موظف'
      }
    ]
  },
  
  // ═══════════════════════════════════════════════════════════
  // Loans - السلف
  // ═══════════════════════════════════════════════════════════
  {
    path: 'loans',
    children: [
      // Employee routes
      {
        path: '',
        redirectTo: 'my-loans',
        pathMatch: 'full'
      },
      {
        path: 'my-loans',
        loadComponent: () => import('./pages/loans/my-loans/my-loans.component').then(m => m.MyLoansComponent),
        title: 'سلفي'
      },
      {
        path: 'new',
        loadComponent: () => import('./pages/loans/loan-form/loan-form.component').then(m => m.LoanFormComponent),
        title: 'طلب سلفة جديدة'
      },
      {
        path: 'details/:id',
        loadComponent: () => import('./pages/loans/loan-details/loan-details.component').then(m => m.LoanDetailsComponent),
        title: 'تفاصيل السلفة'
      },
      
      // Admin routes
      {
        path: 'admin/all',
        loadComponent: () => import('./pages/admin/loans/all-loans-admin/all-loans-admin.component').then(m => m.AllLoansAdminComponent),
        title: 'جميع السلف'
      },
      {
        path: 'admin/create',
        loadComponent: () => import('./pages/admin/loans/create-loan-admin/create-loan-admin.component').then(m => m.CreateLoanAdminComponent),
        title: 'إنشاء سلفة'
      },
      {
        path: 'admin/pending',
        loadComponent: () => import('./pages/admin/loans/pending-loans/pending-loans.component').then(m => m.PendingLoansComponent),
        title: 'السلف المعلقة'
      }
    ]
  },
  
  // ═══════════════════════════════════════════════════════════
  // Salary Elements - عناصر الراتب
  // ═══════════════════════════════════════════════════════════
  {
    path: 'elements',
    children: [
      {
        path: '',
        loadComponent: () => import('./pages/admin/elements/salary-elements-list/salary-elements-list.component').then(m => m.SalaryElementsListComponent),
        title: 'عناصر الراتب'
      },
      {
        path: 'form',
        loadComponent: () => import('./pages/admin/elements/salary-element-form/salary-element-form.component').then(m => m.SalaryElementFormComponent),
        title: 'إضافة عنصر راتب'
      },
      {
        path: 'form/:id',
        loadComponent: () => import('./pages/admin/elements/salary-element-form/salary-element-form.component').then(m => m.SalaryElementFormComponent),
        title: 'تعديل عنصر راتب'
      }
    ]
  },
  
  // ═══════════════════════════════════════════════════════════
  // Salary Structures - هياكل الرواتب
  // ═══════════════════════════════════════════════════════════
  {
    path: 'structures',
    children: [
      {
        path: '',
        loadComponent: () => import('./pages/admin/structures/all-structures/all-structures.component').then(m => m.AllStructuresComponent),
        title: 'هياكل الرواتب'
      },
      {
        path: 'employee/:id',
        loadComponent: () => import('./pages/admin/structures/employee-structure/employee-structure.component').then(m => m.EmployeeStructureComponent),
        title: 'هيكل راتب موظف'
      }
    ]
  },
  
  // ═══════════════════════════════════════════════════════════
  // Payroll Processing - معالجة الرواتب
  // ═══════════════════════════════════════════════════════════
  {
    path: 'processing',
    children: [
      {
        path: 'process',
        loadComponent: () => import('./pages/admin/processing/process-payroll/process-payroll.component').then(m => m.ProcessPayrollComponent),
        title: 'معالجة رواتب'
      },
      {
        path: 'runs',
        loadComponent: () => import('./pages/admin/processing/payroll-runs-list/payroll-runs-list.component').then(m => m.PayrollRunsListComponent),
        title: 'مسيرات الرواتب'
      },
      {
        path: 'runs/:id',
        loadComponent: () => import('./pages/admin/processing/payroll-run-details/payroll-run-details.component').then(m => m.PayrollRunDetailsComponent),
        title: 'تفاصيل مسير رواتب'
      }
    ]
  },
  
  // ═══════════════════════════════════════════════════════════
  // Reports - التقارير
  // ═══════════════════════════════════════════════════════════
  {
    path: 'reports',
    children: [
      {
        path: 'monthly',
        loadComponent: () => import('./pages/admin/reports/monthly-summary/monthly-summary-report.component').then(m => m.MonthlySummaryReportComponent),
        title: 'التقرير الشهري'
      },
      {
        path: 'audit',
        loadComponent: () => import('./pages/admin/reports/audit-trail/audit-trail.component').then(m => m.AuditTrailComponent),
        title: 'سجل التتبع'
      }
    ]
  },
  
  // ═══════════════════════════════════════════════════════════
  // Employee routes - مسارات الموظف
  // ═══════════════════════════════════════════════════════════
  {
    path: 'salary',
    children: [
      {
        path: '',
        redirectTo: 'my-structure',
        pathMatch: 'full'
      },
      {
        path: 'my-structure',
        loadComponent: () => import('./pages/salary-structure/my-salary/my-salary.component').then(m => m.MySalaryComponent),
        title: 'هيكل راتبي'
      }
    ]
  },
  {
    path: 'payslips',
    loadComponent: () => import('./pages/payslips/my-payslips/my-payslips.component').then(m => m.MyPayslipsComponent),
    title: 'قسائم رواتبي'
  }
];

