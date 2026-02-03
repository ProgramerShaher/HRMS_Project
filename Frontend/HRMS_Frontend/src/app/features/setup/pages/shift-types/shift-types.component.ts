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
import { ShiftType } from '../../models/setup.models';
import { ShiftTypeFormComponent } from './shift-type-form.component';

@Component({
  selector: 'app-shift-types',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, InputTextModule, ToolbarModule, ToastModule, ConfirmDialogModule],
  providers: [MessageService, ConfirmationService, DialogService],
  templateUrl: './shift-types.component.html',
  styleUrls: ['./shift-types.component.scss']
})
export class ShiftTypesComponent implements OnInit {
  setupService = inject(SetupService);
  messageService = inject(MessageService);
  confirmationService = inject(ConfirmationService);
  dialogService = inject(DialogService);

  shiftTypes = signal<ShiftType[]>([]);
  loading = signal(true);
  ref: DynamicDialogRef | undefined | null;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    // Endpoint: api/AttendanceSettings/shifts
    this.setupService.getAll<any>('AttendanceSettings/shifts').subscribe({
        next: (response: any) => { 
            const list = Array.isArray(response) ? response : (Array.isArray(response?.data) ? response.data : []);
            this.shiftTypes.set(list);
            this.loading.set(false);
        },
        error: () => this.loading.set(false)
    });
  }

  openNew() {
    this.ref = this.dialogService.open(ShiftTypeFormComponent, {
        header: 'إضافة وردية جديدة',
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

  edit(item: ShiftType) {
    this.ref = this.dialogService.open(ShiftTypeFormComponent, {
        header: 'تعديل بيانات الوردية',
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

  delete(item: ShiftType) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف الوردية؟ ' + item.shiftNameAr,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.setupService.delete('AttendanceSettings/shifts', item.shiftId).subscribe(() => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الحذف' });
          this.loadData();
        });
      }
    });
  }
}
