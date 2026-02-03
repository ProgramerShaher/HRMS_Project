import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToolbarModule } from 'primeng/toolbar';
import { ToastModule } from 'primeng/toast';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { SetupService } from '../../services/setup.service';
import { LeaveType } from '../../models/setup.models';
import { LeaveTypeFormComponent } from './leave-type-form.component';

@Component({
  selector: 'app-leave-types',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, InputTextModule, ToolbarModule, ToastModule, ConfirmDialogModule],
  providers: [MessageService, ConfirmationService, DialogService],
  templateUrl: './leave-types.component.html',
  styleUrls: ['./leave-types.component.scss']
})
export class LeaveTypesComponent implements OnInit {
  setupService = inject(SetupService);
  messageService = inject(MessageService);
  confirmationService = inject(ConfirmationService);
  dialogService = inject(DialogService);

  leaveTypes = signal<LeaveType[]>([]);
  loading = signal(true);
  ref: DynamicDialogRef | undefined | null;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    // Endpoint: api/LeaveConfiguration/leave-types
    this.setupService.getAll<any>('LeaveConfiguration/leave-types').subscribe({
        next: (response: any) => { 
            // The user provided JSON shows "data": [ ... ] directly, or "data": { "items": ... }
            // The JSON snippet: { "data": [ { ... }, { ... } ], "succeeded": true ... }
            const list = Array.isArray(response) ? response : (Array.isArray(response?.data) ? response.data : []);
            this.leaveTypes.set(list);
            this.loading.set(false);
        },
        error: () => this.loading.set(false)
    });
  }

  openNew() {
    this.ref = this.dialogService.open(LeaveTypeFormComponent, {
        header: 'إضافة نوع إجازة جديد',
        width: '450px',
        contentStyle: { overflow: 'visible' },
        breakpoints: {
            '960px': '75vw',
            '640px': '90vw'
        },
        data: {}
    });

    this.ref?.onClose.subscribe((success: boolean) => {
        if (success) this.loadData();
    });
  }

  edit(item: LeaveType) {
    this.ref = this.dialogService.open(LeaveTypeFormComponent, {
        header: 'تعديل نوع الإجازة',
        width: '450px',
        contentStyle: { overflow: 'visible' },
        breakpoints: {
            '960px': '75vw',
            '640px': '90vw'
        },
        data: item
    });

    this.ref?.onClose.subscribe((success: boolean) => {
        if (success) this.loadData();
    });
  }

  delete(item: LeaveType) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف نوع الإجازة؟ ' + item.leaveTypeNameAr,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        // Assuming Delete endpoint is same base URL + ID
        this.setupService.delete('LeaveConfiguration/leave-types', item.leaveTypeId).subscribe(() => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الحذف' });
          this.loadData();
        });
      }
    });
  }
}
