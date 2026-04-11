import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SelectModule } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { MessageService, ConfirmationService } from 'primeng/api';
import { PerformanceService } from '../../services/performance.service';
import { KpiLibrary, CreateKpiCommand } from '../../models/performance.model';

@Component({
    selector: 'app-kpi-settings',
    standalone: true,
    imports: [
        CommonModule, FormsModule, ReactiveFormsModule,
        TableModule, ButtonModule, DialogModule, InputTextModule,
        InputNumberModule, TextareaModule, ToastModule, ConfirmDialogModule,
        SelectModule, TagModule
    ],
    providers: [MessageService, ConfirmationService],
    template: `
    <p-toast position="top-center" />
    <p-confirmDialog />

    <div class="kpi-settings-page">
        <!-- Header -->
        <div class="page-header">
            <div class="header-content">
                <div class="header-icon">
                    <i class="pi pi-sliders-h"></i>
                </div>
                <div>
                    <h1>إعدادات مؤشرات الأداء (KPIs)</h1>
                    <p>إدارة المؤشرات والأوزان النسبية لكل فئة وظيفية</p>
                </div>
            </div>
            <button pButton label="إضافة مؤشر جديد" icon="pi pi-plus" class="btn-primary" (click)="openAdd()"></button>
        </div>

        <!-- Weight Summary Banner -->
        <div class="weight-banner" [class.valid]="totalWeight() === 100" [class.warning]="totalWeight() !== 100">
            <div class="weight-info">
                <i class="pi" [class.pi-check-circle]="totalWeight() === 100" [class.pi-exclamation-triangle]="totalWeight() !== 100"></i>
                <span>إجمالي الأوزان: <strong>{{ totalWeight() }}%</strong></span>
                <span *ngIf="totalWeight() !== 100" class="warning-text">
                    — يجب أن يكون المجموع 100% (الفرق: {{ 100 - totalWeight() }}%)
                </span>
                <span *ngIf="totalWeight() === 100" class="success-text">— الأوزان صحيحة ✓</span>
            </div>
            <div class="weight-bar">
                <div class="weight-fill" [style.width.%]="Math.min(totalWeight(), 100)"
                     [class.over]="totalWeight() > 100"></div>
            </div>
        </div>

        <!-- KPI Table -->
        <div class="table-card">
            <p-table [value]="kpis()" [loading]="loading()" dataKey="kpiId"
                     [paginator]="true" [rows]="10" styleClass="p-datatable-gridlines">

                <ng-template pTemplate="header">
                    <tr>
                        <th style="width:40px">#</th>
                        <th>اسم المؤشر</th>
                        <th>التصنيف</th>
                        <th>وحدة القياس</th>
                        <th style="width:120px">الوزن (%)</th>
                        <th style="width:100px">الفئة المستهدفة</th>
                        <th style="width:120px">إجراءات</th>
                    </tr>
                </ng-template>

                <ng-template pTemplate="body" let-kpi let-i="rowIndex">
                    <tr>
                        <td>{{ i + 1 }}</td>
                        <td>
                            <div class="kpi-name">
                                <span class="kpi-dot" [style.background]="getCategoryColor(kpi.category)"></span>
                                {{ kpi.kpiNameAr }}
                            </div>
                            <small class="kpi-desc" *ngIf="kpi.kpiDescription">{{ kpi.kpiDescription }}</small>
                        </td>
                        <td>
                            <p-tag [value]="kpi.category || 'عام'" [severity]="getCategorySeverity(kpi.category)"></p-tag>
                        </td>
                        <td>{{ kpi.measurementUnit || '—' }}</td>
                        <td>
                            <div class="weight-cell">
                                <div class="weight-mini-bar">
                                    <div class="weight-mini-fill" [style.width.%]="kpi.weight"></div>
                                </div>
                                <strong>{{ kpi.weight }}%</strong>
                            </div>
                        </td>
                        <td>
                            <p-tag [value]="kpi.targetJobType || 'ALL'" severity="secondary"></p-tag>
                        </td>
                        <td>
                            <div class="action-buttons">
                                <button pButton icon="pi pi-pencil" class="p-button-text p-button-sm p-button-info"
                                        (click)="openEdit(kpi)" pTooltip="تعديل"></button>
                                <button pButton icon="pi pi-trash" class="p-button-text p-button-sm p-button-danger"
                                        (click)="delete(kpi)" pTooltip="حذف"></button>
                            </div>
                        </td>
                    </tr>
                </ng-template>

                <ng-template pTemplate="emptymessage">
                    <tr>
                        <td colspan="7" class="empty-state">
                            <i class="pi pi-inbox"></i>
                            <p>لا توجد مؤشرات أداء مضافة بعد</p>
                        </td>
                    </tr>
                </ng-template>
            </p-table>
        </div>
    </div>

    <!-- Add / Edit Dialog -->
    <p-dialog [(visible)]="showDialog" [header]="editMode ? 'تعديل مؤشر الأداء' : 'إضافة مؤشر أداء جديد'"
              [modal]="true" [style]="{width: '560px'}" [draggable]="false">

        <form [formGroup]="form" class="dialog-form">
            <div class="form-grid">
                <div class="form-field full">
                    <label>اسم المؤشر (عربي) <span class="required">*</span></label>
                    <input pInputText formControlName="kpiNameAr" placeholder="مثال: معدل الحضور الشهري" />
                    <small class="field-error" *ngIf="form.get('kpiNameAr')?.invalid && form.get('kpiNameAr')?.touched">
                        اسم المؤشر مطلوب
                    </small>
                </div>

                <div class="form-field full">
                    <label>الوصف</label>
                    <textarea pTextarea formControlName="kpiDescription" rows="2"
                              placeholder="وصف مختصر لكيفية قياس هذا المؤشر"></textarea>
                </div>

                <div class="form-field">
                    <label>التصنيف <span class="required">*</span></label>
                    <p-select formControlName="category" [options]="categories"
                               optionLabel="label" optionValue="value" placeholder="اختر التصنيف"></p-select>
                </div>

                <div class="form-field">
                    <label>وحدة القياس</label>
                    <p-select formControlName="measurementUnit" [options]="measurementUnits"
                               optionLabel="label" optionValue="value" placeholder="اختر الوحدة"></p-select>
                </div>

                <div class="form-field">
                    <label>الفئة الوظيفية المستهدفة</label>
                    <p-select formControlName="targetJobType" [options]="jobTypes"
                               optionLabel="label" optionValue="value"></p-select>
                </div>

                <div class="form-field">
                    <label>الوزن النسبي (%) <span class="required">*</span></label>
                    <p-inputnumber formControlName="weight" [min]="1" [max]="100"
                                   suffix="%" [showButtons]="true" buttonLayout="horizontal"
                                   decrementButtonClass="p-button-secondary"
                                   incrementButtonClass="p-button-secondary"></p-inputnumber>
                    <small class="field-hint">مجموع أوزان كل المؤشرات = 100%</small>
                    <small class="field-error" *ngIf="getProjectedTotal() > 100">
                        ⚠ سيتجاوز المجموع 100% (الحالي: {{ getProjectedTotal() }}%)
                    </small>
                </div>
            </div>
        </form>

        <ng-template pTemplate="footer">
            <button pButton label="إلغاء" icon="pi pi-times" class="p-button-text" (click)="showDialog = false"></button>
            <button pButton [label]="editMode ? 'حفظ التعديلات' : 'إضافة المؤشر'" icon="pi pi-check"
                    (click)="save()" [disabled]="form.invalid || getProjectedTotal() > 100"></button>
        </ng-template>
    </p-dialog>
    `,
    styles: [`
        :host { display: block; direction: rtl; }

        .kpi-settings-page { padding: 1.5rem; }

        .page-header {
            display: flex; align-items: center; justify-content: space-between;
            margin-bottom: 1.5rem;
        }
        .header-content { display: flex; align-items: center; gap: 1rem; }
        .header-icon {
            width: 52px; height: 52px; border-radius: 14px;
            background: linear-gradient(135deg, #6366f1, #8b5cf6);
            display: grid; place-items: center;
            font-size: 1.4rem; color: white;
        }
        h1 { margin: 0; font-size: 1.4rem; color: #1e293b; font-weight: 700; }
        p { margin: 0; color: #64748b; font-size: 0.875rem; }

        .weight-banner {
            border-radius: 12px; padding: 1rem 1.25rem;
            margin-bottom: 1.5rem; border: 1px solid transparent;
        }
        .weight-banner.valid { background: #f0fdf4; border-color: #86efac; }
        .weight-banner.warning { background: #fff7ed; border-color: #fdba74; }
        .weight-info { display: flex; align-items: center; gap: 0.75rem; font-size: 0.95rem; margin-bottom: 0.5rem; }
        .weight-info i { font-size: 1.1rem; }
        .valid .weight-info i { color: #22c55e; }
        .warning .weight-info i { color: #f97316; }
        .warning-text { color: #ea580c; font-weight: 500; }
        .success-text { color: #16a34a; font-weight: 500; }
        .weight-bar {
            height: 8px; background: #e2e8f0; border-radius: 4px; overflow: hidden;
        }
        .weight-fill {
            height: 100%; background: #22c55e; border-radius: 4px;
            transition: width 0.4s ease;
        }
        .weight-fill.over { background: #ef4444; }

        .table-card { background: #fff; border-radius: 16px; box-shadow: 0 1px 8px rgba(0,0,0,0.06); overflow: hidden; }

        .kpi-name { display: flex; align-items: center; gap: 0.5rem; font-weight: 500; }
        .kpi-dot { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
        .kpi-desc { display: block; color: #94a3b8; font-size: 0.8rem; margin-top: 2px; }

        .weight-cell { display: flex; align-items: center; gap: 0.5rem; }
        .weight-mini-bar {
            flex: 1; height: 6px; background: #e2e8f0; border-radius: 3px; overflow: hidden;
        }
        .weight-mini-fill { height: 100%; background: linear-gradient(90deg, #6366f1, #8b5cf6); border-radius: 3px; }

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
        .field-hint { color: #6b7280; font-size: 0.78rem; }

        input.ng-invalid.ng-touched, p-select.ng-invalid.ng-touched { border-color: #ef4444; }
        .btn-primary { background: linear-gradient(135deg, #6366f1, #8b5cf6) !important; border: none !important; }
    `]
})
export class KpiSettingsComponent implements OnInit {
    private svc = inject(PerformanceService);
    private msg = inject(MessageService);
    private confirm = inject(ConfirmationService);
    private fb = inject(FormBuilder);

    kpis = signal<KpiLibrary[]>([]);
    loading = signal(false);
    showDialog = false;
    editMode = false;
    editingId = 0;

    protected Math = Math;

    form!: FormGroup;

    categories = [
        { label: 'حضور وانضباط', value: 'ATTENDANCE' },
        { label: 'إنتاجية وأداء', value: 'PRODUCTIVITY' },
        { label: 'سلوك ومهارات', value: 'BEHAVIOR' },
        { label: 'عمل إضافي', value: 'OVERTIME' },
        { label: 'تطوير ذاتي', value: 'DEVELOPMENT' },
        { label: 'عام', value: 'GENERAL' }
    ];

    measurementUnits = [
        { label: 'نسبة مئوية (%)', value: 'PERCENTAGE' },
        { label: 'عدد (Count)', value: 'COUNT' },
        { label: 'ساعات (Hours)', value: 'HOURS' },
        { label: 'أيام (Days)', value: 'DAYS' }
    ];

    jobTypes = [
        { label: 'الكل', value: 'ALL' },
        { label: 'ممرض / ممرضة', value: 'NURSE' },
        { label: 'طبيب', value: 'DOCTOR' },
        { label: 'إداري', value: 'ADMIN' },
        { label: 'فني', value: 'TECHNICIAN' }
    ];

    totalWeight = computed(() => this.kpis().reduce((s, k) => s + (k.weight || 0), 0));

    ngOnInit() { this.load(); }

    load() {
        this.loading.set(true);
        this.svc.getKpis().subscribe(res => {
            this.loading.set(false);
            if (res.succeeded) this.kpis.set(res.data);
        });
    }

    openAdd() {
        this.editMode = false;
        this.editingId = 0;
        this.form = this.fb.group({
            kpiNameAr: ['', Validators.required],
            kpiDescription: [''],
            category: ['GENERAL', Validators.required],
            measurementUnit: ['PERCENTAGE'],
            targetJobType: ['ALL'],
            weight: [10, [Validators.required, Validators.min(1), Validators.max(100)]]
        });
        this.showDialog = true;
    }

    openEdit(kpi: KpiLibrary) {
        this.editMode = true;
        this.editingId = kpi.kpiId;
        this.form = this.fb.group({
            kpiNameAr: [kpi.kpiNameAr, Validators.required],
            kpiDescription: [kpi.kpiDescription || ''],
            category: [kpi.category || 'GENERAL', Validators.required],
            measurementUnit: [kpi.measurementUnit || 'PERCENTAGE'],
            targetJobType: [kpi.targetJobType || 'ALL'],
            weight: [kpi.weight, [Validators.required, Validators.min(1), Validators.max(100)]]
        });
        this.showDialog = true;
    }

    getProjectedTotal(): number {
        const others = this.kpis()
            .filter(k => k.kpiId !== this.editingId)
            .reduce((s, k) => s + k.weight, 0);
        return others + (this.form?.get('weight')?.value || 0);
    }

    save() {
        if (this.form.invalid) return;
        const cmd: CreateKpiCommand = this.form.value;

        const req = this.editMode
            ? this.svc.updateKpi(this.editingId, cmd)
            : this.svc.createKpi(cmd);

        req.subscribe(res => {
            if (res.succeeded) {
                this.msg.add({ severity: 'success', summary: 'نجاح', detail: res.message });
                this.showDialog = false;
                this.load();
            } else {
                this.msg.add({ severity: 'error', summary: 'خطأ', detail: res.message });
            }
        });
    }

    delete(kpi: KpiLibrary) {
        this.confirm.confirm({
            message: `هل تريد حذف مؤشر "${kpi.kpiNameAr}"؟`,
            header: 'تأكيد الحذف',
            icon: 'pi pi-trash',
            acceptButtonStyleClass: 'p-button-danger',
            accept: () => this.svc.deleteKpi(kpi.kpiId).subscribe(res => {
                if (res.succeeded) {
                    this.msg.add({ severity: 'success', summary: 'تم الحذف', detail: res.message });
                    this.load();
                }
            })
        });
    }

    getCategoryColor(cat?: string): string {
        const m: Record<string, string> = {
            ATTENDANCE: '#6366f1', PRODUCTIVITY: '#10b981',
            BEHAVIOR: '#f59e0b', OVERTIME: '#3b82f6',
            DEVELOPMENT: '#8b5cf6', GENERAL: '#94a3b8'
        };
        return m[cat || 'GENERAL'] ?? '#94a3b8';
    }

    getCategorySeverity(cat?: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
        const m: Record<string, any> = {
            ATTENDANCE: 'info', PRODUCTIVITY: 'success',
            BEHAVIOR: 'warn', OVERTIME: 'info', DEVELOPMENT: 'success'
        };
        return m[cat || ''] ?? 'secondary';
    }
}
