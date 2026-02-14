import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';

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
import { EmployeeAppraisal, SubmitAppraisalCommand, KpiDetailCommand } from '../../models/performance.model';
import { Kpi, AppraisalCycle } from '../../../setup/models/performance-setup.model';

@Component({
    selector: 'app-appraisals',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
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
    // Services
    private performanceService = inject(PerformanceService);
    private setupService = inject(PerformanceSetupService);
    private employeeService = inject(EmployeeService);
    private messageService = inject(MessageService);
    private confirmationService = inject(ConfirmationService);
    private fb = inject(FormBuilder);

    // State
    appraisals = signal<EmployeeAppraisal[]>([]);
    cycles = signal<AppraisalCycle[]>([]);
    kpis = signal<Kpi[]>([]);
    employees = signal<any[]>([]);
    loading = signal<boolean>(false);

    // Form
    showDialog = false;
    form!: FormGroup;
    isEditMode = false;
    selectedAppraisalId: number | null = null;
    submitted = false;

    ngOnInit() {
        this.initForm();
        this.loadLookups();
        this.loadData();
    }

    initForm() {
        this.form = this.fb.group({
            employeeId: [null, Validators.required],
            manualEvaluatorId: [null, Validators.required],
            cycleId: [null, Validators.required],
            employeeComment: [''],
            kpiDetails: this.fb.array([])
        });
    }

    get kpiDetails(): FormArray {
        return this.form.get('kpiDetails') as FormArray;
    }

    loadLookups() {
        this.setupService.getAppraisalCycles().subscribe(res => {
            if (res.succeeded) this.cycles.set(res.data);
        });

        this.setupService.getKpis().subscribe(res => {
            if (res.succeeded) this.kpis.set(res.data);
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

    loadData() {
        this.loading.set(true);
        this.performanceService.getAppraisals().subscribe(res => {
            this.loading.set(false);
            if (res.succeeded) {
                this.appraisals.set(res.data);
            }
        });
    }

    // When opening dialog, we must populate KPIs form array
    openNew() {
        this.showDialog = true;
        this.isEditMode = false;
        this.submitted = false;

        // Reset form but allow keeping KPIs structure?
        // Better to rebuild it fresh
        this.form.reset();
        this.kpiDetails.clear();

        // Add all active KPIs to the form so user can score them
        // Ideally this should be based on Job Role, but for now we load all from Library as per current simple logic
        this.kpis().forEach((kpi: Kpi) => {
            this.kpiDetails.push(this.fb.group({
                kpiId: [kpi.kpiId],
                kpiName: [kpi.kpiNameAr], // Display only
                score: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
                comments: ['']
            }));
        });
    }

    save() {
        this.submitted = true;
        if (this.form.invalid) return;

        const val = this.form.value;

        const detailsCommand: KpiDetailCommand[] = val.kpiDetails.map((d: any) => ({
            kpiId: d.kpiId,
            score: d.score,
            comments: d.comments
        }));

        const command: SubmitAppraisalCommand = {
            employeeId: val.employeeId,
            manualEvaluatorId: val.manualEvaluatorId,
            cycleId: val.cycleId,
            employeeComment: val.employeeComment,
            kpiDetails: detailsCommand
        };

        if (this.isEditMode && this.selectedAppraisalId) {
            this.performanceService.updateAppraisal(this.selectedAppraisalId, command).subscribe({
                next: (res) => {
                    if (res.succeeded) {
                        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Appraisal Updated' });
                        this.showDialog = false;
                        this.loadData();
                    } else {
                        this.messageService.add({ severity: 'error', summary: 'Error', detail: res.message });
                    }
                },
                error: (err) => {
                    const msg = err.error?.message || 'Failed to update appraisal';
                    this.messageService.add({ severity: 'error', summary: 'Error', detail: msg });
                }
            });
        } else {
            this.performanceService.submitAppraisal(command).subscribe({
                next: (res) => {
                    if (res.succeeded) {
                        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Appraisal Submitted' });
                        this.showDialog = false;
                        this.loadData();
                    } else {
                        this.messageService.add({ severity: 'error', summary: 'Error', detail: res.message });
                    }
                },
                error: (err) => {
                    const msg = err.error?.message || 'Failed to submit appraisal';
                    this.messageService.add({ severity: 'error', summary: 'Error', detail: msg });
                }
            });
        }
    }

    delete(appraisal: EmployeeAppraisal) {
        this.confirmationService.confirm({
            message: 'Are you sure you want to delete this appraisal?',
            header: 'Confirm Delete',
            icon: 'pi pi-trash',
            accept: () => {
                this.performanceService.deleteAppraisal(appraisal.appraisalId).subscribe(res => {
                    if (res.succeeded) {
                        this.messageService.add({ severity: 'success', summary: 'Deleted', detail: 'Appraisal deleted' });
                        this.loadData();
                    }
                });
            }
        });
    }
}
