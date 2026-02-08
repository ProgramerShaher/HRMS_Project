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
import { DatePickerModule } from 'primeng/datepicker';
import { MessageService, ConfirmationService } from 'primeng/api';
import { PerformanceSetupService } from '../../services/performance-setup.service';
import { AppraisalCycle } from '../../models/performance-setup.model';

@Component({
  selector: 'app-appraisal-cycles',
  standalone: true,
  imports: [
    CommonModule, FormsModule, TableModule, ButtonModule, InputTextModule,
    DialogModule, ToastModule, ConfirmDialogModule, SelectModule, DatePickerModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './appraisal-cycles.component.html'
})
export class AppraisalCyclesComponent implements OnInit {
  private service = inject(PerformanceSetupService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  cycles = signal<AppraisalCycle[]>([]);
  loading = signal<boolean>(true);
  dialogVisible = false;
  submitted = false;
  
  // Handling Date conversion if needed, but PrimeNG calendar handles Date objects
  currentItem: Partial<AppraisalCycle> | any = {};

  statusOptions = [
    { label: 'مخطط لها (Planning)', value: 'PLANNING' },
    { label: 'نشطة (Active)', value: 'ACTIVE' },
    { label: 'مكتملة (Completed)', value: 'COMPLETED' },
    { label: 'ملغاة (Cancelled)', value: 'CANCELLED' }
  ];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.service.getAppraisalCycles().subscribe({
      next: (res) => {
        if (res.succeeded) {
          // Ensure dates are parsed as Date objects for Calendar
          const data = (res.data || []).map(item => ({
             ...item,
             startDate: item.startDate ? new Date(item.startDate) : null,
             endDate: item.endDate ? new Date(item.endDate) : null
          }));
          this.cycles.set(data as any);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  openNew() {
    this.currentItem = {};
    this.submitted = false;
    this.dialogVisible = true;
  }

  edit(item: AppraisalCycle) {
    this.currentItem = { 
        ...item,
        startDate: item.startDate ? new Date(item.startDate) : null,
        endDate: item.endDate ? new Date(item.endDate) : null
    };
    this.dialogVisible = true;
  }

  delete(item: AppraisalCycle) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete ' + item.cycleName + '?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.service.deleteAppraisalCycle(item.cycleId).subscribe({
            next: (res) => {
                if(res.succeeded) {
                    this.messageService.add({severity:'success', summary: 'Successful', detail: 'Deleted Successfully'});
                    this.loadData();
                } else {
                     this.messageService.add({ severity: 'error', summary: 'Error', detail: res.message });
                }
            }
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

    if (this.currentItem.cycleName) {
        if (this.currentItem.cycleId) {
            this.service.updateAppraisalCycle(this.currentItem.cycleId, this.currentItem).subscribe({
                next: (res) => {
                    if (res.succeeded) {
                        this.messageService.add({severity:'success', summary: 'Successful', detail: 'Updated Successfully'});
                        this.loadData();
                        this.hideDialog();
                    }
                }
            });
        } else {
            this.service.createAppraisalCycle(this.currentItem).subscribe({
                next: (res) => {
                    if (res.succeeded) {
                        this.messageService.add({severity:'success', summary: 'Successful', detail: 'Created Successfully'});
                        this.loadData();
                        this.hideDialog();
                    }
                }
            });
        }
    }
  }

  getStatusClass(status: string): string {
      switch(status?.toUpperCase()) {
          case 'ACTIVE': return 'bg-green-100 text-green-700';
          case 'COMPLETED': return 'bg-gray-100 text-gray-700';
          case 'PLANNING': return 'bg-blue-100 text-blue-700';
          case 'CANCELLED': return 'bg-red-100 text-red-700';
          default: return 'bg-slate-100 text-slate-700';
      }
  }
}
