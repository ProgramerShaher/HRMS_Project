import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

// PrimeNG
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { TagModule } from 'primeng/tag';
import { RadioButtonModule } from 'primeng/radiobutton';

import { PerformanceService } from '../../services/performance.service';
import { PerformanceSetupService } from '../../../setup/services/performance-setup.service';
import { EmployeeService } from '../../../personnel/services/employee.service';
import { EmployeeViolation, RegisterViolationCommand } from '../../models/performance.model';
import { ViolationType, DisciplinaryAction } from '../../../setup/models/performance-setup.model';

@Component({
    selector: 'app-violations',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        TableModule,
        ButtonModule,
        DialogModule,
        SelectModule,
        DatePickerModule,
        TextareaModule,
        ToastModule,
        ConfirmDialogModule,
        TooltipModule,
        TagModule,
        RadioButtonModule
    ],
    providers: [MessageService, ConfirmationService],
    templateUrl: './violations.component.html'
})
export class ViolationsComponent implements OnInit {
    // Services
    private performanceService = inject(PerformanceService);
    private setupService = inject(PerformanceSetupService);
    private employeeService = inject(EmployeeService);
    private messageService = inject(MessageService);
    private confirmationService = inject(ConfirmationService);
    private fb = inject(FormBuilder);

    // State
    violations = signal<EmployeeViolation[]>([]);
    violationTypes = signal<ViolationType[]>([]);
    disciplinaryActions = signal<DisciplinaryAction[]>([]);
    employees = signal<any[]>([]);
    loading = signal<boolean>(false);

    // Form
    showDialog = false;
    form!: FormGroup;
    isEditMode = false;
    selectedViolationId: number | null = null;
    submitted = false;
    actionCategory = 'disciplinary'; // 'disciplinary' | 'amnesty'
    showActionDialog = false;
    currentViolation: EmployeeViolation | null = null;

    ngOnInit() {
        this.initForm();
        this.loadData();
        this.loadLookups();
    }

    initForm() {
        this.form = this.fb.group({
            employeeId: [null, Validators.required],
            violationTypeId: [null, Validators.required],
            violationDate: [new Date(), Validators.required],
            description: ['', [Validators.maxLength(500)]],
            // Action specific field
            actionId: [null]
        });
    }

    loadData(employeeId?: number) {
        this.loading.set(true);
        this.performanceService.getViolations(employeeId).subscribe(res => {
            this.loading.set(false);
            if (res.succeeded) {
                this.violations.set(res.data);
            }
        });
    }

    loadLookups() {
        this.setupService.getViolationTypes().subscribe(res => {
            if (res.succeeded) this.violationTypes.set(res.data);
        });

        this.setupService.getDisciplinaryActions().subscribe(res => {
            if (res.succeeded) this.disciplinaryActions.set(res.data);
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

    // Operations
    openNew() {
        this.form.reset({ violationDate: new Date() });
        this.actionCategory = 'disciplinary';
        this.isEditMode = false;
        this.showDialog = true;
        this.submitted = false;
    }

    save() {
        this.submitted = true;
        if (this.form.invalid) return;

        const val = this.form.value;
        const command: RegisterViolationCommand = {
            employeeId: val.employeeId,
            violationTypeId: val.violationTypeId,
            violationDate: val.violationDate,
            description: val.description
        };

        if (this.isEditMode && this.selectedViolationId) {
            this.performanceService.updateViolation(this.selectedViolationId, command).subscribe(res => {
                if (res.succeeded) {
                    this.messageService.add({ severity: 'success', summary: 'تم بنجاح', detail: 'تم تحديث البيانات' });
                    this.showDialog = false;
                    this.loadData();
                } else {
                    this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
                }
            });
        } else {
            this.performanceService.registerViolation(command).subscribe(res => {
                if (res.succeeded) {
                    this.messageService.add({ severity: 'success', summary: 'تم بنجاح', detail: 'تم تسجيل المخالفة بنجاح، يرجى تحديد الإجراء المقترح من القائمة' });
                    this.showDialog = false;
                    this.loadData();
                } else {
                    this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
                }
            });
        }
    }

    openActionDialog(violation: EmployeeViolation) {
        this.currentViolation = violation;
        this.actionCategory = violation.actionId ? 'disciplinary' : 'amnesty';
        this.form.patchValue({
            actionId: violation.actionId,
            description: violation.description
        });
        this.showActionDialog = true;
    }

    saveAction() {
        if (!this.currentViolation) return;

        const val = this.form.value;
        const actionId = this.actionCategory === 'amnesty' ? null : val.actionId;

        if (this.actionCategory === 'disciplinary' && !val.actionId) {
            this.messageService.add({ severity: 'warn', summary: 'تنبيه', detail: 'يرجى اختيار نوع الجزاء' });
            return;
        }

        const command: RegisterViolationCommand = {
            employeeId: this.currentViolation.employeeId,
            violationTypeId: this.currentViolation.violationTypeId,
            actionId: actionId,
            violationDate: this.currentViolation.violationDate,
            description: val.description
        };

        this.performanceService.updateViolation(this.currentViolation.violationId, command).subscribe(res => {
            if (res.succeeded) {
                this.messageService.add({ severity: 'success', summary: 'تم بنجاح', detail: 'تم تحديد الإجراء المقترح بنجاح' });
                this.showActionDialog = false;
                this.loadData();
            } else {
                this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
            }
        });
    }

    approve(violation: EmployeeViolation) {
        if (!violation.actionId && !violation.description?.includes('عفو')) {
            this.messageService.add({
                severity: 'warn',
                summary: 'لا يمكن الاعتماد',
                detail: 'يجب تحديد الإجراء المقترح (جزاء أو عفو) قبل الاعتماد'
            });
            return;
        }

        this.confirmationService.confirm({
            message: `هل أنت متأكد من اعتماد مخالفة "${violation.violationTypeNameAr}" للموظف ${violation.employeeName}؟ سيتم تطبيق الإجراء التالي: ${violation.actionNameAr || 'عفو إداري'}.`,
            header: 'تأكيد الاعتماد النهائي',
            icon: 'pi pi-check-circle',
            acceptLabel: 'اعتماد الآن',
            rejectLabel: 'إلغاء',
            acceptButtonStyleClass: 'p-button-success',
            accept: () => {
                this.performanceService.approveViolation(violation.violationId).subscribe(res => {
                    if (res.succeeded) {
                        this.messageService.add({ severity: 'success', summary: 'تم الاعتماد', detail: 'تم اعتماد المخالفة بنجاح' });
                        this.loadData();
                    } else {
                        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
                    }
                });
            }
        });
    }

    edit(violation: EmployeeViolation) {
        this.selectedViolationId = violation.violationId;
        this.isEditMode = true;

        // Determine category based on actionId
        this.actionCategory = violation.actionId ? 'disciplinary' : 'amnesty';

        this.form.patchValue({
            employeeId: violation.employeeId,
            violationTypeId: violation.violationTypeId,
            actionId: violation.actionId,
            violationDate: new Date(violation.violationDate),
            description: violation.description
        });
        this.showDialog = true;
    }

    delete(violation: EmployeeViolation) {
        this.confirmationService.confirm({
            message: 'هل أنت متأكد من حذف هذه المخالفة؟',
            header: 'تأكيد الحذف',
            icon: 'pi pi-trash',
            accept: () => {
                this.performanceService.deleteViolation(violation.violationId).subscribe(res => {
                    if (res.succeeded) {
                        this.messageService.add({ severity: 'success', summary: 'تم الحذف', detail: 'تم حذف المخالفة بنجاح' });
                        this.loadData();
                    } else {
                        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
                    }
                });
            }
        });
    }
}