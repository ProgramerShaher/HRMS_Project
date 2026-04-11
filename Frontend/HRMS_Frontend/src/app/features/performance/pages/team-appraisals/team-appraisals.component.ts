import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';

// PrimeNG
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';

import { PerformanceService } from '../../services/performance.service';
import { EmployeeAppraisal, SubmitManagerAppraisalCommand } from '../../models/performance.model';
import { AuthService } from '../../../../core/auth/services/auth.service';

@Component({
  selector: 'app-team-appraisals',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    InputNumberModule,
    TextareaModule,
    ToastModule,
    ConfirmDialogModule,
    TagModule,
    TooltipModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './team-appraisals.component.html'
})
export class TeamAppraisalsComponent implements OnInit {
  private performanceService = inject(PerformanceService);
  private authService = inject(AuthService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  appraisals = signal<EmployeeAppraisal[]>([]);
  loading = signal<boolean>(false);

  showReviewDialog = false;
  reviewForm!: FormGroup;
  selectedAppraisal: EmployeeAppraisal | null = null;

  ngOnInit() {
    this.initForm();
    this.loadData();
  }

  initForm() {
    this.reviewForm = this.fb.group({
      managerComment: [''],
      scores: this.fb.array([])
    });
  }

  get scoreDetails(): FormArray {
    return this.reviewForm.get('scores') as FormArray;
  }

  loadData() {
    this.loading.set(true);
    // Backend GetAppraisalsQuery filters by manager automatically if user is a manager
    this.performanceService.getAppraisals().subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.succeeded) {
          // Filter to show only pending manager evaluation or completed for their team
          this.appraisals.set(res.data);
        }
      },
      error: () => this.loading.set(false)
    });
  }

  openReview(appraisal: EmployeeAppraisal) {
    if (appraisal.status !== 'MANAGER_EVALUATION') {
      this.messageService.add({ 
        severity: 'warn', 
        summary: 'تنبيه', 
        detail: 'هذا التقييم ليس في مرحلة مراجعة المدير حالياً.' 
      });
      return;
    }

    this.selectedAppraisal = appraisal;
    this.showReviewDialog = true;
    this.reviewForm.reset();
    this.scoreDetails.clear();
    
    this.reviewForm.patchValue({ managerComment: '' });

    appraisal.details.forEach(d => {
      this.scoreDetails.push(this.fb.group({
        detailId: [d.detailId],
        kpiName: [d.kpiName],
        employeeScore: [{ value: d.employeeScore || 0, disabled: true }],
        employeeComments: [{ value: d.comments || '', disabled: true }],
        managerScore: [d.employeeScore || 0, [Validators.required, Validators.min(0), Validators.max(100)]],
        managerComments: ['']
      }));
    });
  }

  submitReview() {
    if (this.reviewForm.invalid || !this.selectedAppraisal) return;

    const val = this.reviewForm.value;
    const cmd: SubmitManagerAppraisalCommand = {
      scores: val.scores.map((s: any) => ({
        detailId: s.detailId,
        managerScore: s.managerScore,
        comments: s.managerComments
      })),
      managerComment: val.managerComment
    };

    this.performanceService.submitManagerAppraisal(this.selectedAppraisal.appraisalId, cmd).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.messageService.add({ 
            severity: 'success', 
            summary: 'نجاح', 
            detail: 'تم إرسال تقييمك بنجاح للموارد البشرية.' 
          });
          this.showReviewDialog = false;
          this.loadData();
        } else {
          this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
        }
      }
    });
  }
}
