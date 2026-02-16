import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { MultiSelectModule } from 'primeng/multiselect';
import { FormsModule } from '@angular/forms';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { AccessControlService, ApplicationRole } from '../../services/access-control.service';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { TagModule } from 'primeng/tag';

import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { FloatLabelModule } from 'primeng/floatlabel';

@Component({
    selector: 'app-user-list',
    standalone: true,
    imports: [
        CommonModule,
        TableModule,
        ButtonModule,
        DialogModule,
        MultiSelectModule,
        FormsModule,
        ToastModule,
        ToggleButtonModule,
        TagModule,
        InputTextModule,
        PasswordModule,
        FloatLabelModule
    ],
    providers: [MessageService],
    templateUrl: './user-list.component.html'
})
export class UserListComponent implements OnInit {
    private service = inject(AccessControlService);
    private messageService = inject(MessageService);

    users = signal<any[]>([]);
    roles = signal<ApplicationRole[]>([]);
    loading = signal(false);

    roleDialog = signal(false);
    userDialog = signal(false);

    selectedUser = signal<any>(null);
    userRoles = signal<string[]>([]);

    newUser = {
        userName: '',
        email: '',
        password: '',
        confirmPassword: '',
        fullNameAr: '',
        fullNameEn: ''
    };

    ngOnInit() {
        this.loadData();
    }

    loadData() {
        this.loading.set(true);
        this.service.getAllUsers().subscribe(res => {
            this.users.set(res.data);
            this.loading.set(false);
        });
        this.service.getAllRoles().subscribe(res => {
            this.roles.set(res.data);
        });
    }

    openNewUser() {
        this.newUser = {
            userName: '',
            email: '',
            password: '',
            confirmPassword: '',
            fullNameAr: '',
            fullNameEn: ''
        };
        this.userDialog.set(true);
    }

    saveUser() {
        const data = this.newUser;
        if (!data.userName || !data.password || !data.email) {
            this.messageService.add({ severity: 'warn', summary: 'تنبيه', detail: 'يرجى إكمال البيانات المطلوبة' });
            return;
        }

        if (data.password !== data.confirmPassword) {
            this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'كلمات المرور غير متطابقة' });
            return;
        }

        console.log('Sending User Data:', data);
        this.service.createUser(data).subscribe({
            next: (res) => {
                if (res.succeeded) {
                    this.messageService.add({ severity: 'success', summary: 'تم بنجاح', detail: 'تم إنشاء المستخدم بنجاح' });
                    this.userDialog.set(false);
                    this.loadData();
                } else {
                    this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message || 'فشل إنشاء المستخدم' });
                }
            },
            error: (err) => {
                console.error('Registration Error Details:', err);
                let errorMsg = 'حدث خطأ أثناء الاتصال بالخادم';

                const error = err.error;
                if (error) {
                    // 1. Try to extract from Result object (Message or message)
                    if (error.message || error.Message) {
                        errorMsg = error.message || error.Message;
                    }
                    // 2. Try to extract from standard ASP.NET Core Validation errors
                    else if (error.errors) {
                        errorMsg = Object.values(error.errors).flat().join(', ');
                    }
                    // 3. If Identity errors are returned as a flat array
                    else if (Array.isArray(error)) {
                        errorMsg = error.map(e => e.description || e).join(', ');
                    }
                } else if (err.message) {
                    errorMsg = err.message;
                }

                this.messageService.add({
                    severity: 'error',
                    summary: 'فشل إنشاء المستخدم',
                    detail: errorMsg,
                    life: 5000 // Show for 5 seconds to give time to read
                });
            }
        });
    }

    editRoles(user: any) {
        this.selectedUser.set(user);
        // User object usually has roles property if mapped in Backend or we need another call
        // My AuthResponse had Roles, but ApplicationUser in GetAllUsers might not have them automatically in all configs
        // Let's assume it has a roles property or we need to simulate.
        // In Identity, UserManager.Users doesn't include Roles by default.
        // I should have included them. But for now let's hope it's there or handle it.
        this.userRoles.set(user.roles || []);
        this.roleDialog.set(true);
    }

    saveRoles() {
        const user = this.selectedUser();
        const currentRoles = user.roles || [];
        const newRoles = this.userRoles();

        // Roles to add
        const toAdd = newRoles.filter((r: string) => !currentRoles.includes(r));
        // Roles to remove
        const toRemove = currentRoles.filter((r: string) => !newRoles.includes(r));

        // Simple implementation: sequential calls (can be optimized with forkJoin)
        toAdd.forEach((role: string) => {
            this.service.addUserToRole(user.id, role).subscribe();
        });
        toRemove.forEach((role: string) => {
            this.service.removeUserFromRole(user.id, role).subscribe();
        });

        this.messageService.add({ severity: 'success', summary: 'تم التحديث', detail: 'تم تحديث أدوار المستخدم بنجاح' });
        this.roleDialog.set(false);
        this.loadData();
    }
}
