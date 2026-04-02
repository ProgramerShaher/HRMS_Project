import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DatePickerModule } from 'primeng/datepicker';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { RecruitmentService } from '../../../services/recruitment.service';
import { LookupService, Job, Department } from '../../../../../core/services/lookup.service'
import { Vacancy, CreateVacancyCommand, UpdateVacancyCommand } from '../../../models/recruitment.models';

type TagSeverity = 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast';

@Component({
  selector: 'app-vacancies-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    TableModule, ButtonModule, DialogModule,
    InputTextModule, InputNumberModule, DatePickerModule,
    TextareaModule, SelectModule, TagModule, TooltipModule,
    ToastModule, ConfirmDialogModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './vacancies-list.component.html'
})
export class VacanciesListComponent implements OnInit {
  private svc = inject(RecruitmentService);
  private lookupSvc = inject(LookupService);
  private fb = inject(FormBuilder);
  private msg = inject(MessageService);
  private confirm = inject(ConfirmationService);

  vacancies = signal<Vacancy[]>([]);
  jobs = signal<Job[]>([]);
  departments = signal<Department[]>([]);
  loading = signal(false);
  showDialog = false;
  showViewDialog = false;
  selectedVacancy: Vacancy | null = null;
  isEdit = false;
  editId?: number;
  submitted = false;
  form!: FormGroup;

  ngOnInit() {
    this.buildForm();
    this.load();
    this.loadLookups();
  }

  loadLookups() {
    this.lookupSvc.getJobs().subscribe(res => this.jobs.set(res));
    this.lookupSvc.getDepartments().subscribe(res => this.departments.set(res));
  }

  buildForm() {
    this.form = this.fb.group({
      jobId:             [null, [Validators.required, Validators.min(1)]],
      departmentId:      [null, [Validators.required, Validators.min(1)]],
      numberOfPositions: [1,    [Validators.required, Validators.min(1)]],
      description:       ['',   Validators.required],
      requirements:      ['',   Validators.required],
      closingDate:       [null, Validators.required],
    });
  }

  load() {
    this.loading.set(true);
    this.svc.getVacancies().subscribe({
      next: r => { if (r.succeeded) this.vacancies.set(r.data); this.loading.set(false); },
      error: () => { this.loading.set(false); this.err('فشل تحميل الشواغر'); }
    });
  }

  openNew() {
    this.isEdit = false;
    this.editId = undefined;
    this.submitted = false;
    this.form.reset({ numberOfPositions: 1 });
    this.showDialog = true;
  }

  edit(v: Vacancy) {
    this.isEdit = true;
    this.editId = v.vacancyId;
    this.submitted = false;
    this.form.patchValue({
      jobId: v.jobId,
      departmentId: v.deptId,
      numberOfPositions: v.positionsCount,
      description: v.description ?? '',
      requirements: v.requirements ?? '',
      closingDate: v.closingDate ? new Date(v.closingDate) : null
    });
    this.showDialog = true;
  }

  view(v: Vacancy) {
    this.selectedVacancy = v;
    this.showViewDialog = true;
  }

  save() {
    this.submitted = true;
    if (this.form.invalid) return;
    const v = this.form.value;
    const closingDate = new Date(v.closingDate).toISOString();

    if (this.isEdit && this.editId) {
      const cmd: UpdateVacancyCommand = { ...v, vacancyId: this.editId, closingDate };
      this.svc.updateVacancy(this.editId, cmd).subscribe(r => {
        if (r.succeeded) { this.ok('تم تحديث الشاغر بنجاح'); this.showDialog = false; this.load(); }
        else this.err(r.message);
      });
    } else {
      const cmd: CreateVacancyCommand = { ...v, closingDate };
      this.svc.createVacancy(cmd).subscribe(r => {
        if (r.succeeded) { this.ok('تم نشر الشاغر بنجاح'); this.showDialog = false; this.load(); }
        else this.err(r.message);
      });
    }
  }

  close(id: number) {
    this.confirm.confirm({
      message: 'هل تريد إغلاق هذا الشاغر الوظيفي؟ لن يتمكن أحد من التقديم بعد ذلك.',
      header: 'إغلاق الشاغر',
      icon: 'pi pi-lock',
      acceptLabel: 'نعم، أغلق',
      rejectLabel: 'إلغاء',
      acceptButtonStyleClass: 'p-button-warning',
      accept: () => this.svc.closeVacancy(id).subscribe(r => {
        if (r.succeeded) { this.ok('تم إغلاق الشاغر'); this.load(); }
        else this.err(r.message);
      })
    });
  }

  delete(id: number) {
    this.confirm.confirm({
      message: 'هل أنت متأكد من حذف هذا الشاغر نهائياً؟',
      header: 'تأكيد الحذف',
      icon: 'pi pi-trash',
      acceptLabel: 'حذف نهائي',
      rejectLabel: 'إلغاء',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.svc.deleteVacancy(id).subscribe(r => {
        if (r.succeeded) { this.ok('تم حذف الشاغر'); this.load(); }
        else this.err(r.message);
      })
    });
  }

  severity(status: string): TagSeverity {
    const map: Record<string, TagSeverity> = { ACTIVE: 'success', CLOSED: 'danger', DRAFT: 'warn' };
    return map[status?.toUpperCase()] ?? 'info';
  }

  labelAr(status: string) {
    const map: Record<string, string> = { ACTIVE: 'نشط', CLOSED: 'مغلق', DRAFT: 'مسودة' };
    return map[status?.toUpperCase()] ?? status;
  }

  private ok(msg: string) { this.msg.add({ severity: 'success', summary: 'تم بنجاح', detail: msg }); }
  private err(msg: string) { this.msg.add({ severity: 'error', summary: 'خطأ', detail: msg }); }
}
