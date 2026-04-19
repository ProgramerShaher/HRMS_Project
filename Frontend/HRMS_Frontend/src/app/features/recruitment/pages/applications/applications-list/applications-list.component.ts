import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { RecruitmentService } from '../../../services/recruitment.service';
import { JobApplication, Vacancy, Candidate, APPLICATION_STATUSES } from '../../../models/recruitment.models';

type TagSeverity = 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast';

@Component({
  selector: 'app-applications-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    TableModule, ButtonModule, DialogModule,
    SelectModule, InputTextModule, TextareaModule,
    TagModule, TooltipModule, ToastModule, ConfirmDialogModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './applications-list.component.html'
})
export class ApplicationsListComponent implements OnInit {
  private svc = inject(RecruitmentService);
  private fb = inject(FormBuilder);
  private msg = inject(MessageService);
  private confirm = inject(ConfirmationService);

  applications = signal<JobApplication[]>([]);
  vacancies = signal<Vacancy[]>([]);
  candidates = signal<Candidate[]>([]);
  loading = signal(false);

  // Dialog: new application
  showSubmitDialog = false;
  submitForm!: FormGroup;
  submittedApp = false;

  // Dialog: change status
  showStatusDialog = false;
  selectedApp: JobApplication | null = null;
  statusForm!: FormGroup;
  submittedStatus = false;

  readonly statuses = APPLICATION_STATUSES;
  statusOptions = APPLICATION_STATUSES.map(s => ({ label: s.labelAr, value: s.key }));

  ngOnInit() {
    this.buildForms();
    this.load();
    this.loadLookups();
  }

  buildForms() {
    this.submitForm = this.fb.group({
      vacancyId:   [null, [Validators.required, Validators.min(1)]],
      candidateId: [null, [Validators.required, Validators.min(1)]],
      source:      [''],
    });
    this.statusForm = this.fb.group({
      status: [null, Validators.required],
      notes:  [''],
    });
  }

  load() {
    this.loading.set(true);
    this.svc.getApplications().subscribe({
      next: r => { if (r.succeeded) this.applications.set(r.data); this.loading.set(false); },
      error: () => { this.loading.set(false); this.err('فشل تحميل الطلبات'); }
    });
  }

  loadLookups() {
    this.svc.getVacancies('ACTIVE').subscribe(r => { if (r.succeeded) this.vacancies.set(r.data); });
    this.svc.getCandidates().subscribe(r => { if (r.succeeded) this.candidates.set(r.data); });
  }

  get vacancyOptions() {
    return this.vacancies().map(v => ({ label: `${v.vacancyTitle} (${v.departmentName})`, value: v.vacancyId }));
  }
  get candidateOptions() {
    return this.candidates().map(c => ({ label: `${c.fullNameEn} – ${c.email}`, value: c.candidateId }));
  }

  openSubmit() {
    this.submittedApp = false;
    this.submitForm.reset();
    this.showSubmitDialog = true;
  }

  saveSubmit() {
    this.submittedApp = true;
    if (this.submitForm.invalid) return;
    this.svc.submitApplication(this.submitForm.value).subscribe(r => {
      if (r.succeeded) {
        this.ok('تم تقديم الطلب بنجاح');
        this.showSubmitDialog = false;
        this.load();
      } else this.err(r.message);
    });
  }

  openStatus(app: JobApplication) {
    this.selectedApp = app;
    this.submittedStatus = false;
    this.statusForm.patchValue({ status: app.status, notes: '' });
    this.showStatusDialog = true;
  }

  saveStatus() {
    this.submittedStatus = true;
    if (this.statusForm.invalid || !this.selectedApp) return;
    const cmd = { ...this.statusForm.value, appId: this.selectedApp.applicationId };
    this.svc.changeApplicationStatus(this.selectedApp.applicationId, cmd).subscribe(r => {
      if (r.succeeded) {
        this.ok('تم تحديث حالة الطلب');
        this.showStatusDialog = false;
        this.load();
      } else this.err(r.message);
    });
  }

  withdraw(app: JobApplication) {
    this.confirm.confirm({
      message: `هل تريد سحب طلب ${app.candidateName} من وظيفة "${app.vacancyTitle}"؟`,
      header: 'سحب الطلب',
      icon: 'pi pi-times-circle',
      acceptLabel: 'نعم، اسحب',
      rejectLabel: 'إلغاء',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.svc.withdrawApplication(app.applicationId).subscribe(r => {
        if (r.succeeded) { this.ok('تم سحب الطلب'); this.load(); }
        else this.err(r.message);
      })
    });
  }

  countByStatus(key: string): number {
    return this.applications().filter(a => a.status?.toUpperCase() === key).length;
  }

  severity(status: string): TagSeverity {
    const found = APPLICATION_STATUSES.find(s => s.key === status?.toUpperCase());
    return (found?.color as TagSeverity) ?? 'info';
  }

  labelAr(status: string) {
    return APPLICATION_STATUSES.find(s => s.key === status?.toUpperCase())?.labelAr ?? status;
  }

  private ok(detail: string) { this.msg.add({ severity: 'success', summary: 'تم بنجاح', detail }); }
  private err(detail: string) { this.msg.add({ severity: 'error', summary: 'خطأ', detail }); }
}
