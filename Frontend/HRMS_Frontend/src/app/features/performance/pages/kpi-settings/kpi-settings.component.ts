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

    <div class="bg-white dark:bg-slate-900 rounded-xl p-4 border border-slate-200 dark:border-slate-800 shadow-sm flex flex-col h-full gap-4 animate-in fade-in slide-in-from-bottom-5 duration-700" dir="rtl">
        <!-- Header -->
        <div class="flex flex-col md:flex-row justify-between items-center pb-3 border-b border-slate-100 dark:border-slate-800 gap-4 md:gap-0">
            <div class="flex items-center gap-3">
                <div class="w-7 h-7 rounded-md bg-purple-50 dark:bg-purple-900/20 text-purple-600 dark:text-purple-400 flex items-center justify-center shadow-sm border border-purple-100 dark:border-purple-900/30">
                    <i class="pi pi-sliders-h text-[11px]"></i>
                </div>
                <div>
                    <h3 class="text-xs font-bold text-slate-800 dark:text-slate-100 leading-tight">إعدادات مؤشرات الأداء (KPIs)</h3>
                    <p class="text-[9px] text-slate-500 dark:text-slate-400 font-medium mt-0.5">إدارة المؤشرات والأوزان النسبية لكل فئة وظيفية</p>
                </div>
            </div>
            <button pButton label="إضافة مؤشر جديد" icon="pi pi-plus" class="p-button-outlined p-button-primary !h-8 !px-3 !py-0 flex justify-center items-center !rounded-md hover:!bg-blue-50 dark:hover:!bg-blue-900/20 !border-blue-200 dark:!border-blue-800 transition-all text-[10px] font-bold shadow-sm" (click)="openAdd()"></button>
        </div>

        <!-- Weight Summary Banner -->
        <div class="rounded-lg p-3 border border-slate-200 dark:border-slate-700 flex flex-col gap-2"
             [ngClass]="{'bg-emerald-50/50 dark:bg-emerald-900/10 border-emerald-200 dark:border-emerald-800/50': totalWeight() === 100, 'bg-amber-50/50 dark:bg-amber-900/10 border-amber-200 dark:border-amber-800/50': totalWeight() !== 100}">
            <div class="flex items-center gap-2 text-[10px] font-bold">
                <i class="pi text-[11px]" [ngClass]="{'pi-check-circle text-emerald-500': totalWeight() === 100, 'pi-exclamation-triangle text-amber-500': totalWeight() !== 100}"></i>
                <span class="text-slate-700 dark:text-slate-200">إجمالي الأوزان: <strong class="font-mono" [ngClass]="{'text-emerald-600': totalWeight() === 100, 'text-amber-600': totalWeight() !== 100}">{{ totalWeight() }}%</strong></span>
                <span *ngIf="totalWeight() !== 100" class="text-amber-600 dark:text-amber-500 mr-2">
                    — يجب أن يكون المجموع 100% <span class="font-mono">(الفرق: {{ Math.abs(100 - totalWeight()) }}%)</span>
                </span>
                <span *ngIf="totalWeight() === 100" class="text-emerald-600 dark:text-emerald-500 mr-2">— الأوزان صحيحة ✓</span>
            </div>
            <div class="h-1.5 bg-slate-200 dark:bg-slate-700 rounded-full overflow-hidden w-full">
                <div class="h-full rounded-full transition-all duration-500" 
                     [style.width.%]="Math.min(totalWeight(), 100)"
                     [ngClass]="totalWeight() > 100 ? 'bg-rose-500' : (totalWeight() === 100 ? 'bg-emerald-500' : 'bg-amber-500')"></div>
            </div>
        </div>

        <!-- KPI Table -->
        <div class="border border-slate-200 dark:border-slate-700 rounded-xl overflow-hidden flex flex-col">
            <p-table [value]="kpis()" [loading]="loading()" dataKey="kpiId"
                     [paginator]="true" [rows]="10" styleClass="p-datatable-sm clean-table">

                <ng-template pTemplate="header">
                    <tr class="bg-slate-50/50 dark:bg-slate-800/30 border-b border-slate-100 dark:border-slate-800">
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none" style="width:40px">#</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none">اسم المؤشر</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none">التصنيف</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none">وحدة القياس</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none" style="width:140px">الوزن (%)</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none">الفئة المستهدفة</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none text-center" style="width:100px">إجراءات</th>
                    </tr>
                </ng-template>

                <ng-template pTemplate="body" let-kpi let-i="rowIndex">
                    <tr class="hover:bg-slate-50/50 dark:hover:bg-slate-800/50 transition-colors border-b border-slate-50 dark:border-slate-800/50">
                        <td class="!px-3 !py-2 !border-none text-[9px] font-mono text-slate-400">{{ i + 1 }}</td>
                        <td class="!px-3 !py-2 !border-none">
                            <div class="flex items-center gap-1.5">
                                <span class="w-1.5 h-1.5 rounded-full flex-shrink-0" [style.background]="getCategoryColor(kpi.category)"></span>
                                <span class="font-bold text-[10px] text-slate-700 dark:text-slate-200">{{ kpi.kpiNameAr }}</span>
                            </div>
                            <small class="block text-[8px] text-slate-400 mt-0.5 pr-3" *ngIf="kpi.kpiDescription">{{ kpi.kpiDescription }}</small>
                        </td>
                        <td class="!px-3 !py-2 !border-none">
                            <p-tag [value]="kpi.category || 'عام'" [severity]="getCategorySeverity(kpi.category)" styleClass="!text-[8px] !px-1.5 !py-0.5 !rounded-sm"></p-tag>
                        </td>
                        <td class="!px-3 !py-2 !border-none text-[9px] font-medium text-slate-600 dark:text-slate-400">{{ kpi.measurementUnit || '—' }}</td>
                        <td class="!px-3 !py-2 !border-none">
                            <div class="flex items-center gap-2">
                                <div class="flex-1 h-1.5 bg-slate-100 dark:bg-slate-700 rounded-full overflow-hidden w-16">
                                    <div class="h-full rounded-full" [style.width.%]="kpi.weight" [style.background]="getCategoryColor(kpi.category)"></div>
                                </div>
                                <strong class="text-[9px] font-mono text-slate-700 dark:text-slate-300">{{ kpi.weight }}%</strong>
                            </div>
                        </td>
                        <td class="!px-3 !py-2 !border-none">
                            <p-tag [value]="kpi.targetJobType || 'ALL'" severity="secondary" styleClass="!text-[8px] !px-1.5 !py-0.5 !rounded-sm !bg-slate-100 !text-slate-600 dark:!bg-slate-800 dark:!text-slate-400"></p-tag>
                        </td>
                        <td class="!px-3 !py-2 !border-none text-center">
                            <div class="flex justify-center gap-1">
                                <button pButton icon="pi pi-pencil" class="p-button-text p-button-info !w-6 !h-6 !p-0 !text-[10px]" (click)="openEdit(kpi)" title="تعديل"></button>
                                <button pButton icon="pi pi-trash" class="p-button-text p-button-danger !w-6 !h-6 !p-0 !text-[10px]" (click)="delete(kpi)" title="حذف"></button>
                            </div>
                        </td>
                    </tr>
                </ng-template>

                <ng-template pTemplate="emptymessage">
                    <tr>
                        <td colspan="7" class="text-center py-8 bg-slate-50/50 dark:bg-slate-800/30">
                            <i class="pi pi-inbox text-2xl text-slate-300 dark:text-slate-600 block mb-2"></i>
                            <span class="text-[10px] font-bold text-slate-500">لا توجد مؤشرات أداء مضافة بعد</span>
                        </td>
                    </tr>
                </ng-template>
            </p-table>
        </div>
    </div>

    <!-- Add / Edit Dialog -->
    <p-dialog [(visible)]="showDialog" [header]="editMode ? 'تعديل مؤشر الأداء' : 'إضافة مؤشر أداء جديد'"
              [modal]="true" [style]="{width: '500px'}" [draggable]="false" styleClass="p-fluid">

        <form [formGroup]="form" class="pt-2">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
                <div class="col-span-2">
                    <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300 mb-1">اسم المؤشر (عربي) <span class="text-rose-500">*</span></label>
                    <input pInputText formControlName="kpiNameAr" placeholder="مثال: معدل الحضور الشهري" class="p-inputtext-sm w-full" />
                    <small class="text-rose-500 text-[8px] mt-1 block" *ngIf="form.get('kpiNameAr')?.invalid && form.get('kpiNameAr')?.touched">اسم المؤشر مطلوب</small>
                </div>

                <div class="col-span-2">
                    <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300 mb-1">الوصف</label>
                    <textarea pTextarea formControlName="kpiDescription" rows="2" class="p-inputtext-sm w-full" placeholder="وصف مختصر لكيفية قياس هذا المؤشر"></textarea>
                </div>

                <div class="col-span-1">
                    <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300 mb-1">التصنيف <span class="text-rose-500">*</span></label>
                    <p-select formControlName="category" [options]="categories" optionLabel="label" optionValue="value" placeholder="اختر التصنيف" styleClass="p-inputtext-sm w-full"></p-select>
                </div>

                <div class="col-span-1">
                    <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300 mb-1">وحدة القياس</label>
                    <p-select formControlName="measurementUnit" [options]="measurementUnits" optionLabel="label" optionValue="value" placeholder="اختر الوحدة" styleClass="p-inputtext-sm w-full"></p-select>
                </div>

                <div class="col-span-1">
                    <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300 mb-1">الفئة المستهدفة</label>
                    <p-select formControlName="targetJobType" [options]="jobTypes" optionLabel="label" optionValue="value" styleClass="p-inputtext-sm w-full"></p-select>
                </div>

                <div class="col-span-1">
                    <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300 mb-1">الوزن النسبي (%) <span class="text-rose-500">*</span></label>
                    <p-inputnumber formControlName="weight" [min]="1" [max]="100" suffix="%" [showButtons]="true" buttonLayout="horizontal" styleClass="p-inputtext-sm w-full"
                        incrementButtonIcon="pi pi-plus text-[10px]" decrementButtonIcon="pi pi-minus text-[10px]"></p-inputnumber>
                    <small class="text-slate-500 text-[8px] mt-1 block">مجموع أوزان المؤشرات = 100%</small>
                    <small class="text-rose-500 text-[8px] mt-0.5 block font-bold" *ngIf="getProjectedTotal() > 100">
                        ⚠ سيتجاوز المجموع 100% (الحالي: {{ getProjectedTotal() }}%)
                    </small>
                </div>
            </div>
        </form>

        <ng-template pTemplate="footer">
            <div class="border-t border-slate-100 dark:border-slate-800 pt-3 flex justify-end gap-2 mt-2">
                <button pButton label="إلغاء" class="p-button-text p-button-sm !text-[10px] !font-bold" (click)="showDialog = false"></button>
                <button pButton [label]="editMode ? 'حفظ التعديلات' : 'إضافة المؤشر'" icon="pi pi-check"
                        class="p-button-primary p-button-sm !text-[10px] !font-bold shadow-sm"
                        (click)="save()" [disabled]="form.invalid || getProjectedTotal() > 100"></button>
            </div>
        </ng-template>
    </p-dialog>
    `,
    styles: [`
        :host { display: block; }
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
