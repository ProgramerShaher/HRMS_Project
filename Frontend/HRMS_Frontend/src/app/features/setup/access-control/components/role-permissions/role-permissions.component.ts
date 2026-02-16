import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { AccessControlService, Permission } from '../../services/access-control.service';
import { TabsModule } from 'primeng/tabs';
import { BadgeModule } from 'primeng/badge';

interface PermissionGroup {
    name: string;
    permissions: Permission[];
    selectedCount: number;
}

@Component({
    selector: 'app-role-permissions',
    standalone: true,
    imports: [CommonModule, ButtonModule, CheckboxModule, FormsModule, BadgeModule, TabsModule],
    templateUrl: './role-permissions.component.html'
})
export class RolePermissionsComponent implements OnInit {
    @Input({ required: true }) roleId!: number;
    @Output() onClose = new EventEmitter<void>();

    private service = inject(AccessControlService);
    private messageService = inject(MessageService);

    allPermissions = signal<Permission[]>([]);
    rolePermissions = signal<string[]>([]); // Using names for check, but need IDs for update. The API uses IDs for update? No, API uses IDs.
    // Wait, I updated the API to take List<int> permissionIds through service. But service getRolePermissions returns string[] (Names).
    // I need IDs.
    // Let me check my service again. 
    // getRolePermissions returns string[]. This is bad for update if I need IDs.
    // But wait, the UpdateRolePermissions takes List<int> in API.
    // So I need to map permission names back to IDs, OR update getRolePermissions to return Objects/IDs.

    // Let me check service definition I wrote earlier:
    // getRolePermissions(roleId: number): Observable<Result<string[]>>

    // I should update the service/API to return Permission objects or IDs.
    // For now, since I load AllPermissions (which have IDs), I can map Name -> ID.

    groups = signal<PermissionGroup[]>([]);
    selectedPermissionIds = signal<number[]>([]);
    loading = signal(false);

    ngOnInit() {
        this.loadData();
    }

    loadData() {
        this.loading.set(true);
        // ForkJoin would be better
        this.service.getAllPermissions().subscribe(res => {
            this.allPermissions.set(res.data);
            this.groupPermissions(res.data);

            this.service.getRolePermissions(this.roleId).subscribe(roleRes => {
                // Map permission names to IDs
                const currentPermissionNames = new Set(roleRes.data);
                const selectedIds = this.allPermissions()
                    .filter(p => currentPermissionNames.has(p.name))
                    .map(p => p.id);

                this.selectedPermissionIds.set(selectedIds);
                this.updateGroupCounts();
                this.loading.set(false);
            });
        });
    }

    groupPermissions(permissions: Permission[]) {
        const groupsMap = new Map<string, Permission[]>();

        permissions.forEach(p => {
            const groupName = p.name.split('.')[0] || 'Common';
            if (!groupsMap.has(groupName)) {
                groupsMap.set(groupName, []);
            }
            groupsMap.get(groupName)!.push(p);
        });

        const groups: PermissionGroup[] = [];
        groupsMap.forEach((perms, name) => {
            groups.push({ name, permissions: perms, selectedCount: 0 });
        });

        this.groups.set(groups.sort((a, b) => a.name.localeCompare(b.name)));
    }

    updateGroupCounts() {
        const selected = new Set(this.selectedPermissionIds());
        this.groups.update(groups => {
            groups.forEach(g => {
                g.selectedCount = g.permissions.filter(p => selected.has(p.id)).length;
            });
            return [...groups];
        });
    }

    togglePermission(permissionId: number, checked: boolean) {
        this.selectedPermissionIds.update(ids => {
            if (checked) {
                return [...ids, permissionId];
            } else {
                return ids.filter(id => id !== permissionId);
            }
        });
        this.updateGroupCounts();
    }

    isSelected(permissionId: number): boolean {
        return this.selectedPermissionIds().includes(permissionId);
    }

    toggleGroup(group: PermissionGroup, checked: boolean) {
        const groupPermissionIds = group.permissions.map(p => p.id);
        this.selectedPermissionIds.update(ids => {
            let newIds = ids.filter(id => !groupPermissionIds.includes(id)); // Remove all from group
            if (checked) {
                newIds = [...newIds, ...groupPermissionIds]; // Add all from group
            }
            return newIds;
        });
        this.updateGroupCounts();
    }

    isGroupSelected(group: PermissionGroup): boolean {
        return group.selectedCount === group.permissions.length;
    }

    save() {
        this.service.updateRolePermissions(this.roleId, this.selectedPermissionIds()).subscribe(res => {
            if (res.succeeded) {
                this.messageService.add({ severity: 'success', summary: 'تم الحفظ', detail: 'تم تحديث الصلاحيات بنجاح' });
                this.onClose.emit();
            }
        });
    }
}
