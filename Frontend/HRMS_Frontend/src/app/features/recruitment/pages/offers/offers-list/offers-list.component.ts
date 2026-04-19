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
import { JobOffer, JobApplication, JobGrade, CreateOfferCommand, AcceptOfferCommand } from '../../../models/recruitment.models';

type TagSeverity = 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast';

@Component({
  selector: 'app-offers-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    TableModule, ButtonModule, DialogModule,
    SelectModule, InputNumberModule, TextareaModule, DatePickerModule,
    TagModule, TooltipModule, ToastModule, ConfirmDialogModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './offers-list.component.html'
})
export class OffersListComponent implements OnInit {
  private svc = inject(RecruitmentService);
  private fb = inject(FormBuilder);
  private msg = inject(MessageService);
  private confirm = inject(ConfirmationService);

  offers = signal<JobOffer[]>([]);
  applications = signal<JobApplication[]>([]);
  jobGrades = signal<JobGrade[]>([]);
  loading = signal(false);

  showCreateDialog = false;
  submittedCreate = false;
  createForm!: FormGroup;

  // Accept dialog
  showAcceptDialog = false;
  submittedAccept = false;
  acceptForm!: FormGroup;
  acceptingOfferId: number | null = null;
  acceptingCandidateName = '';

  // Computed total package for display
  totalPackage = signal(0);

  ngOnInit() {
    this.buildForm();
    this.load();
    this.loadLookups();
  }

  buildForm() {
    this.createForm = this.fb.group({
      appId:              [null, [Validators.required, Validators.min(1)]],
      jobGradeId:         [null, [Validators.required, Validators.min(1)]],
      basicSalary:        [0,    [Validators.required, Validators.min(1)]],
      housingAllowance:   [0,    [Validators.required, Validators.min(0)]],
      transportAllowance: [0,    [Validators.required, Validators.min(0)]],
      medicalAllowance:   [0,    [Validators.required, Validators.min(0)]],
      otherAllowances:    [0,    [Validators.required, Validators.min(0)]],
      offerDate:          [new Date(), Validators.required],
      expiryDate:         [null, Validators.required],
      joiningDate:        [null, Validators.required],
      terms:              [''],
    });

    this.acceptForm = this.fb.group({
      joiningDate:   [null],
      nationalId:    [''],
      mobile:        [''],
      birthDate:     [null],
      gender:        ['M'],
      maritalStatus: ['Single'],
    });

    // React to salary changes to compute total
    this.createForm.valueChanges.subscribe(v => {
      const total = (v.basicSalary || 0) + (v.housingAllowance || 0) +
        (v.transportAllowance || 0) + (v.medicalAllowance || 0) + (v.otherAllowances || 0);
      this.totalPackage.set(total);
    });
  }

  load() {
    this.loading.set(true);
    this.svc.getOffers().subscribe({
      next: r => { if (r.succeeded) this.offers.set(r.data); this.loading.set(false); },
      error: () => { this.loading.set(false); this.err('فشل تحميل عروض العمل'); }
    });
  }

  loadLookups() {
    this.svc.getApplications(undefined, undefined, 'INTERVIEW').subscribe(r => {
      if (r.succeeded) this.applications.set(r.data);
    });
    this.svc.getJobGrades().subscribe(r => {
      if (r.succeeded) this.jobGrades.set(r.data);
    });
  }

  get appOptions() {
    return this.applications().map(a => ({
      label: `${a.candidateName} – ${a.vacancyTitle}`,
      value: a.applicationId
    }));
  }

  get gradeOptions() {
    return this.jobGrades().map(g => ({
      label: `${g.gradeNameAr} (${g.minSalary.toLocaleString()} – ${g.maxSalary.toLocaleString()})`,
      value: g.jobGradeId
    }));
  }

  openCreate() {
    this.submittedCreate = false;
    this.createForm.reset({ basicSalary: 0, housingAllowance: 0, transportAllowance: 0, medicalAllowance: 0, otherAllowances: 0, offerDate: new Date() });
    this.totalPackage.set(0);
    this.showCreateDialog = true;
  }

  saveCreate() {
    this.submittedCreate = true;
    if (this.createForm.invalid) return;
    const v = this.createForm.value;
    const cmd: CreateOfferCommand = {
      ...v,
      offerDate:   new Date(v.offerDate).toISOString(),
      expiryDate:  new Date(v.expiryDate).toISOString(),
      joiningDate: new Date(v.joiningDate).toISOString(),
    };
    this.svc.createOffer(cmd).subscribe(r => {
      if (r.succeeded) {
        this.ok(r.message || 'تم إنشاء عرض العمل بنجاح');
        this.showCreateDialog = false;
        this.load();
      } else this.err(r.message);
    });
  }

  accept(offerId: number, candidateName: string) {
    this.acceptingOfferId = offerId;
    this.acceptingCandidateName = candidateName;
    this.submittedAccept = false;
    this.acceptForm.reset({ gender: 'M', maritalStatus: 'Single' });
    this.showAcceptDialog = true;
  }

  saveAccept() {
    if (!this.acceptingOfferId) return;
    const v = this.acceptForm.value;
    const cmd: AcceptOfferCommand = {
      offerId: this.acceptingOfferId,
      joiningDate:   v.joiningDate   ? new Date(v.joiningDate).toISOString()  : undefined,
      birthDate:     v.birthDate     ? new Date(v.birthDate).toISOString()    : undefined,
      nationalId:    v.nationalId    || undefined,
      mobile:        v.mobile        || undefined,
      gender:        v.gender        || undefined,
      maritalStatus: v.maritalStatus || undefined,
    };
    this.svc.acceptOffer(this.acceptingOfferId, cmd).subscribe(r => {
      if (r.succeeded) {
        this.ok(r.message || 'تم توظيف المرشح بنجاح 🎉');
        this.showAcceptDialog = false;
        this.load();
      } else this.err(r.message);
    });
  }

  withdraw(offerId: number) {
    this.confirm.confirm({
      message: 'هل تريد سحب هذا العرض؟',
      header: 'سحب العرض',
      icon: 'pi pi-times-circle',
      acceptLabel: 'نعم، اسحب',
      rejectLabel: 'تراجع',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.svc.withdrawOffer(offerId).subscribe(r => {
        if (r.succeeded) { this.ok('تم سحب العرض'); this.load(); }
        else this.err(r.message);
      })
    });
  }

  severity(s: string): TagSeverity {
    const map: Record<string, TagSeverity> = {
      PENDING: 'warn', SENT: 'info',
      ACCEPTED: 'success', REJECTED: 'danger', WITHDRAWN: 'secondary'
    };
    return map[s?.toUpperCase()] ?? 'info';
  }

  statusLabel(s: string) {
    const map: Record<string, string> = {
      PENDING: 'بانتظار الإرسال', SENT: 'تم الإرسال',
      ACCEPTED: 'مقبول', REJECTED: 'مرفوض', WITHDRAWN: 'مسحوب'
    };
    return map[s?.toUpperCase()] ?? s;
  }

  private ok(detail: string) { this.msg.add({ severity: 'success', summary: 'تم بنجاح', detail }); }
  private err(detail: string) { this.msg.add({ severity: 'error', summary: 'خطأ', detail }); }
}
