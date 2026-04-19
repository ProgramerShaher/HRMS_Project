import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';

// PrimeNG
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { ProgressBarModule } from 'primeng/progressbar';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { DividerModule } from 'primeng/divider';
import { SkeletonModule } from 'primeng/skeleton';

// Services & Models
import { PerformanceService } from '../../services/performance.service';
import { AttendanceService } from '../../../attendance/services/attendance.service';
import { EmployeeAppraisal, AppraisalDetail, SubmitManagerAppraisalCommand, FinalizeAppraisalCommand } from '../../models/performance.model';
import { TimesheetDayDto } from '../../../attendance/models/attendance.models';
import { AuthService } from '../../../../core/auth/services/auth.service';

@Component({
  selector: 'app-appraisal-execution',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    CardModule,
    TableModule,
    ButtonModule,
    InputNumberModule,
    InputTextModule,
    TextareaModule,
    ProgressBarModule,
    TagModule,
    ToastModule,
    DividerModule,
    SkeletonModule
  ],
  providers: [MessageService],
  templateUrl: './appraisal-execution.component.html',
  styleUrls: ['./appraisal-execution.component.css']
})
export class AppraisalExecutionComponent implements OnInit {
  private route = inject(ActivatedRoute);
  public router = inject(Router);
  private performanceService = inject(PerformanceService);
  private attendanceService = inject(AttendanceService);
  private messageService = inject(MessageService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  // State
  appraisal = signal<EmployeeAppraisal | null>(null);
  attendance = signal<TimesheetDayDto[]>([]);
  violations = signal<any[]>([]); // Using any for now or EmployeeViolation
  loading = signal<boolean>(true);
  submitting = signal<boolean>(false);
  
  // Phase mapping for logic
  isManagerPhase = computed(() => this.appraisal()?.status === 'MANAGER_EVALUATION');
  isHRPhase = computed(() => this.appraisal()?.status === 'HR_REVIEW');
  isReadOnly = computed(() => {
    const a = this.appraisal();
    if (!a || a.status === 'COMPLETED') return true;
    
    const user = this.authService.currentUser();
    const roles = user?.roles || [];
    const isHR = roles.includes('HR_Manager') || roles.includes('System_Admin');
    
    // Logic: If in HR Phase, only HR can edit. If in Manager phase, only managers can edit.
    if (a.status === 'MANAGER_EVALUATION' && !roles.includes('Manager') && !isHR) return true;
    if (a.status === 'HR_REVIEW' && !isHR) return true;
    if (a.status === 'SELF_EVALUATION') return true;
    
    return false;
  });

  canSubmit = computed(() => {
     const a = this.appraisal();
     if (!a || this.isReadOnly()) return false;
     
     const roles = this.authService.currentUser()?.roles || [];
     if (a.status === 'MANAGER_EVALUATION') return roles.includes('Manager') || roles.includes('System_Admin') || roles.includes('HR_Manager');
     if (a.status === 'HR_REVIEW') return roles.includes('HR_Manager') || roles.includes('System_Admin');
     return false;
  });

  // Form
  form!: FormGroup;

  ngOnInit() {
    const id = this.route.snapshot.params['id'];
    if (id) {
      this.loadAppraisal(id);
    }
  }

  initForm(appraisal: EmployeeAppraisal) {
    this.form = this.fb.group({
      comments: [appraisal.comments || ''],
      scores: this.fb.array(appraisal.details.map(d => this.createScoreGroup(d)))
    });

    // Auto-Calculate listener
    this.form.valueChanges.subscribe(() => {
      this.calculateRealtimeResults();
    });
  }

  get scoreDetails(): FormArray {
    return this.form.get('scores') as FormArray;
  }

  createScoreGroup(d: AppraisalDetail): FormGroup {
    const group = this.fb.group({
      detailId: [d.detailId],
      kpiName: [d.kpiName],
      kpiCategory: [d.kpiCategory],
      weight: [d.weight],
      employeeScore: [d.employeeScore],
      managerScore: [d.managerScore || 0, [Validators.min(0), Validators.max(5)]], // Scale 1-5 as per user request
      finalScore: [d.finalScore || 0, [Validators.min(0), Validators.max(5)]],
      comments: [d.comments || '']
    });

    if (this.isReadOnly()) group.disable();
    return group;
  }

  loadAppraisal(id: number) {
    this.loading.set(true);
    this.performanceService.getAppraisalById(id).subscribe(res => {
      if (res.succeeded) {
        this.appraisal.set(res.data);
        this.initForm(res.data);
        this.loadAutomatedMetrics(res.data);
      } else {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
        this.router.navigate(['/performance/appraisals']);
      }
    });
  }

  loadAutomatedMetrics(a: EmployeeAppraisal) {
    // Fetch Attendance
    if (a.cycleStartDate && a.cycleEndDate) {
      this.attendanceService.getAttendanceByRange(a.employeeId, new Date(a.cycleStartDate), new Date(a.cycleEndDate))
        .subscribe(res => {
          if (res.succeeded) this.attendance.set(res.data);
        });
        
      // Fetch Violations
      this.performanceService.getViolations(a.employeeId, 'APPROVED').subscribe(res => {
        if (res.succeeded) {
          // Filter by cycle dates manually for precision
          const start = new Date(a.cycleStartDate!);
          const end = new Date(a.cycleEndDate!);
          this.violations.set(res.data.filter(v => {
            const d = new Date(v.violationDate);
            return d >= start && d <= end;
          }));
        }
      });
    }
    this.loading.set(false);
  }

  // Real-time calculation logic
  preliminaryTotal = signal<number>(0);
  
  calculateRealtimeResults() {
    const scores = this.scoreDetails.getRawValue();
    let total = 0;
    scores.forEach((s: any) => {
      // Score 1-5 maps to percentage (Score/5 * 100)
      const input = this.isHRPhase() ? s.finalScore : s.managerScore;
      const percentage = (input / 5) * 100;
      total += (percentage * s.weight) / 100;
    });
    this.preliminaryTotal.set(Math.round(total * 100) / 100);
  }

  submit() {
    const a = this.appraisal();
    if (!a || this.form.invalid) {
        this.messageService.add({severity: 'warn', summary: 'تنبيه', detail: 'يرجى مراجعة البيانات المدخلة'});
        return;
    }

    this.submitting.set(true);
    const val = this.form.getRawValue();

    if (this.isManagerPhase()) {
      const cmd: SubmitManagerAppraisalCommand = {
        scores: val.scores.map((s: any) => ({
          detailId: s.detailId,
          managerScore: (s.managerScore / 5) * 100, // Convert to backend percentage
          comments: s.comments
        })),
        managerComment: val.comments
      };
      this.performanceService.submitManagerAppraisal(a.appraisalId, cmd).subscribe(this.handleResponse());
    } 
    else if (this.isHRPhase()) {
      const cmd: FinalizeAppraisalCommand = {
        scores: val.scores.map((s: any) => ({
          detailId: s.detailId,
          finalScore: (s.finalScore / 5) * 100
        }))
      };
      this.performanceService.finalizeAppraisal(a.appraisalId, cmd).subscribe(this.handleResponse());
    }
  }

  private handleResponse() {
    return {
      next: (res: any) => {
        this.submitting.set(false);
        if (res.succeeded) {
          this.messageService.add({ severity: 'success', summary: 'تم بنجاح', detail: 'تم حفظ واعتماد التقييم بنجاح' });
          setTimeout(() => this.router.navigate(['/performance/appraisals']), 1500);
        } else {
          this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
        }
      },
      error: () => this.submitting.set(false)
    };
  }

  getStatusColor(status: string) {
      if (status === 'PRESENT') return 'success';
      if (status === 'ABSENT') return 'danger';
      if (status === 'LATE') return 'warn';
      return 'info';
  }
}
