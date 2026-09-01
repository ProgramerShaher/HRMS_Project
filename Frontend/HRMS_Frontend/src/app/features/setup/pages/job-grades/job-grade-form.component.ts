import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FloatLabelModule } from 'primeng/floatlabel';
import { CheckboxModule } from 'primeng/checkbox';
import { SetupService } from '../../services/setup.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-job-grade-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, InputTextModule, InputNumberModule, ButtonModule, FloatLabelModule, CheckboxModule],
    template: `
    <form [formGroup]="form" (ngSubmit)="save()" class="flex flex-col h-full bg-white dark:bg-slate-900 rounded-xl shadow-2xl border border-slate-200 dark:border-slate-800" dir="rtl">
        
        <!-- Dialog Header -->
        <div class="px-5 py-4 border-b border-slate-100 dark:border-slate-800 flex justify-between items-center bg-slate-50/50 dark:bg-slate-900/50 rounded-t-xl">
            <div class="flex items-center gap-3">
                <div class="w-8 h-8 rounded-lg bg-blue-50 dark:bg-blue-900/20 text-blue-600 dark:text-blue-400 flex items-center justify-center">
                    <i [class]="isEdit ? 'pi pi-pencil' : 'pi pi-plus'" class="text-[13px]"></i>
                </div>
                <h3 class="text-sm font-bold text-slate-900 dark:text-white">
                    {{ isEdit ? 'تعديل الدرجة الوظيفية' : 'إضافة درجة وظيفية جديدة' }}
                </h3>
            </div>
            <button type="button"
                class="w-8 h-8 flex items-center justify-center rounded-lg text-slate-400 hover:text-red-500 hover:bg-red-50 dark:hover:bg-red-900/20 transition-all"
                (click)="close()">
                <i class="pi pi-times text-[12px]"></i>
            </button>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-x-5 gap-y-4 p-5 overflow-y-auto">
            
            <!-- Code -->
            <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">رمز الدرجة <span class="text-red-500">*</span></label>
                <input pInputText id="gradeCode" formControlName="gradeCode" 
                    class="w-full uppercase !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] font-mono tracking-wider focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
                <small class="text-red-500 text-[10px] block mt-0.5 font-bold" *ngIf="form.get('gradeCode')?.invalid && form.get('gradeCode')?.touched">رمز الدرجة مطلوب</small>
            </div>

            <!-- Level -->
            <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">مستوى الدرجة <span class="text-red-500">*</span></label>
                <p-inputNumber inputId="gradeLevel" formControlName="gradeLevel" [min]="1" 
                    styleClass="w-full" class="w-full" 
                    inputStyleClass="w-full !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all"></p-inputNumber>
                <small class="text-red-500 text-[10px] block mt-0.5 font-bold" *ngIf="form.get('gradeLevel')?.invalid && form.get('gradeLevel')?.touched">مستوى الدرجة مطلوب</small>
            </div>

            <!-- Ar Name -->
            <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">اسم الدرجة (عربي) <span class="text-red-500">*</span></label>
                <input pInputText id="gradeNameAr" formControlName="gradeNameAr" 
                    class="w-full !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
                <small class="text-red-500 text-[10px] block mt-0.5 font-bold" *ngIf="form.get('gradeNameAr')?.invalid && form.get('gradeNameAr')?.touched">الاسم العربي مطلوب</small>
            </div>

            <!-- En Name -->
            <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">اسم الدرجة (إنجليزي)</label>
                <input pInputText id="gradeNameEn" formControlName="gradeNameEn" 
                    class="w-full !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

            <!-- Min Salary -->
             <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">الحد الأدنى للراتب</label>
                <p-inputNumber inputId="minSalary" formControlName="minSalary" mode="decimal" [minFractionDigits]="2" [maxFractionDigits]="2" suffix=" ر.س" 
                    styleClass="w-full" class="w-full" 
                    inputStyleClass="w-full !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] font-mono focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all"></p-inputNumber>
            </div>

            <!-- Max Salary -->
             <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">الحد الأعلى للراتب</label>
                <p-inputNumber inputId="maxSalary" formControlName="maxSalary" mode="decimal" [minFractionDigits]="2" [maxFractionDigits]="2" suffix=" ر.س" 
                    styleClass="w-full" class="w-full" 
                    inputStyleClass="w-full !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] font-mono focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all"></p-inputNumber>
            </div>

            <!-- Is Active -->
            <div class="col-span-1 md:col-span-2 flex items-end">
                <div class="w-full md:w-1/2 h-9 flex items-center gap-2 px-3 border border-slate-100 dark:border-slate-800 bg-slate-50 dark:bg-slate-800/30 rounded-md cursor-pointer hover:bg-slate-100 dark:hover:bg-slate-800/50 transition-colors"
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
            <button pButton [label]="isEdit ? 'حفظ التغييرات' : 'إضافة الدرجة'" icon="pi pi-check" type="submit" [loading]="loading"
                class="p-button-primary !h-9 !bg-blue-600 hover:!bg-blue-700 !border-none !text-[12px] !px-6 !rounded-lg shadow-sm font-bold transition-colors">
            </button>
        </div>
    </form>
  `
})
export class JobGradeFormComponent implements OnInit {
    form!: FormGroup;
    loading = false;
    isEdit = false;
    id!: number;

    constructor(
        private fb: FormBuilder,
        public ref: DynamicDialogRef,
        public config: DynamicDialogConfig,
        private setupService: SetupService,
        private messageService: MessageService
    ) {}

    ngOnInit() {
        this.isEdit = !!this.config.data?.jobGradeId;
        this.id = this.config.data?.jobGradeId;

        this.form = this.fb.group({
            gradeCode: [this.config.data?.gradeCode || '', Validators.required],
            gradeLevel: [this.config.data?.gradeLevel || null, [Validators.required, Validators.min(1)]],
            gradeNameAr: [this.config.data?.gradeNameAr || '', Validators.required],
            gradeNameEn: [this.config.data?.gradeNameEn || ''],
            minSalary: [this.config.data?.minSalary || 0],
            maxSalary: [this.config.data?.maxSalary || 0],
            isActive: [this.config.data?.isActive === undefined ? true : (this.config.data.isActive == 1 || this.config.data.isActive === true)]
        });
    }

    save() {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.loading = true;
        const payload = this.form.value;
        const request = this.isEdit
            ? this.setupService.update('JobGrades', this.id, payload)
            : this.setupService.create('JobGrades', payload);

        request.subscribe({
            next: (res: any) => {
                this.loading = false;
                if (res.succeeded) {
                    this.messageService.add({severity:'success', summary:'نجاح', detail: 'تم الحفظ بنجاح'});
                    this.ref.close(true);
                } else {
                     this.messageService.add({severity:'error', summary:'خطأ', detail: res.message || 'فشل الحفظ'});
                }
            },
            error: (err) => {
                this.loading = false;
                console.error('Error saving Job Grade:', err);
                
                // Handle FluentValidation ValidationException format
                if (err.error && err.error.errors) {
                    const validationErrors = err.error.errors;
                    Object.keys(validationErrors).forEach(key => {
                         const messages = validationErrors[key];
                         messages.forEach((msg: string) => {
                             this.messageService.add({severity:'error', summary: 'خطأ تحقق', detail: msg});
                         });
                    });
                } else {
                    this.messageService.add({severity:'error', summary:'خطأ', detail: 'حدث خطأ أثناء الحفظ'});
                }
            }
        });
    }

    close() {
        this.ref.close();
    }
}
