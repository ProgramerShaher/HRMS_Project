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
import { JobGrade, PaginatedResult } from '../../models/setup.models';
import { JobGradeFormComponent } from './job-grade-form.component';

@Component({
  selector: 'app-job-grades',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, InputTextModule, ToolbarModule, ToastModule, ConfirmDialogModule],
  providers: [MessageService, ConfirmationService, DialogService],
  templateUrl: './job-grades.component.html',
  styleUrls: ['./job-grades.component.scss']
})
export class JobGradesComponent implements OnInit {
  setupService = inject(SetupService);
  messageService = inject(MessageService);
  confirmationService = inject(ConfirmationService);
  dialogService = inject(DialogService);

  jobGrades = signal<JobGrade[]>([]);
  loading = signal(true);
  ref: DynamicDialogRef | undefined | null;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    // Assuming API endpoint is 'JobGrades' not 'job-grades' based on conventions so far?
    // User sidebar route said 'job-grades', but SetupService maps strings to API endpoints. 
    // Usually backend is PascalCase plural: 'JobGrades'.
    this.setupService.getAll<PaginatedResult<JobGrade>>('JobGrades').subscribe({
        next: (response: any) => { 
            const list = response?.data?.items || response?.items || [];
            this.jobGrades.set(list);
            this.loading.set(false);
        },
        error: () => this.loading.set(false)
    });
  }

  openNew() {
    this.ref = this.dialogService.open(JobGradeFormComponent, {
        header: 'إضافة درجة وظيفية جديدة',
        width: '50vw',
        contentStyle: { overflow: 'auto' },
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

  edit(item: JobGrade) {
    this.ref = this.dialogService.open(JobGradeFormComponent, {
        header: 'تعديل الدرجة الوظيفية',
        width: '50vw',
        contentStyle: { overflow: 'auto' },
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

  delete(item: JobGrade) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف الدرجة الوظيفية؟ ' + item.gradeNameAr,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.setupService.delete('JobGrades', item.jobGradeId).subscribe(() => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الحذف' });
          this.loadData();
        });
      }
    });
  }
}
