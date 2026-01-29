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
    { label: 'سجل الحضور', icon: 'pi pi-calendar-plus', route: '/attendance' },
    { label: 'الموظفين', icon: 'pi pi-users', route: '/employees' },
    { label: 'الرواتب', icon: 'pi pi-wallet', route: '/payroll' },
    { label: 'الإجازات', icon: 'pi pi-send', route: '/leaves' },
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
        { label: 'الدرجات الوظيفية', icon: 'pi pi-list', route: '/setup/job-grades' }
      ]
    },
  ];

  toggleSubmenu(item: any) {
    if (item.children) {
      item.expanded = !item.expanded;
    }
  }
}
