import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

// PrimeNG
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';

import { PerformanceService } from '../../services/performance.service';
import { PerformanceSetupService } from '../../../setup/services/performance-setup.service';
import { EmployeeService } from '../../../personnel/services/employee.service';
import { EmployeeAppraisal, AppraisalDetail, InitiateAppraisalCommand, SubmitManagerAppraisalCommand, FinalizeAppraisalCommand } from '../../models/performance.model';
import { AppraisalCycle } from '../../../setup/models/performance-setup.model';
import { AuthService } from '../../../../core/auth/services/auth.service';
@Component({
    selector: 'app-appraisals',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        TableModule,
        ButtonModule,
        DialogModule,
        SelectModule,
        InputTextModule,
        InputNumberModule,
        TextareaModule,
        ToastModule,
        ConfirmDialogModule,
        TagModule,
        TooltipModule
    ],
    providers: [MessageService, ConfirmationService],
    templateUrl: './appraisals.component.html'
})
export class AppraisalsComponent implements OnInit {
    private performanceService = inject(PerformanceService);
    private setupService = inject(PerformanceSetupService);
    private employeeService = inject(EmployeeService);
    private authService = inject(AuthService);
    private messageService = inject(MessageService);
    private confirmationService = inject(ConfirmationService);
    private fb = inject(FormBuilder);
    private router = inject(Router);
    private route = inject(ActivatedRoute);

    appraisals = signal<EmployeeAppraisal[]>([]);
    cycles = signal<AppraisalCycle[]>([]);
    employees = signal<any[]>([]);
    loading = signal<boolean>(false);

    // Initiation Form
    showInitDialog = false;
    initForm!: FormGroup;

    // Review Form (Manager / HR)
    showReviewDialog = false;
    reviewForm!: FormGroup;
    selectedAppraisal: EmployeeAppraisal | null = null;
    currentPhaseActive = ''; // 'MANAGER_EVALUATION' or 'HR_REVIEW'

    isAdmin = signal<boolean>(false);

    ngOnInit() {
        const roles = this.authService.currentUser()?.roles || [];
        this.isAdmin.set(roles.includes('System_Admin') || roles.includes('HR_Manager'));

        this.initForms();
        this.loadLookups();

        // دعم الفلترة بالدورة عبر query params
        this.route.queryParams.subscribe(params => {
            const cycleId = params['cycleId'] ? +params['cycleId'] : undefined;
            this.loadData(cycleId);
        });
    }

    initForms() {
        this.initForm = this.fb.group({
            employeeId: [null, Validators.required],
            cycleId: [null, Validators.required],
            evaluatorId: [null]
        });

        this.reviewForm = this.fb.group({
            reviewComment: [''],
            scores: this.fb.array([])
        });
    }

    get scoreDetails(): FormArray {
        return this.reviewForm.get('scores') as FormArray;
    }

    loadLookups() {
        this.setupService.getAppraisalCycles().subscribe(res => {
            if (res.succeeded) this.cycles.set(res.data);
        });

        this.employeeService.getAll(1, 1000).subscribe(res => {
            if (res && res.items) {
                this.employees.set(res.items.map((e: any) => ({
                    label: `${e.fullNameAr || e.fullNameEn} (${e.employeeNumber})`,
                    value: e.employeeId
                })));
            }
        });
    }

    loadData(cycleId?: number) {
        this.loading.set(true);
        this.performanceService.getAppraisals(undefined, cycleId).subscribe(res => {
            this.loading.set(false);
            if (res.succeeded) this.appraisals.set(res.data);
        });
    }

    viewResult(appraisal: EmployeeAppraisal) {
        this.router.navigate(['/performance/appraisals', appraisal.appraisalId, 'result']);
    }

    // Phase: HR Initiates Appraisal
    openInitiate() {
        this.showInitDialog = true;
        this.initForm.reset();
    }

    submitInitiate() {
        if (this.initForm.invalid) return;

        const cmd: InitiateAppraisalCommand = this.initForm.value;
        this.performanceService.initiateAppraisal(cmd).subscribe({
            next: (res) => {
                if (res.succeeded) {
                    this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم تهيئة التقييم وإرساله للموظف' });
                    this.showInitDialog = false;
                    this.loadData();
                } else this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
            },
            error: (err) => this.messageService.add({ severity: 'error', summary: 'خطأ', detail: err.error?.message || 'فشل' })
        });
    }

    // Phase: Manager / HR Reviews
    openReview(appraisal: EmployeeAppraisal) {
        if (appraisal.status !== 'MANAGER_EVALUATION' && appraisal.status !== 'HR_REVIEW') {
            this.messageService.add({ severity: 'warn', summary: 'تنبيه', detail: 'التقييم ليس في مرحلة تقييم الإدارة حالياً.' });
            return;
        }

        this.selectedAppraisal = appraisal;
        this.currentPhaseActive = appraisal.status;
        this.showReviewDialog = true;
        
        this.reviewForm.reset();
        this.scoreDetails.clear();
        this.reviewForm.patchValue({ reviewComment: appraisal.comments || '' });

        // If it's MANAGER_EVALUATION, they edit managerScore based on employeeScore.
        // If it's HR_REVIEW, they edit finalScore based on managerScore.
        appraisal.details.forEach(d => {
            this.scoreDetails.push(this.fb.group({
                detailId: [d.detailId],
                kpiName: [d.kpiName],
                employeeScore: [{ value: d.employeeScore || 0, disabled: true }],
                managerScore: [{ value: d.managerScore || d.employeeScore || 0, disabled: appraisal.status === 'HR_REVIEW' }, Validators.required],
                finalScore: [d.finalScore || d.managerScore || d.employeeScore || 0, Validators.required],
                comments: ['']
            }));
        });
    }

    submitReview() {
        if (this.reviewForm.invalid || !this.selectedAppraisal) return;

        const val = this.reviewForm.value;

        if (this.currentPhaseActive === 'MANAGER_EVALUATION') {
            const cmd: SubmitManagerAppraisalCommand = {
                scores: val.scores.map((s: any) => ({
                    detailId: s.detailId,
                    managerScore: s.managerScore,
                    comments: s.comments
                })),
                managerComment: val.reviewComment
            };
            this.performanceService.submitManagerAppraisal(this.selectedAppraisal.appraisalId, cmd).subscribe({
                next: (res) => {
                    if (res.succeeded) {
                        this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم تسليم تقييم المدير للموارد البشرية' });
                        this.showReviewDialog = false;
                        this.loadData();
                    } else this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
                }
            });
        } 
        else if (this.currentPhaseActive === 'HR_REVIEW') {
            const cmd: FinalizeAppraisalCommand = {
                scores: val.scores.map((s: any) => ({
                    detailId: s.detailId,
                    finalScore: s.finalScore
                }))
            };
            this.performanceService.finalizeAppraisal(this.selectedAppraisal.appraisalId, cmd).subscribe({
                next: (res) => {
                    if (res.succeeded) {
                        this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم اعتماد التقييم النهائي' });
                        this.showReviewDialog = false;
                        this.loadData();
                    } else this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
                }
            });
        }
    }

    delete(appraisal: EmployeeAppraisal) {
        this.confirmationService.confirm({
            message: 'هل أنت متأكد من حذف التقييم؟',
            header: 'تأكيد החذف',
            icon: 'pi pi-trash',
            accept: () => {
                this.performanceService.deleteAppraisal(appraisal.appraisalId).subscribe(res => {
                    if (res.succeeded) {
                        this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'تم الحذف' });
                        this.loadData();
                    }
                });
            }
        });
    }
}
