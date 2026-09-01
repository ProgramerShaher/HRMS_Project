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

    <div class="bg-white dark:bg-slate-900 rounded-xl p-4 border border-slate-200 dark:border-slate-800 shadow-sm flex flex-col h-full gap-4 animate-in fade-in slide-in-from-bottom-5 duration-700" dir="rtl">
        <!-- Header -->
        <div class="flex flex-col md:flex-row justify-between items-center pb-3 border-b border-slate-100 dark:border-slate-800 gap-4 md:gap-0">
            <div class="flex items-center gap-3">
                <div class="w-7 h-7 rounded-md bg-blue-50 dark:bg-blue-900/20 text-blue-600 dark:text-blue-400 flex items-center justify-center shadow-sm border border-blue-100 dark:border-blue-900/30">
                    <i class="pi pi-calendar-plus text-[11px]"></i>
                </div>
                <div>
                    <h3 class="text-xs font-bold text-slate-800 dark:text-slate-100 leading-tight">دورات التقييم</h3>
                    <p class="text-[9px] text-slate-500 dark:text-slate-400 font-medium mt-0.5">إنشاء وإدارة فترات تقييم الأداء السنوية والدورية</p>
                </div>
            </div>
            <button pButton label="دورة تقييم جديدة" icon="pi pi-plus" class="p-button-outlined p-button-primary !h-8 !px-3 !py-0 flex justify-center items-center !rounded-md hover:!bg-blue-50 dark:hover:!bg-blue-900/20 !border-blue-200 dark:!border-blue-800 transition-all text-[10px] font-bold shadow-sm" (click)="openAdd()"></button>
        </div>

        <!-- Stats Cards -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-3">
            <div class="bg-slate-50/50 dark:bg-slate-800/30 rounded-lg p-3 border border-slate-100 dark:border-slate-800 flex items-center gap-3">
                <div class="w-8 h-8 rounded bg-blue-50 dark:bg-blue-900/20 flex items-center justify-center text-blue-500 shadow-sm border border-blue-100 dark:border-blue-900/30">
                    <i class="pi pi-list text-sm"></i>
                </div>
                <div class="flex flex-col">
                    <span class="text-lg font-black text-slate-700 dark:text-slate-200 leading-tight">{{ cycles().length }}</span>
                    <span class="text-[9px] font-bold text-slate-500 dark:text-slate-400">إجمالي الدورات</span>
                </div>
            </div>
            <div class="bg-emerald-50/50 dark:bg-emerald-900/10 rounded-lg p-3 border border-emerald-100 dark:border-emerald-900/30 flex items-center gap-3">
                <div class="w-8 h-8 rounded bg-white dark:bg-slate-800 flex items-center justify-center text-emerald-500 shadow-sm border border-emerald-200 dark:border-emerald-800/50">
                    <i class="pi pi-check-circle text-sm"></i>
                </div>
                <div class="flex flex-col">
                    <span class="text-lg font-black text-emerald-700 dark:text-emerald-400 leading-tight">{{ activeCycles() }}</span>
                    <span class="text-[9px] font-bold text-emerald-600 dark:text-emerald-500">دورات نشطة</span>
                </div>
            </div>
            <div class="bg-amber-50/50 dark:bg-amber-900/10 rounded-lg p-3 border border-amber-100 dark:border-amber-900/30 flex items-center gap-3">
                <div class="w-8 h-8 rounded bg-white dark:bg-slate-800 flex items-center justify-center text-amber-500 shadow-sm border border-amber-200 dark:border-amber-800/50">
                    <i class="pi pi-clock text-sm"></i>
                </div>
                <div class="flex flex-col">
                    <span class="text-lg font-black text-amber-700 dark:text-amber-400 leading-tight">{{ upcomingCycles() }}</span>
                    <span class="text-[9px] font-bold text-amber-600 dark:text-amber-500">قادمة</span>
                </div>
            </div>
        </div>

        <!-- Table -->
        <div class="border border-slate-200 dark:border-slate-700 rounded-xl overflow-hidden flex flex-col">
            <p-table [value]="cycles()" [loading]="loading()" dataKey="cycleId"
                     [paginator]="true" [rows]="8" styleClass="p-datatable-sm clean-table"
                     [globalFilterFields]="['cycleNameAr']">

                <ng-template pTemplate="header">
                    <tr class="bg-slate-50/50 dark:bg-slate-800/30 border-b border-slate-100 dark:border-slate-800">
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none" style="width:40px">#</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none">اسم الدورة</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none">تاريخ البدء</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none">تاريخ الانتهاء</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none">المدة</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none">الحالة</th>
                        <th class="!px-3 !py-1.5 !text-[9px] !font-bold !text-slate-500 !bg-transparent !border-none text-center" style="width:100px">الإجراءات</th>
                    </tr>
                </ng-template>

                <ng-template pTemplate="body" let-cycle let-i="rowIndex">
                    <tr class="hover:bg-slate-50/50 dark:hover:bg-slate-800/50 transition-colors border-b border-slate-50 dark:border-slate-800/50" [ngClass]="{'bg-blue-50/20 dark:bg-blue-900/10': cycle.isActive === 1}">
                        <td class="!px-3 !py-2 !border-none text-[9px] font-mono text-slate-400">{{ i + 1 }}</td>
                        <td class="!px-3 !py-2 !border-none">
                            <div class="flex items-center gap-1.5">
                                <i class="pi pi-calendar text-[10px] text-blue-500"></i>
                                <strong class="font-bold text-[10px] text-slate-700 dark:text-slate-200">{{ cycle.cycleNameAr }}</strong>
                            </div>
                        </td>
                        <td class="!px-3 !py-2 !border-none text-[9px] font-mono text-slate-600 dark:text-slate-400">{{ cycle.startDate | date:'yyyy-MM-dd' }}</td>
                        <td class="!px-3 !py-2 !border-none text-[9px] font-mono text-slate-600 dark:text-slate-400">{{ cycle.endDate | date:'yyyy-MM-dd' }}</td>
                        <td class="!px-3 !py-2 !border-none">
                            <span class="px-2 py-0.5 rounded text-[8px] font-bold bg-purple-50 text-purple-600 border border-purple-200 dark:bg-purple-900/20 dark:text-purple-400 dark:border-purple-800/50">{{ getDuration(cycle) }} يوم</span>
                        </td>
                        <td class="!px-3 !py-2 !border-none">
                            <p-tag [value]="getStatusLabel(cycle)"
                                   [severity]="getStatusSeverity(cycle)" styleClass="!text-[8px] !px-1.5 !py-0.5 !rounded-sm"></p-tag>
                        </td>
                        <td class="!px-3 !py-2 !border-none text-center">
                            <div class="flex justify-center gap-1">
                                <button pButton icon="pi pi-users" class="p-button-text p-button-info !w-6 !h-6 !p-0 !text-[10px]"
                                        title="تهيئة تقييمات الدورة"
                                        (click)="goToAppraisals(cycle)"></button>
                                <button pButton icon="pi pi-trash" class="p-button-text p-button-danger !w-6 !h-6 !p-0 !text-[10px]"
                                        title="حذف الدورة"
                                        (click)="delete(cycle)"></button>
                            </div>
                        </td>
                    </tr>
                </ng-template>

                <ng-template pTemplate="emptymessage">
                    <tr>
                        <td colspan="7" class="text-center py-8 bg-slate-50/50 dark:bg-slate-800/30">
                            <i class="pi pi-calendar text-2xl text-slate-300 dark:text-slate-600 block mb-2"></i>
                            <span class="text-[10px] font-bold text-slate-500">لا توجد دورات تقييم. ابدأ بإنشاء الدورة الأولى.</span>
                        </td>
                    </tr>
                </ng-template>
            </p-table>
        </div>
    </div>

    <!-- Create Dialog -->
    <p-dialog [(visible)]="showDialog" header="إنشاء دورة تقييم جديدة"
              [modal]="true" [style]="{width: '450px'}" [draggable]="false" styleClass="p-fluid">

        <form [formGroup]="form" class="pt-2">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
                <div class="col-span-2">
                    <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300 mb-1">اسم الدورة <span class="text-rose-500">*</span></label>
                    <input pInputText formControlName="cycleName"
                           placeholder="مثال: تقييم الأداء السنوي 2025" class="p-inputtext-sm w-full" />
                    <small class="text-rose-500 text-[8px] mt-1 block" *ngIf="form.get('cycleName')?.invalid && form.get('cycleName')?.touched">
                        اسم الدورة مطلوب
                    </small>
                </div>

                <div class="col-span-1">
                    <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300 mb-1">تاريخ البدء <span class="text-rose-500">*</span></label>
                    <p-datepicker formControlName="startDate" dateFormat="dd/mm/yy"
                                  [showIcon]="true" appendTo="body" styleClass="p-inputtext-sm w-full"></p-datepicker>
                </div>

                <div class="col-span-1">
                    <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300 mb-1">تاريخ الانتهاء <span class="text-rose-500">*</span></label>
                    <p-datepicker formControlName="endDate" dateFormat="dd/mm/yy"
                                  [showIcon]="true" appendTo="body"
                                  [minDate]="form.get('startDate')?.value" styleClass="p-inputtext-sm w-full"></p-datepicker>
                    <small class="text-rose-500 text-[8px] mt-1 block font-bold" *ngIf="isDateRangeInvalid()">
                        يجب أن يكون بعد تاريخ البدء
                    </small>
                </div>

                <div class="col-span-2">
                    <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300 mb-1">حالة الدورة</label>
                    <p-select formControlName="status" [options]="statusOptions"
                               optionLabel="label" optionValue="value" styleClass="p-inputtext-sm w-full"></p-select>
                </div>
            </div>

            <!-- Preview -->
            <div class="mt-3 p-2 bg-blue-50/50 dark:bg-blue-900/10 border border-blue-100 dark:border-blue-900/30 rounded-lg text-blue-700 dark:text-blue-300 flex items-center gap-1.5 text-[10px]" *ngIf="form.get('startDate')?.value && form.get('endDate')?.value">
                <i class="pi pi-info-circle"></i>
                مدة الدورة: <strong class="font-mono text-blue-800 dark:text-blue-200">{{ getFormDuration() }} يوم</strong>
            </div>
        </form>

        <ng-template pTemplate="footer">
            <div class="border-t border-slate-100 dark:border-slate-800 pt-3 flex justify-end gap-2 mt-2">
                <button pButton label="إلغاء" class="p-button-text p-button-sm !text-[10px] !font-bold" (click)="showDialog = false"></button>
                <button pButton label="إنشاء الدورة" icon="pi pi-check"
                        (click)="save()" [disabled]="form.invalid || isDateRangeInvalid()" class="p-button-primary p-button-sm !text-[10px] !font-bold shadow-sm"></button>
            </div>
        </ng-template>
    </p-dialog>
    `,
    styles: [`
        :host { display: block; }
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
