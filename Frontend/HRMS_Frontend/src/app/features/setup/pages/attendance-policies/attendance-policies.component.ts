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
import { AttendancePolicy } from '../../models/setup.models';
import { AttendancePolicyFormComponent } from './attendance-policy-form.component';

@Component({
  selector: 'app-attendance-policies',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, InputTextModule, ToolbarModule, ToastModule, ConfirmDialogModule],
  providers: [MessageService, ConfirmationService, DialogService],
  templateUrl: './attendance-policies.component.html',
  styleUrls: ['./attendance-policies.component.scss']
})
export class AttendancePoliciesComponent implements OnInit {
  setupService = inject(SetupService);
  messageService = inject(MessageService);
  confirmationService = inject(ConfirmationService);
  dialogService = inject(DialogService);

  policies = signal<AttendancePolicy[]>([]);
  loading = signal(true);
  ref: DynamicDialogRef | undefined | null;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    // Endpoint: api/AttendancePolicy
    this.setupService.getAll<any>('AttendancePolicy').subscribe({
        next: (response: any) => { 
            const list = Array.isArray(response) ? response : (Array.isArray(response?.data) ? response.data : []);
            this.policies.set(list);
            this.loading.set(false);
        },
        error: () => this.loading.set(false)
    });
  }

  openNew() {
    this.ref = this.dialogService.open(AttendancePolicyFormComponent, {
        header: 'إضافة سياسة حضور جديدة',
        width: '500px',
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

  edit(item: AttendancePolicy) {
    this.ref = this.dialogService.open(AttendancePolicyFormComponent, {
        header: 'تعديل سياسة الحضور',
        width: '500px',
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

  delete(item: AttendancePolicy) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف السياسة؟ ' + item.policyNameAr,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.setupService.delete('AttendancePolicy', item.policyId).subscribe(() => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الحذف' });
          this.loadData();
        });
      }
    });
  }
}
