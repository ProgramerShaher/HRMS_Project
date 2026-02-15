import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../core/auth/services/auth.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DrawerModule } from 'primeng/drawer';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { TooltipModule } from 'primeng/tooltip';
import { LayoutService } from '../../core/services/layout.service';

import { PermissionService } from '../../core/auth/services/permission.service';
import { HasPermissionDirective } from '../../shared/directives/permission.directives';

interface MenuItem {
  label: string;
  icon?: string;
  route?: string;
  permission?: string;
  expanded?: boolean;
  children?: MenuItem[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, DrawerModule, ButtonModule, RippleModule, TooltipModule, HasPermissionDirective],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {
  layoutService = inject(LayoutService);
  permissionService = inject(PermissionService);
  private authService = inject(AuthService); // Inject AuthService for debugging

  ngOnInit() {
    const user = this.authService.currentUser();
    console.log('Sidebar Init - Current User:', user);
    console.log('Roles:', user?.roles);
    console.log('Permissions:', user?.permissions);
    console.log('Is System Admin:', this.permissionService.isSystemAdmin());
  }

  menuItems: MenuItem[] = [
    { label: 'الرئيسية', icon: 'pi pi-home', route: '/dashboard' },
    {
      label: 'الموظفين',
      icon: 'pi pi-users',
      expanded: false,
      permission: 'Employees.View',
      children: [
        { label: 'قائمة الموظفين', icon: 'pi pi-list', route: '/personnel/employees', permission: 'Employees.View' },
        { label: 'إضافة موظف جديد', icon: 'pi pi-user-plus', route: '/personnel/employees/wizard', permission: 'Employees.Create' },
        { label: 'إدارة السلف', icon: 'pi pi-money-bill', route: '/payroll/loans', permission: 'Loans.View' },
      ]
    },
    {
      label: 'الحـضـور',
      icon: 'pi pi-id-card',
      expanded: false,
      permission: 'Attendance.View',
      children: [
        { label: 'لوحة الحضور', icon: 'pi pi-chart-line', route: '/attendance/dashboard', permission: 'Attendance.View' },
        { label: 'تسجيل الحضور', icon: 'pi pi-check-circle', route: '/attendance/punch', permission: 'Attendance.Punch' },
        { label: 'جهاز البصمة (محاكاة)', icon: 'pi pi-desktop', route: '/attendance/device', permission: 'Attendance.Device' },
        { label: 'تقرير التحضير', icon: 'pi pi-list', route: '/attendance/reports', permission: 'Attendance.Reports' },
        { label: 'جدول دوامي', icon: 'pi pi-calendar', route: '/attendance/my-roster' },
        { label: 'الطلبات', icon: 'pi pi-file-edit', route: '/attendance/requests' }
      ]
    },
    {
      label: 'إدارة الحضور',
      icon: 'pi pi-calendar-plus',
      expanded: false,
      permission: 'Attendance.Manage',
      children: [
        { label: 'المناوبات', icon: 'pi pi-clock', route: '/attendance/settings/shifts', permission: 'Attendance.Manage' },
        { label: 'توزيع الجداول', icon: 'pi pi-th-large', route: '/attendance/settings/roster', permission: 'Attendance.Manage' },
        { label: 'الموافقات', icon: 'pi pi-check-square', route: '/attendance/settings/approvals', permission: 'Attendance.Approve' }
      ]
    },
    {
      label: 'الرواتب المحاسبية',
      icon: 'pi pi-calculator',
      expanded: false,
      permission: 'Payroll.View',
      children: [
        { label: 'لوحة التحكم', icon: 'pi pi-chart-bar', route: '/payroll/dashboard', permission: 'Payroll.View' },
        {
          label: 'إدارة العناصر',
          icon: 'pi pi-list',
          expanded: false,
          permission: 'Payroll.Edit',
          children: [
            { label: 'قائمة العناصر', icon: 'pi pi-list', route: '/payroll/elements', permission: 'Payroll.Edit' },
            { label: 'إضافة عنصر', icon: 'pi pi-plus', route: '/payroll/elements/form', permission: 'Payroll.Edit' }
          ]
        },
        {
          label: 'هياكل الرواتب',
          icon: 'pi pi-sitemap',
          expanded: false,
          permission: 'Payroll.View',
          children: [
            { label: 'جميع الهياكل', icon: 'pi pi-list', route: '/payroll/structures', permission: 'Payroll.View' },
            { label: 'هيكل راتبي', icon: 'pi pi-user', route: '/payroll/salary/my-structure' }
          ]
        },
        {
          label: 'معالجة الرواتب',
          icon: 'pi pi-cog',
          expanded: false,
          permission: 'Payroll.Process',
          children: [
            { label: 'معالجة شهر جديد', icon: 'pi pi-play', route: '/payroll/processing/process', permission: 'Payroll.Process' },
            { label: 'مسيرات الرواتب', icon: 'pi pi-list', route: '/payroll/processing/runs', permission: 'Payroll.View' }
          ]
        },
        {
          label: 'التقارير',
          icon: 'pi pi-chart-bar',
          expanded: false,
          permission: 'Reports.View',
          children: [
            { label: 'ملخص شهري', icon: 'pi pi-calendar', route: '/payroll/reports/monthly', permission: 'Reports.View' },
            { label: 'سجل التتبع', icon: 'pi pi-history', route: '/payroll/reports/audit', permission: 'Reports.Advanced' }
          ]
        },
        { label: 'قسائم رواتبي', icon: 'pi pi-file-pdf', route: '/payroll/payslips' }
      ]
    },
    {
      label: 'الإجازات',
      icon: 'pi pi-send',
      expanded: false,
      permission: 'Leaves.View',
      children: [
        { label: 'لوحة التحكم', icon: 'pi pi-chart-bar', route: '/leaves/dashboard', permission: 'Leaves.View' },
        { label: 'إجازاتي', icon: 'pi pi-calendar', route: '/leaves/my-leaves' },
        { label: 'الاعتمادات', icon: 'pi pi-check-square', route: '/leaves/approvals', permission: 'Leaves.Approve' },
        { label: 'سجل الحركات', icon: 'pi pi-history', route: '/leaves/history', permission: 'Leaves.View' },
        { label: 'إعدادات الإجازات', icon: 'pi pi-cog', route: '/leaves/setup', permission: 'Leaves.Manage' }
      ]
    },
    {
      label: 'الأداء والتقييم',
      icon: 'pi pi-chart-line',
      expanded: false,
      permission: 'Performance.View',
      children: [
        { label: 'لوحة التحكم', icon: 'pi pi-home', route: '/performance', permission: 'Performance.View' },
        { label: 'المخالفات والجزاءات', icon: 'pi pi-exclamation-triangle', route: '/performance/violations', permission: 'Violations.View' },
        { label: 'تقييم الأداء', icon: 'pi pi-star', route: '/performance/appraisals', permission: 'Performance.Evaluate' }
      ]
    },
    { label: 'التقارير', icon: 'pi pi-chart-pie', route: '/reports', permission: 'Reports.View' },
    {
      label: 'تهيئة النظام',
      icon: 'pi pi-cog',
      expanded: false,
      permission: 'System_Admin',
      children: [
        { label: 'الصلاحيات والمستخدمين', icon: 'pi pi-key', route: '/setup/access-control', permission: 'System_Admin' },
        { label: 'تهيئة الدول', icon: 'pi pi-flag', route: '/setup/countries', permission: 'Setup.Countries' },
        { label: 'تهيئة المدن', icon: 'pi pi-map-marker', route: '/setup/cities', permission: 'Setup.Manage' },
        { label: 'تهيئة البنوك', icon: 'pi pi-building', route: '/setup/banks', permission: 'Setup.Manage' },
        { label: 'تهيئة الأقسام', icon: 'pi pi-sitemap', route: '/setup/departments', permission: 'Setup.Departments' },
        { label: 'الدرجات الوظيفية', icon: 'pi pi-list', route: '/setup/job-grades', permission: 'Setup.Manage' },
        { label: 'الوظائف', icon: 'pi pi-briefcase', route: '/setup/jobs', permission: 'Setup.Jobs' },
        { label: 'أنواع الوثائق', icon: 'pi pi-file', route: '/setup/document-types', permission: 'Setup.Manage' },
        { label: 'أنواع الإجازات', icon: 'pi pi-calendar-times', route: '/setup/leave-types', permission: 'Setup.Manage' },
        { label: 'سياسات الحضور', icon: 'pi pi-clock', route: '/setup/attendance-policies', permission: 'Setup.Manage' },
        { label: 'أنواع الورديات', icon: 'pi pi-moon', route: '/setup/shift-types', permission: 'Setup.Manage' },
        { label: 'بنود الراتب', icon: 'pi pi-dollar', route: '/setup/payroll-elements', permission: 'Setup.Manage' },
        { label: 'أنواع المخالفات', icon: 'pi pi-exclamation-triangle', route: '/setup/violation-types', permission: 'Setup.Manage' },
        { label: 'الإجراءات التأديبية', icon: 'pi pi-shield', route: '/setup/disciplinary-actions', permission: 'Setup.Manage' },
        { label: 'مكتبة المؤشرات', icon: 'pi pi-chart-bar', route: '/setup/kpi-library', permission: 'Setup.Manage' },
        { label: 'فترات التقييم', icon: 'pi pi-calendar-clock', route: '/setup/appraisal-cycles', permission: 'Setup.Manage' }
      ]
    },
  ];

  toggleSubmenu(item: any) {
    if (item.children) {
      item.expanded = !item.expanded;
    }
  }
}
