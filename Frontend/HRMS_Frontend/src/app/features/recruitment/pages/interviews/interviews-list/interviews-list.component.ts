import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { InputNumberModule } from 'primeng/inputnumber';
import { TextareaModule } from 'primeng/textarea';
import { DatePickerModule } from 'primeng/datepicker';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { RecruitmentService } from '../../../services/recruitment.service';
import { Interview, ScheduleInterviewCommand, RecordInterviewResultCommand, JobApplication } from '../../../models/recruitment.models';

type TagSeverity = 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast';

@Component({
  selector: 'app-interviews-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    TableModule, ButtonModule, DialogModule,
    SelectModule, InputNumberModule, TextareaModule, DatePickerModule,
    TagModule, TooltipModule, ToastModule, ConfirmDialogModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './interviews-list.component.html'
})
export class InterviewsListComponent implements OnInit {
  private svc = inject(RecruitmentService);
  private fb = inject(FormBuilder);
  private msg = inject(MessageService);
  private confirm = inject(ConfirmationService);

  interviews = signal<Interview[]>([]);
  applications = signal<JobApplication[]>([]);
  loading = signal(false);

  showScheduleDialog = false;
  showResultDialog = false;
  selectedInterview: Interview | null = null;
  submittedSchedule = false;
  submittedResult = false;

  scheduleForm!: FormGroup;
  resultForm!: FormGroup;

  readonly interviewTypeOptions = [
    { label: 'حضوري',   value: 'IN_PERSON' },
    { label: 'عبر الإنترنت', value: 'ONLINE' },
    { label: 'هاتفي',   value: 'PHONE' },
  ];

  readonly resultOptions = [
    { label: 'اجتاز ✓',          value: 'PASSED' },
    { label: 'لم يجتز ✗',       value: 'FAILED' },
    { label: 'لم يحضر',         value: 'NO_SHOW' },
  ];

  readonly ratingOptions = [
    { label: '1 - ضعيف جداً',   value: 1 },
    { label: '2 - ضعيف',        value: 2 },
    { label: '3 - مقبول',       value: 3 },
    { label: '4 - جيد',         value: 4 },
    { label: '5 - ممتاز',       value: 5 },
  ];

  ngOnInit() {
    this.buildForms();
    this.load();
    this.loadApplications();
  }

  buildForms() {
    this.scheduleForm = this.fb.group({
      appId:         [null, [Validators.required, Validators.min(1)]],
      interviewerId: [null],
      scheduledTime: [null, Validators.required],
      interviewType: ['IN_PERSON', Validators.required],
    });
    this.resultForm = this.fb.group({
      result:  ['', Validators.required],
      rating:  [null, [Validators.min(1), Validators.max(5)]],
      notes:   [''],
    });
  }

  load() {
    this.loading.set(true);
    this.svc.getInterviews().subscribe({
      next: r => { if (r.succeeded) this.interviews.set(r.data); this.loading.set(false); },
      error: () => { this.loading.set(false); this.err('فشل تحميل المقابلات'); }
    });
  }

  loadApplications() {
    this.svc.getApplications(undefined, undefined, 'SHORTLISTED').subscribe(r => {
      if (r.succeeded) this.applications.set(r.data);
    });
  }

  get appOptions() {
    return this.applications().map(a => ({
      label: `${a.candidateName} – ${a.vacancyTitle} (#${a.applicationId})`,
      value: a.applicationId
    }));
  }

  openSchedule() {
    this.submittedSchedule = false;
    this.scheduleForm.reset({ interviewType: 'IN_PERSON' });
    this.showScheduleDialog = true;
  }

  saveSchedule() {
    this.submittedSchedule = true;
    if (this.scheduleForm.invalid) return;
    const v = this.scheduleForm.value;
    const cmd: ScheduleInterviewCommand = {
      ...v,
      scheduledTime: new Date(v.scheduledTime).toISOString()
    };
    this.svc.scheduleInterview(cmd).subscribe(r => {
      if (r.succeeded) {
        this.ok('تم جدولة المقابلة بنجاح');
        this.showScheduleDialog = false;
        this.load();
      } else this.err(r.message);
    });
  }

  openResult(interview: Interview) {
    this.selectedInterview = interview;
    this.submittedResult = false;
    this.resultForm.reset({ score: interview.score, feedback: interview.feedback });
    this.showResultDialog = true;
  }

  saveResult() {
    this.submittedResult = true;
    if (this.resultForm.invalid || !this.selectedInterview) return;
    const cmd: RecordInterviewResultCommand = {
      interviewId: this.selectedInterview.interviewId,
      result:  this.resultForm.value.result,
      rating:  this.resultForm.value.rating || undefined,
      notes:   this.resultForm.value.notes  || undefined,
    };
    this.svc.recordInterviewResult(this.selectedInterview.interviewId, cmd).subscribe(r => {
      if (r.succeeded) {
        this.ok('تم تسجيل نتيجة المقابلة');
        this.showResultDialog = false;
        this.load();
      } else this.err(r.message);
    });
  }

  cancel(id: number) {
    this.confirm.confirm({
      message: 'هل تريد إلغاء هذه المقابلة؟',
      header: 'إلغاء المقابلة',
      icon: 'pi pi-calendar-times',
      acceptLabel: 'نعم، ألغِ',
      rejectLabel: 'تراجع',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.svc.cancelInterview(id).subscribe(r => {
        if (r.succeeded) { this.ok('تم إلغاء المقابلة'); this.load(); }
        else this.err(r.message);
      })
    });
  }

  typeLabel(t: string) {
    const map: Record<string, string> = { IN_PERSON: 'حضوري', ONLINE: 'عبر الإنترنت', PHONE: 'هاتفي' };
    return map[t] ?? t;
  }

  statusSeverity(s: string): TagSeverity {
    const map: Record<string, TagSeverity> = {
      SCHEDULED: 'info', COMPLETED: 'secondary',
      PASSED: 'success', FAILED: 'danger', CANCELLED: 'warn'
    };
    return map[s?.toUpperCase()] ?? 'info';
  }

  statusLabel(s: string) {
    const map: Record<string, string> = {
      SCHEDULED: 'مجدولة', COMPLETED: 'مكتملة',
      PASSED: 'اجتاز', FAILED: 'لم يجتز', CANCELLED: 'ملغية'
    };
    return map[s?.toUpperCase()] ?? s;
  }

  private ok(detail: string) { this.msg.add({ severity: 'success', summary: 'تم بنجاح', detail }); }
  private err(detail: string) { this.msg.add({ severity: 'error', summary: 'خطأ', detail }); }
}
