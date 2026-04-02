import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ToastModule } from 'primeng/toast';
import { AvatarModule } from 'primeng/avatar';
import { ConfirmationService, MessageService } from 'primeng/api';
import { RecruitmentService } from '../../../services/recruitment.service';
import { LookupService, Country } from '../../../../../core/services/lookup.service';
import { Candidate, CreateCandidateCommand, UpdateCandidateCommand } from '../../../models/recruitment.models';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SelectModule } from 'primeng/select';
import { FileUploadModule } from 'primeng/fileupload';

@Component({
  selector: 'app-candidates-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    TableModule, ButtonModule, DialogModule,
    InputTextModule, TagModule, TooltipModule,
    ToastModule, AvatarModule, ConfirmDialogModule,
    SelectModule, FileUploadModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './candidates-list.component.html'
})
export class CandidatesListComponent implements OnInit {
  private svc = inject(RecruitmentService);
  private lookupSvc = inject(LookupService);
  private fb = inject(FormBuilder);
  private msg = inject(MessageService);
  private confirm = inject(ConfirmationService);

  candidates = signal<Candidate[]>([]);
  nationalities = signal<Country[]>([]);
  loading = signal(false);
  showDialog = false;
  showViewDialog = false;
  selectedCandidate: Candidate | null = null;
  isEdit = false;
  editId?: number;
  submitted = false;
  form!: FormGroup;
  cvFile: File | null = null;

  ngOnInit() {
    this.buildForm();
    this.load();
    this.loadLookups();
  }

  loadLookups() {
    this.lookupSvc.getCountries().subscribe(res => this.nationalities.set(res));
  }

  buildForm() {
    this.form = this.fb.group({
      fullNameEn:      ['', [Validators.required, Validators.maxLength(200)]],
      firstNameAr:     ['', Validators.maxLength(50)],
      familyNameAr:    ['', Validators.maxLength(50)],
      email:           ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      phone:           ['', [Validators.required, Validators.pattern(/^[0-9+\-\s()]+$/)]],
      nationalityId:   [null],
      linkedinProfile: [''],
    });
  }

  load() {
    this.loading.set(true);
    this.svc.getCandidates().subscribe({
      next: r => { if (r.succeeded) this.candidates.set(r.data); this.loading.set(false); },
      error: () => { this.loading.set(false); this.err('فشل تحميل المرشحين'); }
    });
  }

  openNew() {
    this.isEdit = false;
    this.editId = undefined;
    this.submitted = false;
    this.cvFile = null;
    this.form.reset();
    this.showDialog = true;
  }

  edit(c: Candidate) {
    this.isEdit = true;
    this.editId = c.candidateId;
    this.submitted = false;
    this.cvFile = null;
    this.form.patchValue({
      fullNameEn: c.fullNameEn,
      firstNameAr: c.firstNameAr,
      familyNameAr: c.familyNameAr,
      email: c.email,
      phone: c.phone,
      nationalityId: c.nationalityId,
      linkedinProfile: '', // Usually not show for privacy or not in DTO? Check entity if needed
    });
    this.showDialog = true;
  }

  view(c: Candidate) {
    this.selectedCandidate = c;
    this.showViewDialog = true;
  }

  delete(id: number) {
    this.confirm.confirm({
      message: 'هل أنت متأكد من حذف هذا المرشح نهائياً؟',
      header: 'تأكيد الحذف',
      icon: 'pi pi-trash',
      acceptLabel: 'حذف',
      rejectLabel: 'إلغاء',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.svc.deleteCandidate(id).subscribe(r => {
        if (r.succeeded) { this.ok('تم حذف المرشح'); this.load(); }
        else this.err(r.message);
      })
    });
  }

  onFileSelect(event: any) {
    this.cvFile = event.files[0];
  }

  fillSocial(platform: string) {
    let url = '';
    switch (platform) {
      case 'linkedin': url = 'https://linkedin.com/in/'; break;
      case 'github':   url = 'https://github.com/'; break;
      case 'facebook': url = 'https://facebook.com/'; break;
      case 'instagram':url = 'https://instagram.com/'; break;
      case 'x':        url = 'https://x.com/'; break;
    }
    this.form.patchValue({ linkedinProfile: url });
  }

  getSourceIcon(url?: string): string {
    if (!url) return 'pi-id-card';
    const s = url.toLowerCase();
    if (s.includes('linkedin')) return 'pi-linkedin';
    if (s.includes('github')) return 'pi-github';
    if (s.includes('facebook')) return 'pi-facebook';
    if (s.includes('instagram')) return 'pi-instagram';
    if (s.includes('x.com') || s.includes('twitter')) return 'pi-twitter';
    return 'pi-link';
  }

  getSourceLabel(url?: string): string {
    if (!url) return 'مباشر';
    const s = url.toLowerCase();
    if (s.includes('linkedin')) return 'LinkedIn';
    if (s.includes('github')) return 'GitHub';
    if (s.includes('facebook')) return 'Facebook';
    if (s.includes('instagram')) return 'Instagram';
    if (s.includes('x.com') || s.includes('twitter')) return 'X / Twitter';
    return 'رابط خارجي';
  }

  save() {
    this.submitted = true;
    if (this.form.invalid) {
      this.msg.add({ severity: 'warn', summary: 'تنبيه', detail: 'يرجى التأكد من صحة كافة البيانات المدخلة' });
      return;
    }

    if (this.isEdit && this.editId) {
      const cmd: UpdateCandidateCommand = {
        candidateId: this.editId,
        ...this.form.value
      };
      this.svc.updateCandidate(this.editId, cmd).subscribe(r => {
        if (r.succeeded) { this.ok('تم تحديث بيانات المرشح'); this.showDialog = false; this.load(); }
        else this.err(r.message);
      });
    } else {
      const fd = new FormData();
      // Ensure PascalCase keys to match Backend Command properties exactly
      const v = this.form.value;
      fd.append('FullNameEn', v.fullNameEn);
      if (v.firstNameAr) fd.append('FirstNameAr', v.firstNameAr);
      if (v.familyNameAr) fd.append('FamilyNameAr', v.familyNameAr);
      fd.append('Email', v.email);
      fd.append('Phone', v.phone);
      if (v.nationalityId) fd.append('NationalityId', String(v.nationalityId));
      if (v.linkedinProfile) fd.append('LinkedinProfile', v.linkedinProfile);
      if (this.cvFile) fd.append('CvFile', this.cvFile);

      this.svc.createCandidate(fd).subscribe(r => {
        if (r.succeeded) { 
          this.ok('تم تسجيل المرشح بنجاح'); 
          this.showDialog = false; 
          setTimeout(() => this.load(), 300); // Small delay to ensure DB consistency
        }
        else this.err(r.message);
      });
    }
  }

  initials(name: string) {
    return name?.split(' ').slice(0, 2).map(w => w[0]?.toUpperCase()).join('') ?? '??';
  }

  statusLabel(s: string) {
    const map: Record<string, string> = { ACTIVE: 'نشط', HIRED: 'تم توظيفه', BLACKLISTED: 'محظور' };
    return map[s?.toUpperCase()] ?? s;
  }

  statusSeverity(s: string): 'success' | 'danger' | 'warn' | 'info' {
    const map: Record<string, 'success' | 'danger' | 'warn' | 'info'> = {
      ACTIVE: 'success', HIRED: 'info', BLACKLISTED: 'danger'
    };
    return map[s?.toUpperCase()] ?? 'info';
  }

  private ok(msg: string) { this.msg.add({ severity: 'success', summary: 'تم بنجاح', detail: msg }); }
  private err(msg: string) { this.msg.add({ severity: 'error', summary: 'خطأ', detail: msg }); }
}
