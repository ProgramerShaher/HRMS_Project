import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FloatLabelModule } from 'primeng/floatlabel';
import { CheckboxModule } from 'primeng/checkbox';
import { SelectModule } from 'primeng/select';
import { SetupService } from '../../services/setup.service';
import { MessageService } from 'primeng/api';

@Component({
    selector: 'app-department-form',
    standalone: true,
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        InputTextModule,
        ButtonModule,
        FloatLabelModule,
        CheckboxModule,
        SelectModule
    ],
    changeDetection: ChangeDetectionStrategy.OnPush,
    template: `
    <form [formGroup]="form" (ngSubmit)="save()" class="flex flex-col h-full bg-white dark:bg-slate-900 rounded-xl shadow-2xl border border-slate-200 dark:border-slate-800" dir="rtl">
        
        <!-- Dialog Header -->
        <div class="px-5 py-4 border-b border-slate-100 dark:border-slate-800 flex justify-between items-center bg-slate-50/50 dark:bg-slate-900/50 rounded-t-xl">
            <div class="flex items-center gap-3">
                <div class="w-8 h-8 rounded-lg bg-blue-50 dark:bg-blue-900/20 text-blue-600 dark:text-blue-400 flex items-center justify-center">
                    <i [class]="isEdit ? 'pi pi-pencil' : 'pi pi-plus'" class="text-[13px]"></i>
                </div>
                <h3 class="text-sm font-bold text-slate-900 dark:text-white">
                    {{ isEdit ? 'تعديل بيانات القسم' : 'إضافة قسم جديد' }}
                </h3>
            </div>
            <button type="button"
                class="w-8 h-8 flex items-center justify-center rounded-lg text-slate-400 hover:text-red-500 hover:bg-red-50 dark:hover:bg-red-900/20 transition-all"
                (click)="close()">
                <i class="pi pi-times text-[12px]"></i>
            </button>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-x-5 gap-y-4 p-5 overflow-y-auto">
            
            <!-- Ar Name -->
            <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">اسم القسم (عربي) <span class="text-red-500">*</span></label>
                <input pInputText id="deptNameAr" formControlName="deptNameAr" autofocus 
                    class="w-full !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

            <!-- En Name -->
            <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">اسم القسم (إنجليزي)</label>
                <input pInputText id="deptNameEn" formControlName="deptNameEn" 
                    class="w-full !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

            <!-- Parent Department (Select with Search) -->
            <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">القسم الرئيسي</label>
                <p-select 
                    [options]="departmentsList" 
                    formControlName="parentDeptId" 
                    optionLabel="deptNameAr" 
                    optionValue="deptId"
                    [filter]="true"
                    filterBy="deptNameAr,deptNameEn" 
                    [showClear]="true"
                    placeholder="بدون قسم رئيسي"
                    styleClass="w-full !rounded-md !h-9 !text-[12px] shadow-none !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all [&_.p-select-label]:!py-0 [&_.p-select-label]:!px-3 [&_.p-select-label]:!leading-[34px]"
                    appendTo="body"
                    scrollHeight="250px">
                    <ng-template pTemplate="selectedItem" let-selectedOption>
                        <div class="flex align-items-center gap-2" *ngIf="selectedOption">
                            <div>{{ selectedOption.deptNameAr }}</div>
                        </div>
                    </ng-template>
                    <ng-template pTemplate="item" let-dept>
                        <div class="flex align-items-center gap-2 text-[12px] py-1">
                            <div>{{ dept.deptNameAr }}</div>
                        </div>
                    </ng-template>
                </p-select>
            </div>

             <!-- Manager (Select with Search from API) -->
             <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">مدير القسم</label>
                <p-select 
                    [options]="employeesList" 
                    formControlName="managerId" 
                    optionLabel="fullNameAr" 
                    optionValue="employeeId"
                    [filter]="true"
                    filterBy="fullNameAr,fullNameEn,employeeNumber" 
                    [showClear]="true"
                    placeholder="اختر مدير القسم"
                    styleClass="w-full !rounded-md !h-9 !text-[12px] shadow-none !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all [&_.p-select-label]:!py-0 [&_.p-select-label]:!px-3 [&_.p-select-label]:!leading-[34px]"
                    appendTo="body"
                    scrollHeight="250px">
                    <ng-template pTemplate="selectedItem" let-selectedOption>
                        <div class="flex items-center gap-2" *ngIf="selectedOption">
                            <i class="pi pi-user text-slate-400 text-[10px]"></i>
                            <div>{{ selectedOption.fullNameAr }}</div>
                        </div>
                    </ng-template>
                    <ng-template pTemplate="item" let-emp>
                        <div class="flex flex-col py-1 border-b border-slate-50 dark:border-slate-800 last:border-0">
                            <span class="font-bold text-[11px] text-slate-700 dark:text-slate-200">{{ emp.fullNameAr }}</span>
                            <span class="text-[9px] text-slate-500">{{ emp.jobTitle || 'بدون مسمى' }}</span>
                        </div>
                    </ng-template>
                </p-select>
            </div>

             <!-- Cost Center -->
             <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">مركز التكلفة</label>
                <input pInputText id="costCenterCode" formControlName="costCenterCode" 
                    class="w-full uppercase !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] font-mono tracking-wider focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

            <!-- Is Active -->
            <div class="col-span-1 flex items-end">
                <div class="w-full h-9 flex items-center gap-2 px-3 border border-slate-100 dark:border-slate-800 bg-slate-50 dark:bg-slate-800/30 rounded-md cursor-pointer hover:bg-slate-100 dark:hover:bg-slate-800/50 transition-colors"
                     (click)="form.get('isActive')?.setValue(!form.get('isActive')?.value)">
                    <p-checkbox formControlName="isActive" [binary]="true" inputId="isActive"></p-checkbox>
                    <label for="isActive" class="cursor-pointer text-[11px] font-bold text-slate-700 dark:text-slate-300 flex-grow">حالة التفعيل (نشط)</label>
                </div>
            </div>
        </div>

        <div class="px-5 py-4 border-t border-slate-100 dark:border-slate-800 flex justify-end gap-3 bg-slate-50/50 dark:bg-slate-900/50 mt-auto rounded-b-xl">
            <button pButton label="إلغاء" type="button"
                class="p-button-text !h-9 !text-[12px] !text-slate-600 dark:!text-slate-400 !font-bold hover:!bg-slate-200 dark:hover:!bg-slate-800 !px-5 !rounded-lg transition-colors"
                (click)="close()">
            </button>
            <button pButton [label]="isEdit ? 'حفظ التغييرات' : 'إضافة القسم'" icon="pi pi-check" type="submit" [loading]="loading"
                class="p-button-primary !h-9 !bg-blue-600 hover:!bg-blue-700 !border-none !text-[12px] !px-6 !rounded-lg shadow-sm font-bold transition-colors">
            </button>
        </div>
    </form>
  `
})
export class DepartmentFormComponent implements OnInit {
    form!: FormGroup;
    loading = false;
    isEdit = false;
    id!: number;

    employeesList: any[] = [];
    departmentsList: any[] = [];

    constructor(
        private fb: FormBuilder,
        public ref: DynamicDialogRef,
        public config: DynamicDialogConfig,
        private setupService: SetupService,
        private messageService: MessageService,
        private cdr: ChangeDetectorRef
    ) { }

    ngOnInit() {
        this.isEdit = !!this.config.data?.deptId;
        this.id = this.config.data?.deptId;

        // Initialize Form
        this.form = this.fb.group({
            deptNameAr: [this.config.data?.deptNameAr || '', Validators.required],
            deptNameEn: [this.config.data?.deptNameEn || ''],
            parentDeptId: [this.config.data?.parentDeptId || null],
            managerId: [this.config.data?.managerId || null],
            costCenterCode: [this.config.data?.costCenterCode || ''],
            isActive: [this.config.data?.isActive === undefined ? true : (this.config.data.isActive == 1 || this.config.data.isActive === true)]
        });

        // Load Data for Dropdowns
        this.loadLookups();
    }

    loadLookups() {
        // 1. Load Employees
        this.setupService.getAll<any>('Employees').subscribe({
            next: (res: any) => {
                this.employeesList = res.data?.items || res.items || res.data || res || [];
                // Manually trigger change detection after data loads
                this.cdr.markForCheck();
            },
            error: () => this.cdr.markForCheck()
        });

        // 2. Load Departments
        this.setupService.getAll<any>('Departments').subscribe({
            next: (res: any) => {
                let allDepts = res.data?.items || res.items || res.data || res || [];
                if (Array.isArray(allDepts) && this.isEdit) {
                    allDepts = allDepts.filter((d: any) => d.deptId !== this.id);
                }
                this.departmentsList = Array.isArray(allDepts) ? allDepts : [];
                this.cdr.markForCheck();
            },
            error: () => this.cdr.markForCheck()
        });
    }

    save() {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.loading = true;
        this.cdr.markForCheck();

        const formVal = this.form.value;
        const payload = {
            ...formVal,
            parentDeptId: formVal.parentDeptId || null,
            managerId: formVal.managerId || null,
            isActive: formVal.isActive ? 1 : 0
        };

        const request = this.isEdit
            ? this.setupService.update('Departments', this.id, { deptId: this.id, ...payload })
            : this.setupService.create('Departments', payload);

        request.subscribe({
            next: (res: any) => {
                this.loading = false;
                this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الحفظ بنجاح' });
                // Small delay to ensure message is shown before closing if needed, 
                // but usually ref.close(true) is fine.
                this.ref.close(true);
            },
            error: (err) => {
                this.loading = false;
                console.error('Save error:', err);
                const errorDetail = err.error?.message || 'حدث خطأ أثناء الحفظ';
                this.messageService.add({ severity: 'error', summary: 'خطأ', detail: errorDetail });
                this.cdr.markForCheck();
            }
        });
    }

    close() {
        this.ref.close();
    }
}
