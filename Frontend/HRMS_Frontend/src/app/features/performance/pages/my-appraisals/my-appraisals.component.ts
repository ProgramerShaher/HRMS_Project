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

import { PerformanceService } from '../../services/performance.service';
import { EmployeeAppraisal, SubmitSelfAppraisalCommand } from '../../models/performance.model';
import { AuthService } from '../../../../core/auth/services/auth.service';

@Component({
    selector: 'app-my-appraisals',
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
        TagModule
    ],
    providers: [MessageService, ConfirmationService],
    templateUrl: './my-appraisals.component.html'
})
export class MyAppraisalsComponent implements OnInit {
    private performanceService = inject(PerformanceService);
    private authService = inject(AuthService);
    private messageService = inject(MessageService);
    private fb = inject(FormBuilder);

    appraisals = signal<EmployeeAppraisal[]>([]);
    loading = signal<boolean>(false);

    showDialog = false;
    form!: FormGroup;
    selectedAppraisal: EmployeeAppraisal | null = null;

    ngOnInit() {
        this.initForm();
        this.loadData();
    }

    initForm() {
        this.form = this.fb.group({
            employeeComment: [''],
            scores: this.fb.array([])
        });
    }

    get scoreDetails(): FormArray {
        return this.form.get('scores') as FormArray;
    }

    loadData() {
        this.loading.set(true);
        // Ensure only my appraisals by fetching via current user employee ID
        const user = this.authService.currentUser();
        if (!user || (!user.employeeId && !user.roles.includes('System_Admin'))) {
            this.loading.set(false);
            return; 
        }

        const employeeId = user.employeeId || undefined; // If undefined, Backend will filter by currentUser anyways

        this.performanceService.getAppraisals(employeeId).subscribe(res => {
            this.loading.set(false);
            if (res.succeeded) {
                // To be completely safe and clean we show those where EmployeeId matches 
                // OR we just trust backend if it filtered correctly.
                this.appraisals.set(res.data.filter(a => a.employeeId === employeeId || employeeId === undefined));
            }
        });
    }

    openSelfEvaluation(appraisal: EmployeeAppraisal) {
        if (appraisal.status !== 'SELF_EVALUATION') {
            this.messageService.add({ severity: 'warn', summary: 'تنبيه', detail: 'هذا التقييم ليس متاحاً للتقييم الذاتي حالياً.' });
            return;
        }

        this.selectedAppraisal = appraisal;
        this.showDialog = true;
        this.form.reset();
        this.scoreDetails.clear();

        appraisal.details.forEach(d => {
            this.scoreDetails.push(this.fb.group({
                detailId: [d.detailId],
                kpiName: [d.kpiName],
                employeeScore: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
                comments: ['']
            }));
        });
    }

    submitSelfAppraisal() {
        if (this.form.invalid || !this.selectedAppraisal) return;

        const val = this.form.value;
        const cmd: SubmitSelfAppraisalCommand = {
            scores: val.scores.map((s: any) => ({
                detailId: s.detailId,
                employeeScore: s.employeeScore,
                comments: s.comments
            })),
            employeeComment: val.employeeComment
        };

        this.performanceService.submitSelfAppraisal(this.selectedAppraisal.appraisalId, cmd).subscribe({
            next: (res) => {
                if (res.succeeded) {
                    this.messageService.add({ severity: 'success', summary: 'النجاح', detail: 'تم إرسال التقييم الذاتي لمديرك للمراجعة.' });
                    this.showDialog = false;
                    this.loadData();
                } else this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
            }
        });
    }
}
