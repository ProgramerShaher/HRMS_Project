import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'primeng/tabs';
import { RoleListComponent } from '../components/role-list/role-list.component';
import { UserListComponent } from '../components/user-list/user-list.component';

@Component({
    selector: 'app-access-control',
    standalone: true,
    imports: [CommonModule, TabsModule, RoleListComponent, UserListComponent],
    template: `
    <div class="p-4">
        <div class="mb-6">
            <h1 class="text-2xl font-black text-slate-900 dark:text-white">التحكم في الوصول</h1>
            <p class="text-slate-500">إدارة المستخدمين والأدوار وصلاحيات النظام</p>
        </div>

        <p-tabs [value]="0">
            <p-tablist>
                <p-tab [value]="0">
                    <i class="pi pi-users ml-2"></i> إدارة المستخدمين
                </p-tab>
                <p-tab [value]="1">
                    <i class="pi pi-lock ml-2"></i> إدارة الأدوار
                </p-tab>
            </p-tablist>
            <p-tabpanels>
                <p-tabpanel [value]="0">
                    <app-user-list></app-user-list>
                </p-tabpanel>
                <p-tabpanel [value]="1">
                    <app-role-list></app-role-list>
                </p-tabpanel>
            </p-tabpanels>
        </p-tabs>
    </div>
  `
})
export class AccessControlComponent { }
