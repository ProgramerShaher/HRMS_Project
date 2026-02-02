import { Component, signal, WritableSignal, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { SetupService } from '../../services/setup.service';
import { JobFormComponent } from './job-form.component';

@Component({
  selector: 'app-jobs',
  standalone: true,
  imports: [
    CommonModule, 
    InputTextModule, 
    ButtonModule, 
    TableModule, 
    ToastModule, 
    ConfirmDialogModule
  ],
  providers: [MessageService, ConfirmationService, DialogService],
  templateUrl: './jobs.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class JobsComponent implements OnInit {
  jobs: WritableSignal<any[]> = signal([]);
  loading: WritableSignal<boolean> = signal(true);
  ref: DynamicDialogRef | undefined | null;

  constructor(
    private setupService: SetupService,
    public dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadJobs();
  }

  loadJobs() {
    this.loading.set(true);
    this.setupService.getAll('Jobs').subscribe({
      next: (res: any) => {
        // Handle paginated response structure: { data: { items: [...] } }
        this.jobs.set(res.data?.items || res.items || res.data || res || []);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل الوظائف' });
      }
    });
  }

  openNew() {
    this.ref = this.dialogService.open(JobFormComponent, {
      header: 'إضافة وظيفة جديدة',
      width: '50vw',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      maximizable: true,
      data: {}
    });

    this.ref?.onClose.subscribe((result: any) => {
      if (result) {
        this.loadJobs();
      }
    });
  }

  edit(job: any) {
    this.ref = this.dialogService.open(JobFormComponent, {
      header: 'تعديل بيانات الوظيفة',
      width: '50vw',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      maximizable: true,
      data: job
    });

    this.ref?.onClose.subscribe((result: any) => {
      if (result) {
        this.loadJobs();
      }
    });
  }

  delete(job: any) {
    this.confirmationService.confirm({
      message: `هل أنت متأكد من حذف الوظيفة  "${job.jobTitleAr}"؟`,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم، احذف',
      rejectLabel: 'إلغاء',
      acceptButtonStyleClass: 'p-button-danger p-button-text',
      rejectButtonStyleClass: 'p-button-text p-button-secondary',
      accept: () => {
        this.setupService.delete('Jobs', job.jobId).subscribe({
            next: () => {
                this.messageService.add({ severity: 'success', summary: 'تم الحذف', detail: 'تم حذف الوظيفة بنجاح' });
                this.loadJobs();
            },
            error: () => {
                this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء الحذف' });
            }
        });
      }
    });
  }
}
