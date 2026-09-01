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
  selector: 'app-job-form',
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
                <div class="w-8 h-8 rounded-lg bg-violet-50 dark:bg-violet-900/20 text-violet-600 dark:text-violet-400 flex items-center justify-center">
                    <i [class]="isEdit ? 'pi pi-pencil' : 'pi pi-plus'" class="text-[13px]"></i>
                </div>
                <h3 class="text-sm font-bold text-slate-900 dark:text-white">
                    {{ isEdit ? 'تعديل بيانات الوظيفة' : 'إضافة وظيفة جديدة' }}
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
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">اسم الوظيفة (عربي) <span class="text-red-500">*</span></label>
                <input pInputText id="jobTitleAr" formControlName="jobTitleAr" autofocus 
                    class="w-full !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

            <!-- En Name -->
            <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">اسم الوظيفة (إنجليزي)</label>
                <input pInputText id="jobTitleEn" formControlName="jobTitleEn" 
                    class="w-full !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

            <!-- Default Grade -->
            <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">الدرجة الوظيفية الافتراضية <span class="text-red-500">*</span></label>
                <p-select 
                    [options]="jobGradesList" 
                    formControlName="defaultGradeId" 
                    optionLabel="gradeNameAr" 
                    optionValue="jobGradeId"
                    [filter]="true"
                    filterBy="gradeNameAr,gradeNameEn" 
                    [showClear]="true"
                    placeholder="اختر الدرجة الافتراضية"
                    styleClass="w-full !rounded-md !h-9 !text-[12px] shadow-none !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all [&_.p-select-label]:!py-0 [&_.p-select-label]:!px-3 [&_.p-select-label]:!leading-[34px]"
                    appendTo="body"
                    scrollHeight="250px">
                    <ng-template pTemplate="selectedItem" let-selectedOption>
                        <div class="flex align-items-center gap-2" *ngIf="selectedOption">
                            <div>{{ selectedOption.gradeNameAr }}</div>
                        </div>
                    </ng-template>
                    <ng-template pTemplate="item" let-grade>
                        <div class="flex flex-col gap-1 border-b border-slate-50 dark:border-slate-800 last:border-0 py-1">
                            <span class="font-bold text-[11px]">{{ grade.gradeNameAr }}</span>
                            <div class="flex gap-2 text-[9px] text-slate-400">
                                <span>{{ grade.minSalary | number }} - {{ grade.maxSalary | number }} ر.س</span>
                            </div>
                        </div>
                    </ng-template>
                </p-select>
            </div>

            <!-- Is Medical -->
            <div class="col-span-1 flex items-end">
                <div class="w-full h-9 flex items-center gap-2 px-3 border border-slate-100 dark:border-slate-800 bg-slate-50 dark:bg-slate-800/30 rounded-md cursor-pointer hover:bg-slate-100 dark:hover:bg-slate-800/50 transition-colors"
                     (click)="form.get('isMedical')?.setValue(!form.get('isMedical')?.value)">
                    <p-checkbox formControlName="isMedical" [binary]="true" inputId="isMedical"></p-checkbox>
                    <label for="isMedical" class="cursor-pointer text-[11px] font-bold text-slate-700 dark:text-slate-300 flex-grow flex items-center">
                        <i class="pi pi-briefcase text-violet-500 mr-2 ml-1 text-[10px]"></i>
                        هل هذه وظيفة طبية؟
                    </label>
                </div>
            </div>

        </div>

        <div class="px-5 py-4 border-t border-slate-100 dark:border-slate-800 flex justify-end gap-3 bg-slate-50/50 dark:bg-slate-900/50 mt-auto rounded-b-xl">
            <button pButton label="إلغاء" type="button"
                class="p-button-text !h-9 !text-[12px] !text-slate-600 dark:!text-slate-400 !font-bold hover:!bg-slate-200 dark:hover:!bg-slate-800 !px-5 !rounded-lg transition-colors"
                (click)="close()">
            </button>
            <button pButton [label]="isEdit ? 'حفظ التغييرات' : 'إضافة الوظيفة'" icon="pi pi-check" type="submit" [loading]="loading"
                class="p-button-primary !h-9 !bg-violet-600 hover:!bg-violet-700 !border-none !text-[12px] !px-6 !rounded-lg shadow-sm font-bold transition-colors">
            </button>
        </div>
    </form>
  `
})
export class JobFormComponent implements OnInit {
    form!: FormGroup;
    loading = false;
    isEdit = false;
    id!: number;
    jobGradesList: any[] = [];

    constructor(
        private fb: FormBuilder,
        public ref: DynamicDialogRef,
        public config: DynamicDialogConfig,
        private setupService: SetupService,
        private messageService: MessageService,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit() {
        this.isEdit = !!this.config.data?.jobId;
        this.id = this.config.data?.jobId;

        // Initialize Form
        this.form = this.fb.group({
            jobTitleAr: [this.config.data?.jobTitleAr || '', Validators.required],
            jobTitleEn: [this.config.data?.jobTitleEn || ''],
            defaultGradeId: [this.config.data?.defaultGradeId || null, Validators.required],
            isMedical: [this.config.data?.isMedical === 1 || this.config.data?.isMedical === true || false]
        });

        this.loadLookups();
    }

    loadLookups() {
        this.setupService.getAll<any>('JobGrades').subscribe({
            next: (res: any) => {
                this.jobGradesList = res.data?.items || res.items || res.data || res || [];
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
            isMedical: formVal.isMedical ? 1 : 0 // Convert boolean to 1/0
        };

        const request = this.isEdit
            ? this.setupService.update('Jobs', this.id, { jobId: this.id, ...payload })
            : this.setupService.create('Jobs', payload);

        request.subscribe({
            next: () => {
                this.loading = false;
                this.messageService.add({severity:'success', summary:'نجاح', detail: 'تم الحفظ بنجاح'});
                this.ref.close(true);
            },
            error: (err) => {
                this.loading = false;
                this.messageService.add({severity:'error', summary:'خطأ', detail: 'حدث خطأ أثناء الحفظ'});
                this.cdr.markForCheck();
            }
        });
    }

    close() {
        this.ref.close();
    }
}
