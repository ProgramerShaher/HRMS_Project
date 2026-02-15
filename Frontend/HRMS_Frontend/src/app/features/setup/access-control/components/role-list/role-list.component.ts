import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { AccessControlService, ApplicationRole, CreateRoleDto, UpdateRoleDto } from '../../services/access-control.service';
import { RolePermissionsComponent } from '../role-permissions/role-permissions.component';

@Component({
    selector: 'app-role-list',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        TableModule,
        ButtonModule,
        DialogModule,
        InputTextModule,
        ToastModule,
        ConfirmDialogModule,
        RolePermissionsComponent
    ],
    providers: [MessageService, ConfirmationService],
    templateUrl: './role-list.component.html',
    styles: [`
    :host {
      display: block;
      padding: 1rem;
    }
  `]
})
export class RoleListComponent implements OnInit {
    private service = inject(AccessControlService);
    private messageService = inject(MessageService);
    private confirmationService = inject(ConfirmationService);
    private fb = inject(FormBuilder);

    roles = signal<ApplicationRole[]>([]);
    loading = signal(false);

    // Dialog State
    roleDialog = signal(false);
    permissionsDialog = signal(false);
    currentRole = signal<ApplicationRole | null>(null);
    submitted = signal(false);

    // Form
    roleForm = this.fb.group({
        name: ['', [Validators.required]],
        description: ['']
    });

    ngOnInit() {
        this.loadRoles();
    }

    loadRoles() {
        this.loading.set(true);
        this.service.getAllRoles().subscribe({
            next: (result) => {
                this.roles.set(result.data);
                this.loading.set(false);
            },
            error: () => this.loading.set(false)
        });
    }

    openNew() {
        this.currentRole.set(null);
        this.roleForm.reset();
        this.submitted.set(false);
        this.roleDialog.set(true);
    }

    editRole(role: ApplicationRole) {
        this.currentRole.set(role);
        this.roleForm.patchValue({
            name: role.name,
            description: role.description
        });
        this.roleDialog.set(true);
    }

    managePermissions(role: ApplicationRole) {
        this.currentRole.set(role);
        this.permissionsDialog.set(true);
    }

    deleteRole(role: ApplicationRole) {
        this.confirmationService.confirm({
            message: 'هل أنت متأكد من حذف هذا الدور؟',
            header: 'تأكيد الحذف',
            icon: 'pi pi-exclamation-triangle',
            acceptLabel: 'نعم',
            rejectLabel: 'لا',
            accept: () => {
                this.service.deleteRole(role.id).subscribe(res => {
                    if (res.succeeded) {
                        this.messageService.add({ severity: 'success', summary: 'ناجح', detail: 'تم حذف الدور بنجاح' });
                        this.loadRoles();
                    } else {
                        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
                    }
                });
            }
        });
    }

    saveRole() {
        this.submitted.set(true);

        if (this.roleForm.invalid) return;

        const roleData = this.roleForm.value as CreateRoleDto;

        if (this.currentRole()) {
            // Update
            this.service.updateRole(this.currentRole()!.id, roleData).subscribe(res => {
                if (res.succeeded) {
                    this.messageService.add({ severity: 'success', summary: 'ناجح', detail: 'تم تحديث الدور بنجاح' });
                    this.loadRoles();
                    this.roleDialog.set(false);
                }
            });
        } else {
            // Create
            this.service.createRole(roleData).subscribe(res => {
                if (res.succeeded) {
                    this.messageService.add({ severity: 'success', summary: 'ناجح', detail: 'تم إنشاء الدور بنجاح' });
                    this.loadRoles();
                    this.roleDialog.set(false);
                }
            });
        }
    }

    hideDialog() {
        this.roleDialog.set(false);
        this.permissionsDialog.set(false);
    }
}
