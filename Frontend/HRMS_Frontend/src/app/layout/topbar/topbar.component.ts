import { Component, inject, computed, ViewChild, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { AvatarModule } from 'primeng/avatar';
import { BadgeModule } from 'primeng/badge';
import { TooltipModule } from 'primeng/tooltip';
import { MenuModule, Menu } from 'primeng/menu';
import { LayoutService } from '../../core/services/layout.service'; 
import { ThemeService } from '../../core/services/theme.service';
import { AuthService } from '../../core/auth/services/auth.service';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [
    CommonModule, 
    ButtonModule, 
    InputTextModule, 
    AvatarModule, 
    BadgeModule, 
    TooltipModule,
    MenuModule
  ],
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent implements OnInit {
  layoutService = inject(LayoutService);
  themeService = inject(ThemeService);
  authService = inject(AuthService);

  @ViewChild('menu') menu!: Menu;

  user = this.authService.currentUser;
  menuItems: MenuItem[] = [];

  ngOnInit() {
      this.menuItems = [
        { 
            label: 'الملف الشخصي', 
            icon: 'pi pi-user',
            command: () => { /* Navigate to profile */ }
        },
        { 
            label: 'إعدادات الحساب', 
            icon: 'pi pi-cog',
            command: () => { /* Navigate to settings */ }
        },
        { separator: true },
        { 
            label: 'تسجيل الخروج', 
            icon: 'pi pi-sign-out', 
            styleClass: 'text-red-500',
            iconStyle: { color: 'var(--red-500)' },
            command: () => this.authService.logout() 
        }
      ];
  }

  toggleMenu(event: Event) {
      this.menu.toggle(event);
  }
}
