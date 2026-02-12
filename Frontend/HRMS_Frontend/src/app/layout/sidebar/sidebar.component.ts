import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DrawerModule } from 'primeng/drawer';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { TooltipModule } from 'primeng/tooltip';
import { LayoutService } from '../../core/services/layout.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, DrawerModule, ButtonModule, RippleModule, TooltipModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
  layoutService = inject(LayoutService);

  menuItems = [
    { label: 'لوحة التحكم', icon: 'pi pi-objects-column', route: '/dashboard' },
    {
      label: 'الحـضـور',
      icon: 'pi pi-id-card',
      expanded: false,
      children: [
        { label: 'لوحة الحضور', icon: 'pi pi-chart-line', route: '/attendance/dashboard' },
        { label: 'تسجيل الحضور', icon: 'pi pi-check-circle', route: '/attendance/punch' },
        { label: 'جهاز البصمة (محاكاة)', icon: 'pi pi-desktop', route: '/attendance/device' },
        { label: 'تقرير التحضير', icon: 'pi pi-list', route: '/attendance/reports' },
        { label: 'جدول دوامي', icon: 'pi pi-calendar', route: '/attendance/my-roster' },
        { label: 'الطلبات', icon: 'pi pi-file-edit', route: '/attendance/requests' }
      ]
    },
    {
      label: 'إدارة الحضور',
      icon: 'pi pi-calendar-plus',
      expanded: false,
      children: [
        { label: 'المناوبات', icon: 'pi pi-clock', route: '/attendance/settings/shifts' },
        { label: 'توزيع الجداول', icon: 'pi pi-th-large', route: '/attendance/settings/roster' },
        { label: 'الموافقات', icon: 'pi pi-check-square', route: '/attendance/settings/approvals' }
      ]
    },
    { label: 'الموظفين', icon: 'pi pi-users', route: '/employees' },
    { label: 'الرواتب', icon: 'pi pi-wallet', route: '/payroll' },
    { 
      label: 'الإجازات', 
      icon: 'pi pi-send', 
      expanded: false,
      children: [
        { label: 'لوحة التحكم', icon: 'pi pi-chart-bar', route: '/leaves/dashboard' },
        { label: 'إجازاتي', icon: 'pi pi-calendar', route: '/leaves/my-leaves' },
        { label: 'الاعتمادات', icon: 'pi pi-check-square', route: '/leaves/approvals' },
        { label: 'سجل الحركات', icon: 'pi pi-history', route: '/leaves/history' },
        { label: 'إعدادات الإجازات', icon: 'pi pi-cog', route: '/leaves/setup' }
      ]
    },
    { label: 'التقارير', icon: 'pi pi-chart-pie', route: '/reports' },
    { 
      label: 'تهيئة النظام', 
      icon: 'pi pi-cog', 
      expanded: false,
      children: [
        { label: 'تهيئة الدول', icon: 'pi pi-flag', route: '/setup/countries' },
        { label: 'تهيئة المدن', icon: 'pi pi-map-marker', route: '/setup/cities' },
        { label: 'تهيئة البنوك', icon: 'pi pi-building', route: '/setup/banks' },
        { label: 'تهيئة الأقسام', icon: 'pi pi-sitemap', route: '/setup/departments' },
        { label: 'الدرجات الوظيفية', icon: 'pi pi-list', route: '/setup/job-grades' },
        { label: 'الوظائف', icon: 'pi pi-briefcase', route: '/setup/jobs' },
        { label: 'أنواع الوثائق', icon: 'pi pi-file', route: '/setup/document-types' },
        { label: 'أنواع الإجازات', icon: 'pi pi-calendar-times', route: '/setup/leave-types' },
        { label: 'سياسات الحضور', icon: 'pi pi-clock', route: '/setup/attendance-policies' },
        { label: 'أنواع الورديات', icon: 'pi pi-moon', route: '/setup/shift-types' },
        { label: 'بنود الراتب', icon: 'pi pi-dollar', route: '/setup/payroll-elements' },
        { label: 'أنواع المخالفات', icon: 'pi pi-exclamation-triangle', route: '/setup/violation-types' },
        { label: 'الإجراءات التأديبية', icon: 'pi pi-shield', route: '/setup/disciplinary-actions' },
        { label: 'مكتبة المؤشرات', icon: 'pi pi-chart-bar', route: '/setup/kpi-library' },
        { label: 'فترات التقييم', icon: 'pi pi-calendar-clock', route: '/setup/appraisal-cycles' }
      ]
    },
  ];

  toggleSubmenu(item: any) {
    if (item.children) {
      item.expanded = !item.expanded;
    }
  }
}
