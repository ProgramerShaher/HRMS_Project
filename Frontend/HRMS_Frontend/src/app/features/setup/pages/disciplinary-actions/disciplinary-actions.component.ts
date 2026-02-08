import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { CheckboxModule } from 'primeng/checkbox';
import { InputNumberModule } from 'primeng/inputnumber';
import { MessageService, ConfirmationService } from 'primeng/api';
import { PerformanceSetupService } from '../../services/performance-setup.service';
import { DisciplinaryAction } from '../../models/performance-setup.model';

@Component({
  selector: 'app-disciplinary-actions',
  standalone: true,
  imports: [
    CommonModule, FormsModule, TableModule, ButtonModule, InputTextModule,
    DialogModule, ToastModule, ConfirmDialogModule, CheckboxModule, InputNumberModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './disciplinary-actions.component.html'
})
export class DisciplinaryActionsComponent implements OnInit {
  private service = inject(PerformanceSetupService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  actions = signal<DisciplinaryAction[]>([]);
  loading = signal<boolean>(true);
  dialogVisible = false;
  submitted = false;
  
  // Extending Partial to handle the boolean conversion for switch
  currentItem: Partial<DisciplinaryAction> & { isTerminationBoolean?: boolean } = {};

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.service.getDisciplinaryActions().subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.actions.set(res.data || []);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  openNew() {
    this.currentItem = { deductionDays: 0, isTermination: 0, isTerminationBoolean: false };
    this.submitted = false;
    this.dialogVisible = true;
  }

  edit(item: DisciplinaryAction) {
    this.currentItem = { 
        ...item,
        isTerminationBoolean: item.isTermination === 1
    };
    this.dialogVisible = true;
  }

  delete(item: DisciplinaryAction) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete ' + item.actionNameAr + '?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.service.deleteDisciplinaryAction(item.actionId).subscribe({
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

    if (this.currentItem.actionNameAr) {
        // Convert boolean back to 0/1
        this.currentItem.isTermination = this.currentItem.isTerminationBoolean ? 1 : 0;

        if (this.currentItem.actionId) {
            this.service.updateDisciplinaryAction(this.currentItem.actionId, this.currentItem).subscribe({
                next: (res) => {
                    if (res.succeeded) {
                        this.messageService.add({severity:'success', summary: 'Successful', detail: 'Updated Successfully'});
                        this.loadData();
                        this.hideDialog();
                    }
                }
            });
        } else {
            this.service.createDisciplinaryAction(this.currentItem).subscribe({
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
