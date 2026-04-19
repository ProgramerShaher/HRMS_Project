import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TagModule } from 'primeng/tag';
import { DatePickerModule } from 'primeng/datepicker';
import { SelectModule } from 'primeng/select';
import { MessageService, ConfirmationService } from 'primeng/api';
import { PerformanceService } from '../../services/performance.service';
import { AppraisalCycle, CreateCycleCommand } from '../../models/performance.model';
import { RouterModule, Router } from '@angular/router';

@Component({
    selector: 'app-evaluation-cycles',
    standalone: true,
    imports: [
        CommonModule, FormsModule, ReactiveFormsModule, RouterModule,
        TableModule, ButtonModule, DialogModule, InputTextModule,
        ToastModule, ConfirmDialogModule, TagModule, DatePickerModule, SelectModule
    ],
    providers: [MessageService, ConfirmationService],
    template: `
    <p-toast position="top-center" />
    <p-confirmDialog />

    <div class="cycles-page">
        <!-- Header -->
        <div class="page-header">
            <div class="header-content">
                <div class="header-icon">
                    <i class="pi pi-calendar-plus"></i>
                </div>
                <div>
                    <h1>دورات التقييم</h1>
                    <p>إنشاء وإدارة فترات تقييم الأداء السنوية والدورية</p>
                </div>
            </div>
            <button pButton label="دورة تقييم جديدة" icon="pi pi-plus" class="btn-primary" (click)="openAdd()"></button>
        </div>

        <!-- Stats Cards -->
        <div class="stats-row">
            <div class="stat-card">
                <i class="pi pi-list stat-icon blue"></i>
                <div>
                    <div class="stat-num">{{ cycles().length }}</div>
                    <div class="stat-label">إجمالي الدورات</div>
                </div>
            </div>
            <div class="stat-card">
                <i class="pi pi-check-circle stat-icon green"></i>
                <div>
                    <div class="stat-num">{{ activeCycles() }}</div>
                    <div class="stat-label">دورات نشطة</div>
                </div>
            </div>
            <div class="stat-card">
                <i class="pi pi-clock stat-icon orange"></i>
                <div>
                    <div class="stat-num">{{ upcomingCycles() }}</div>
                    <div class="stat-label">قادمة</div>
                </div>
            </div>
        </div>

        <!-- Table -->
        <div class="table-card">
            <p-table [value]="cycles()" [loading]="loading()" dataKey="cycleId"
                     [paginator]="true" [rows]="8" styleClass="p-datatable-gridlines"
                     [globalFilterFields]="['cycleNameAr']">

                <ng-template pTemplate="header">
                    <tr>
                        <th>#</th>
                        <th>اسم الدورة</th>
                        <th>تاريخ البدء</th>
                        <th>تاريخ الانتهاء</th>
                        <th>المدة</th>
                        <th>الحالة</th>
                        <th>الإجراءات</th>
                    </tr>
                </ng-template>

                <ng-template pTemplate="body" let-cycle let-i="rowIndex">
                    <tr [class.active-row]="cycle.isActive === 1">
                        <td>{{ i + 1 }}</td>
                        <td>
                            <div class="cycle-name">
                                <i class="pi pi-calendar cycle-icon"></i>
                                <div>
                                    <strong>{{ cycle.cycleNameAr }}</strong>
                                </div>
                            </div>
                        </td>
                        <td>{{ cycle.startDate | date:'dd/MM/yyyy' }}</td>
                        <td>{{ cycle.endDate | date:'dd/MM/yyyy' }}</td>
                        <td>
                            <span class="duration-badge">{{ getDuration(cycle) }} يوم</span>
                        </td>
                        <td>
                            <p-tag [value]="getStatusLabel(cycle)"
                                   [severity]="getStatusSeverity(cycle)"></p-tag>
                        </td>
                        <td>
                            <div class="action-buttons">
                                <button pButton icon="pi pi-users" class="p-button-text p-button-sm p-button-info"
                                        pTooltip="تهيئة تقييمات الدورة"
                                        (click)="goToAppraisals(cycle)"></button>
                                <button pButton icon="pi pi-trash" class="p-button-text p-button-sm p-button-danger"
                                        pTooltip="حذف الدورة"
                                        (click)="delete(cycle)"></button>
                            </div>
                        </td>
                    </tr>
                </ng-template>

                <ng-template pTemplate="emptymessage">
                    <tr>
                        <td colspan="7" class="empty-state">
                            <i class="pi pi-calendar"></i>
                            <p>لا توجد دورات تقييم. ابدأ بإنشاء الدورة الأولى.</p>
                        </td>
                    </tr>
                </ng-template>
            </p-table>
        </div>
    </div>

    <!-- Create Dialog -->
    <p-dialog [(visible)]="showDialog" header="إنشاء دورة تقييم جديدة"
              [modal]="true" [style]="{width: '520px'}" [draggable]="false">

        <form [formGroup]="form" class="dialog-form">
            <div class="form-grid">
                <div class="form-field full">
                    <label>اسم الدورة <span class="required">*</span></label>
                    <input pInputText formControlName="cycleName"
                           placeholder="مثال: تقييم الأداء السنوي 2025" />
                    <small class="field-error" *ngIf="form.get('cycleName')?.invalid && form.get('cycleName')?.touched">
                        اسم الدورة مطلوب
                    </small>
                </div>

                <div class="form-field">
                    <label>تاريخ البدء <span class="required">*</span></label>
                    <p-datepicker formControlName="startDate" dateFormat="dd/mm/yy"
                                  [showIcon]="true" appendTo="body"></p-datepicker>
                </div>

                <div class="form-field">
                    <label>تاريخ الانتهاء <span class="required">*</span></label>
                    <p-datepicker formControlName="endDate" dateFormat="dd/mm/yy"
                                  [showIcon]="true" appendTo="body"
                                  [minDate]="form.get('startDate')?.value"></p-datepicker>
                    <small class="field-error" *ngIf="isDateRangeInvalid()">
                        تاريخ الانتهاء يجب أن يكون بعد تاريخ البدء
                    </small>
                </div>

                <div class="form-field full">
                    <label>حالة الدورة</label>
                    <p-select formControlName="status" [options]="statusOptions"
                               optionLabel="label" optionValue="value"></p-select>
                </div>
            </div>

            <!-- Preview -->
            <div class="preview-box" *ngIf="form.get('startDate')?.value && form.get('endDate')?.value">
                <i class="pi pi-info-circle"></i>
                مدة الدورة: <strong>{{ getFormDuration() }} يوم</strong>
            </div>
        </form>

        <ng-template pTemplate="footer">
            <button pButton label="إلغاء" icon="pi pi-times" class="p-button-text" (click)="showDialog = false"></button>
            <button pButton label="إنشاء الدورة" icon="pi pi-check"
                    (click)="save()" [disabled]="form.invalid || isDateRangeInvalid()" class="btn-primary"></button>
        </ng-template>
    </p-dialog>
    `,
    styles: [`
        :host { display: block; direction: rtl; }
        .cycles-page { padding: 1.5rem; }

        .page-header {
            display: flex; align-items: center; justify-content: space-between;
            margin-bottom: 1.5rem;
        }
        .header-content { display: flex; align-items: center; gap: 1rem; }
        .header-icon {
            width: 52px; height: 52px; border-radius: 14px;
            background: linear-gradient(135deg, #0ea5e9, #6366f1);
            display: grid; place-items: center; font-size: 1.4rem; color: white;
        }
        h1 { margin: 0; font-size: 1.4rem; color: #1e293b; font-weight: 700; }
        p { margin: 0; color: #64748b; font-size: 0.875rem; }

        .stats-row { display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; margin-bottom: 1.5rem; }
        .stat-card {
            background: #fff; border-radius: 14px; padding: 1.25rem;
            display: flex; align-items: center; gap: 1rem;
            box-shadow: 0 1px 6px rgba(0,0,0,0.06);
        }
        .stat-icon { font-size: 1.8rem; }
        .stat-icon.blue { color: #6366f1; }
        .stat-icon.green { color: #10b981; }
        .stat-icon.orange { color: #f59e0b; }
        .stat-num { font-size: 1.8rem; font-weight: 700; color: #1e293b; line-height: 1; }
        .stat-label { font-size: 0.8rem; color: #64748b; margin-top: 2px; }

        .table-card {
            background: #fff; border-radius: 16px;
            box-shadow: 0 1px 8px rgba(0,0,0,0.06); overflow: hidden;
        }

        .cycle-name { display: flex; align-items: center; gap: 0.75rem; }
        .cycle-icon { color: #6366f1; font-size: 1rem; }
        .duration-badge {
            background: #ede9fe; color: #6d28d9; border-radius: 20px;
            padding: 2px 10px; font-size: 0.82rem; font-weight: 600;
        }
        .active-row { background: rgba(99,102,241,0.03); }
        .action-buttons { display: flex; gap: 0.25rem; }
        .empty-state { text-align: center; padding: 3rem; color: #94a3b8; }
        .empty-state i { font-size: 2.5rem; display: block; margin-bottom: 1rem; }

        .dialog-form { padding: 0.5rem 0; }
        .form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
        .form-field { display: flex; flex-direction: column; gap: 0.4rem; }
        .form-field.full { grid-column: 1 / -1; }
        label { font-size: 0.85rem; font-weight: 600; color: #374151; }
        .required { color: #ef4444; }
        .field-error { color: #ef4444; font-size: 0.78rem; }

        .preview-box {
            margin-top: 1rem; padding: 0.75rem 1rem;
            background: #f0f9ff; border-radius: 8px; color: #0369a1;
            display: flex; align-items: center; gap: 0.5rem; font-size: 0.875rem;
        }
        .btn-primary { background: linear-gradient(135deg, #6366f1, #8b5cf6) !important; border: none !important; }
    `]
})
export class EvaluationCyclesComponent implements OnInit {
    private svc = inject(PerformanceService);
    private msg = inject(MessageService);
    private confirm = inject(ConfirmationService);
    private fb = inject(FormBuilder);
    private router = inject(Router);

    cycles = signal<AppraisalCycle[]>([]);
    loading = signal(false);
    showDialog = false;
    form!: FormGroup;

    statusOptions = [
        { label: 'تخطيط', value: 'PLANNING' },
        { label: 'نشطة', value: 'ACTIVE' },
        { label: 'مكتملة', value: 'COMPLETED' }
    ];

    activeCycles = () => this.cycles().filter(c => c.isActive === 1).length;
    upcomingCycles = () => this.cycles().filter(c => new Date(c.startDate) > new Date()).length;

    ngOnInit() { this.load(); }

    load() {
        this.loading.set(true);
        this.svc.getCycles().subscribe(res => {
            this.loading.set(false);
            if (res.succeeded) this.cycles.set(res.data);
        });
    }

    openAdd() {
        this.form = this.fb.group({
            cycleName: ['', [Validators.required, Validators.maxLength(100)]],
            startDate: [null, Validators.required],
            endDate: [null, Validators.required],
            status: ['ACTIVE']
        });
        this.showDialog = true;
    }

    isDateRangeInvalid(): boolean {
        const s = this.form?.get('startDate')?.value;
        const e = this.form?.get('endDate')?.value;
        return s && e && new Date(e) <= new Date(s);
    }

    getDuration(cycle: AppraisalCycle): number {
        const diff = new Date(cycle.endDate).getTime() - new Date(cycle.startDate).getTime();
        return Math.ceil(diff / (1000 * 60 * 60 * 24));
    }

    getFormDuration(): number {
        const s = this.form?.get('startDate')?.value;
        const e = this.form?.get('endDate')?.value;
        if (!s || !e) return 0;
        return Math.ceil((new Date(e).getTime() - new Date(s).getTime()) / 86400000);
    }

    getStatusLabel(cycle: AppraisalCycle): string {
        const now = new Date();
        const start = new Date(cycle.startDate);
        const end = new Date(cycle.endDate);
        if (now < start) return 'قادمة';
        if (now > end) return 'منتهية';
        return 'جارية';
    }

    getStatusSeverity(cycle: AppraisalCycle): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
        const lbl = this.getStatusLabel(cycle);
        if (lbl === 'جارية') return 'success';
        if (lbl === 'قادمة') return 'info';
        return 'secondary';
    }

    save() {
        if (this.form.invalid) return;
        const v = this.form.value;
        const cmd: CreateCycleCommand = {
            cycleName: v.cycleName,
            startDate: v.startDate instanceof Date ? v.startDate.toISOString() : v.startDate,
            endDate: v.endDate instanceof Date ? v.endDate.toISOString() : v.endDate,
            status: v.status
        };
        this.svc.createCycle(cmd).subscribe(res => {
            if (res.succeeded) {
                this.msg.add({ severity: 'success', summary: 'نجاح', detail: 'تم إنشاء الدورة بنجاح' });
                this.showDialog = false;
                this.load();
            } else {
                this.msg.add({ severity: 'error', summary: 'خطأ', detail: res.message });
            }
        });
    }

    goToAppraisals(cycle: AppraisalCycle) {
        this.router.navigate(['/performance/appraisals'], { queryParams: { cycleId: cycle.cycleId } });
    }

    delete(cycle: AppraisalCycle) {
        this.confirm.confirm({
            message: `هل تريد حذف دورة "${cycle.cycleNameAr}"؟`,
            header: 'تأكيد الحذف',
            icon: 'pi pi-exclamation-triangle',
            acceptButtonStyleClass: 'p-button-danger',
            accept: () => this.svc.deleteCycle(cycle.cycleId).subscribe(res => {
                if (res.succeeded) {
                    this.msg.add({ severity: 'success', summary: 'تم الحذف', detail: res.message });
                    this.load();
                }
            })
        });
    }
}
