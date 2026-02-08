import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { MessageService, ConfirmationService } from 'primeng/api';
import { PerformanceSetupService } from '../../services/performance-setup.service';
import { ViolationType } from '../../models/performance-setup.model';

@Component({
  selector: 'app-violation-types',
  standalone: true,
  imports: [
    CommonModule, FormsModule, TableModule, ButtonModule, InputTextModule,
    DialogModule, ToastModule, ConfirmDialogModule, SelectModule, TextareaModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './violation-types.component.html'
})
export class ViolationTypesComponent implements OnInit {
  private service = inject(PerformanceSetupService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  violationTypes = signal<ViolationType[]>([]);
  loading = signal<boolean>(true);
  dialogVisible = false;
  submitted = false;
  
  currentItem: Partial<ViolationType> = {};

  severityOptions = [
    { label: 'عالي (High)', value: 1 },
    { label: 'متوسط (Medium)', value: 2 },
    { label: 'منخفض (Low)', value: 3 },
    { label: 'حرج (Critical)', value: 4 }
  ];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.service.getViolationTypes().subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.violationTypes.set(res.data || []); 
        } else {
             this.violationTypes.set([]); 
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load data' });
      }
    });
  }

  openNew() {
    this.currentItem = {};
    this.submitted = false;
    this.dialogVisible = true;
  }

  edit(item: ViolationType) {
    this.currentItem = { ...item };
    this.dialogVisible = true;
  }

  delete(item: ViolationType) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف ' + item.violationNameAr + '؟',
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.service.deleteViolationType(item.violationTypeId).subscribe({
            next: (res) => {
                if(res.succeeded) {
                    this.messageService.add({severity:'success', summary: 'تم بنجاح', detail: 'تم الحذف بنجاح', life: 3000});
                    this.loadData();
                } else {
                     this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message || 'فشل الحذف' });
                }
            },
            error: () => this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل الحذف' })
        });
      }
    });
  }

  hideDialog() {
    this.dialogVisible = false;
    this.submitted = false;
  }

  save() {
    this.submitted = true;

    if (this.currentItem.violationNameAr) {
        if (this.currentItem.violationTypeId) {
            this.service.updateViolationType(this.currentItem.violationTypeId, this.currentItem).subscribe({
                next: (res) => {
                    if (res.succeeded) {
                        this.messageService.add({severity:'success', summary: 'تم بنجاح', detail: 'تم التحديث بنجاح', life: 3000});
                        this.loadData();
                        this.hideDialog();
                    }
                }
            });
        } else {
            this.service.createViolationType(this.currentItem).subscribe({
                next: (res) => {
                    if (res.succeeded) {
                        this.messageService.add({severity:'success', summary: 'تم بنجاح', detail: 'تم الإضافة بنجاح', life: 3000});
                        this.loadData();
                        this.hideDialog();
                    }
                }
            });
        }
    }
  }

  getSeverityLabel(level: number): string {
    switch (level) {
      case 1: return 'عالي (High)';
      case 2: return 'متوسط (Medium)';
      case 3: return 'منخفض (Low)';
      case 4: return 'حرج (Critical)';
      default: return 'غير محدد';
    }
  }

  getSeverityClass(level: number): string {
    switch (level) {
      case 1: return 'bg-orange-100 text-orange-700'; // High
      case 2: return 'bg-yellow-100 text-yellow-700'; // Medium
      case 3: return 'bg-blue-100 text-blue-700'; // Low
      case 4: return 'bg-red-100 text-red-700'; // Critical
      default: return 'bg-gray-100 text-gray-700';
    }
  }
}
