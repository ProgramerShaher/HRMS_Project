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
import { DocumentType, PaginatedResult } from '../../models/setup.models';
import { DocumentTypeFormComponent } from './document-type-form.component';

@Component({
  selector: 'app-document-types',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, InputTextModule, ToolbarModule, ToastModule, ConfirmDialogModule],
  providers: [MessageService, ConfirmationService, DialogService],
  templateUrl: './document-types.component.html',
  styleUrls: ['./document-types.component.scss']
})
export class DocumentTypesComponent implements OnInit {
  setupService = inject(SetupService);
  messageService = inject(MessageService);
  confirmationService = inject(ConfirmationService);
  dialogService = inject(DialogService);

  documentTypes = signal<DocumentType[]>([]);
  loading = signal(true);
  ref: DynamicDialogRef | undefined | null;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.setupService.getAll<PaginatedResult<DocumentType>>('DocumentTypes').subscribe({
        next: (response: any) => { 
            // Handle both wrapped and unwrapped responses if necessary, based on user input example "data": { "items": [] }
            const list = response?.data?.items || response?.items || [];
            this.documentTypes.set(list);
            this.loading.set(false);
        },
        error: () => this.loading.set(false)
    });
  }

  openNew() {
    this.ref = this.dialogService.open(DocumentTypeFormComponent, {
        header: 'إضافة نوع وثيقة جديد',
        width: '500px',
        contentStyle: { overflow: 'visible' }, // visible for dropdowns etc to pop out if needed
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

  edit(item: DocumentType) {
    this.ref = this.dialogService.open(DocumentTypeFormComponent, {
        header: 'تعديل بيانات الوثيقة',
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

  delete(item: DocumentType) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف نوع الوثيقة؟ ' + item.documentTypeNameAr,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.setupService.delete('DocumentTypes', item.documentTypeId).subscribe(() => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الحذف' });
          this.loadData();
        });
      }
    });
  }
}
