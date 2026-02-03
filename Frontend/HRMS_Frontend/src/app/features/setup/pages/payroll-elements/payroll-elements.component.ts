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
import { PayrollElement } from '../../models/setup.models';
import { PayrollElementFormComponent } from './payroll-element-form.component';

@Component({
  selector: 'app-payroll-elements',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, InputTextModule, ToolbarModule, ToastModule, ConfirmDialogModule],
  providers: [MessageService, ConfirmationService, DialogService],
  templateUrl: './payroll-elements.component.html',
  styleUrls: ['./payroll-elements.component.scss']
})
export class PayrollElementsComponent implements OnInit {
  setupService = inject(SetupService);
  messageService = inject(MessageService);
  confirmationService = inject(ConfirmationService);
  dialogService = inject(DialogService);

  elements = signal<PayrollElement[]>([]);
  loading = signal(true);
  ref: DynamicDialogRef | undefined | null;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    // Endpoint: api/PayrollSettings/elements
    this.setupService.getAll<any>('PayrollSettings/elements').subscribe({
        next: (response: any) => { 
            const list = Array.isArray(response) ? response : (Array.isArray(response?.data) ? response.data : []);
            this.elements.set(list);
            this.loading.set(false);
        },
        error: () => this.loading.set(false)
    });
  }

  openNew() {
    this.ref = this.dialogService.open(PayrollElementFormComponent, {
        header: 'إضافة بند راتب جديد',
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

  edit(item: PayrollElement) {
    this.ref = this.dialogService.open(PayrollElementFormComponent, {
        header: 'تعديل بند الراتب',
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

  delete(item: PayrollElement) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف البند؟ ' + item.elementNameAr,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.setupService.delete('PayrollSettings/elements', item.elementId).subscribe(() => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الحذف' });
          this.loadData();
        });
      }
    });
  }
}
