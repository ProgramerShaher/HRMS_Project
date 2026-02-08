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
import { Kpi } from '../../models/performance-setup.model';

@Component({
  selector: 'app-kpi-library',
  standalone: true,
  imports: [
    CommonModule, FormsModule, TableModule, ButtonModule, InputTextModule,
    DialogModule, ToastModule, ConfirmDialogModule, SelectModule, TextareaModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './kpi-library.component.html'
})
export class KpiLibraryComponent implements OnInit {
  private service = inject(PerformanceSetupService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  kpis = signal<Kpi[]>([]);
  loading = signal<boolean>(true);
  dialogVisible = false;
  submitted = false;
  
  currentItem: Partial<Kpi> = {};

  categoryOptions = [
    { label: 'Technical', value: 'Technical' },
    { label: 'Behavioral', value: 'Behavioral' },
    { label: 'Management', value: 'Management' },
    { label: 'Operational', value: 'Operational' }
  ];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.service.getKpis().subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.kpis.set(res.data || []);
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

  edit(item: Kpi) {
    this.currentItem = { ...item };
    this.dialogVisible = true;
  }

  delete(item: Kpi) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete ' + item.kpiNameAr + '?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.service.deleteKpi(item.kpiId).subscribe({
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

    if (this.currentItem.kpiNameAr) {
        if (this.currentItem.kpiId) {
            this.service.updateKpi(this.currentItem.kpiId, this.currentItem).subscribe({
                next: (res) => {
                    if (res.succeeded) {
                        this.messageService.add({severity:'success', summary: 'Successful', detail: 'Updated Successfully'});
                        this.loadData();
                        this.hideDialog();
                    }
                }
            });
        } else {
            this.service.createKpi(this.currentItem).subscribe({
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
}
