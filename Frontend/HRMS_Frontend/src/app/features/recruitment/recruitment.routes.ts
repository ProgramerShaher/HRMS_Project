import { Routes } from '@angular/router';

export const RECRUITMENT_ROUTES: Routes = [
  {
    path: 'vacancies',
    loadComponent: () => import('./pages/vacancies/vacancies-list/vacancies-list.component').then(c => c.VacanciesListComponent),
    title: 'الوظائف الشاغرة'
  },
  {
    path: 'candidates',
    loadComponent: () => import('./pages/candidates/candidates-list/candidates-list.component').then(c => c.CandidatesListComponent),
    title: 'بنك المرشحين'
  },
  {
    path: 'applications',
    loadComponent: () => import('./pages/applications/applications-list/applications-list.component').then(c => c.ApplicationsListComponent),
    title: 'طلبات التوظيف'
  },
  {
    path: 'interviews',
    loadComponent: () => import('./pages/interviews/interviews-list/interviews-list.component').then(c => c.InterviewsListComponent),
    title: 'المقابلات'
  },
  {
    path: 'offers',
    loadComponent: () => import('./pages/offers/offers-list/offers-list.component').then(c => c.OffersListComponent),
    title: 'عروض العمل'
  },
  {
    path: 'reports',
    loadComponent: () => import('./pages/reports/recruitment-reports.component').then(c => c.RecruitmentReportsComponent),
    title: 'خرائط وتقارير التوظيف'
  },
  {
    path: '',
    redirectTo: 'vacancies',
    pathMatch: 'full'
  }
];
