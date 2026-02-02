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
    <form [formGroup]="form" (ngSubmit)="save()" class="p-4" dir="rtl">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            
            <!-- Ar Name -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="deptNameAr" formControlName="deptNameAr" class="w-full" autofocus />
                    <label for="deptNameAr">اسم القسم (عربي)</label>
                </p-floatLabel>
            </div>

            <!-- En Name -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="deptNameEn" formControlName="deptNameEn" class="w-full" />
                    <label for="deptNameEn">اسم القسم (إنجليزي)</label>
                </p-floatLabel>
            </div>

            <!-- Parent Department (Select with Search) -->
            <div class="col-span-1">
                <p-floatLabel>
                    <p-select 
                        [options]="departmentsList" 
                        formControlName="parentDeptId" 
                        optionLabel="deptNameAr" 
                        optionValue="deptId"
                        [filter]="true"
                        filterBy="deptNameAr,deptNameEn" 
                        [showClear]="true"
                        placeholder="استخدم البحث للعثور على القسم"
                        styleClass="w-full"
                        appendTo="body"
                        scrollHeight="250px">
                        <ng-template pTemplate="selectedItem" let-selectedOption>
                            <div class="flex align-items-center gap-2" *ngIf="selectedOption">
                                <div>{{ selectedOption.deptNameAr }}</div>
                            </div>
                        </ng-template>
                        <ng-template pTemplate="item" let-dept>
                            <div class="flex align-items-center gap-2" style="height: 35px;">
                                <div>{{ dept.deptNameAr }}</div>
                            </div>
                        </ng-template>
                    </p-select>
                    <label for="parentDeptId">القسم الرئيسي</label>
                </p-floatLabel>
            </div>

             <!-- Manager (Select with Search from API) -->
             <div class="col-span-1">
                <p-floatLabel>
                    <p-select 
                        [options]="employeesList" 
                        formControlName="managerId" 
                        optionLabel="fullNameAr" 
                        optionValue="employeeId"
                        [filter]="true"
                        filterBy="fullNameAr,fullNameEn,employeeNumber" 
                        [showClear]="true"
                        placeholder="ابحث باسم الموظف أو رقمه"
                        styleClass="w-full"
                        appendTo="body"
                        scrollHeight="250px">
                        <ng-template pTemplate="selectedItem" let-selectedOption>
                            <div class="flex align-items-center gap-2" *ngIf="selectedOption">
                                <i class="pi pi-user text-slate-500"></i>
                                <div>{{ selectedOption.fullNameAr }}</div>
                            </div>
                        </ng-template>
                        <ng-template pTemplate="item" let-emp>
                            <div class="flex flex-col justify-center h-full border-b border-gray-100 last:border-0 py-1">
                                <span class="font-bold text-sm">{{ emp.fullNameAr }}</span>
                                <span class="text-xs text-slate-500">{{ emp.jobTitle || 'بدون مسمى' }}</span>
                            </div>
                        </ng-template>
                    </p-select>
                    <label for="managerId">مدير القسم</label>
                </p-floatLabel>
            </div>

             <!-- Cost Center -->
             <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="costCenterCode" formControlName="costCenterCode" class="w-full uppercase" />
                    <label for="costCenterCode">مركز التكلفة</label>
                </p-floatLabel>
            </div>

            <!-- Is Active -->
            <div class="col-span-1 flex items-center h-full">
                <div class="flex items-center gap-2">
                    <p-checkbox formControlName="isActive" [binary]="true" inputId="isActive"></p-checkbox>
                    <label for="isActive" class="cursor-pointer select-none text-slate-700 dark:text-slate-300">نشط / مفعل</label>
                </div>
            </div>
        </div>

        <div class="flex justify-end gap-2 mt-8 pt-4 border-t border-slate-100 dark:border-zinc-800">
            <p-button label="إلغاء" icon="pi pi-times" styleClass="p-button-text p-button-secondary" (onClick)="close()"></p-button>
            <p-button label="حفظ التغييرات" icon="pi pi-check" type="submit" [loading]="loading" styleClass="p-button-primary"></p-button>
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
    ) {}

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
                this.employeesList = res.items || res.data || res || [];
                // Manually trigger change detection after data loads
                this.cdr.markForCheck(); 
            },
            error: () => this.cdr.markForCheck()
        });

        // 2. Load Departments
        this.setupService.getAll<any>('Departments').subscribe({
            next: (res: any) => {
                let allDepts = res.items || res.data || res || [];
                if (this.isEdit) {
                    allDepts = allDepts.filter((d: any) => d.deptId !== this.id);
                }
                this.departmentsList = allDepts;
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
            managerId: formVal.managerId || 0,
            isActive: formVal.isActive ? 1 : 0
        };

        const request = this.isEdit
            ? this.setupService.update('Departments', this.id, { deptId: this.id, ...payload })
            : this.setupService.create('Departments', payload);

        request.subscribe({
            next: () => {
                this.loading = false;
                this.messageService.add({severity:'success', summary:'نجاح', detail: 'تم الحفظ بنجاح'});
                this.ref.close(true);
            },
            error: (err) => {
                this.loading = false;
                console.error(err);
                this.messageService.add({severity:'error', summary:'خطأ', detail: 'حدث خطأ أثناء الحفظ'});
                this.cdr.markForCheck();
            }
        });
    }

    close() {
        this.ref.close();
    }
}
