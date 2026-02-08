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
    <form [formGroup]="form" (ngSubmit)="save()" class="p-4" dir="rtl">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            
            <!-- Code -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="gradeCode" formControlName="gradeCode" class="w-full" />
                    <label for="gradeCode">رمز الدرجة</label>
                </p-floatLabel>
                <small class="p-error block mt-1" *ngIf="form.get('gradeCode')?.invalid && form.get('gradeCode')?.touched">رمز الدرجة مطلوب</small>
            </div>

            <!-- Level -->
            <div class="col-span-1">
                <p-floatLabel>
                    <p-inputNumber inputId="gradeLevel" formControlName="gradeLevel" [min]="1" styleClass="w-full" class="w-full" inputStyleClass="w-full"></p-inputNumber>
                    <label for="gradeLevel">مستوى الدرجة</label>
                </p-floatLabel>
                <small class="p-error block mt-1" *ngIf="form.get('gradeLevel')?.invalid && form.get('gradeLevel')?.touched">مستوى الدرجة مطلوب</small>
            </div>

            <!-- Ar Name -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="gradeNameAr" formControlName="gradeNameAr" class="w-full" />
                    <label for="gradeNameAr">اسم الدرجة (عربي)</label>
                </p-floatLabel>
                <small class="p-error block mt-1" *ngIf="form.get('gradeNameAr')?.invalid && form.get('gradeNameAr')?.touched">الاسم العربي مطلوب</small>
            </div>

            <!-- En Name -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="gradeNameEn" formControlName="gradeNameEn" class="w-full" />
                    <label for="gradeNameEn">اسم الدرجة (إنجليزي)</label>
                </p-floatLabel>
            </div>

            <!-- Min Salary -->
             <div class="col-span-1">
                <p-floatLabel>
                    <p-inputNumber inputId="minSalary" formControlName="minSalary" mode="decimal" [minFractionDigits]="2" [maxFractionDigits]="2" suffix=" ر.س" styleClass="w-full" class="w-full" inputStyleClass="w-full"></p-inputNumber>
                    <label for="minSalary">الحد الأدنى للراتب</label>
                </p-floatLabel>
            </div>

            <!-- Max Salary -->
             <div class="col-span-1">
                <p-floatLabel>
                    <p-inputNumber inputId="maxSalary" formControlName="maxSalary" mode="decimal" [minFractionDigits]="2" [maxFractionDigits]="2" suffix=" ر.س" styleClass="w-full" class="w-full" inputStyleClass="w-full"></p-inputNumber>
                    <label for="maxSalary">الحد الأعلى للراتب</label>
                </p-floatLabel>
            </div>

            <!-- Is Active -->
            <div class="col-span-2 flex items-center gap-2 p-2 border border-slate-100 rounded-lg">
                <p-checkbox formControlName="isActive" [binary]="true" inputId="isActive"></p-checkbox>
                <label for="isActive" class="cursor-pointer">حالة التفعيل (نشط)</label>
            </div>
        </div>

        <div class="flex justify-end gap-2 mt-8">
            <p-button label="إلغاء" icon="pi pi-times" styleClass="p-button-text p-button-secondary" (onClick)="close()"></p-button>
            <p-button label="حفظ" icon="pi pi-check" type="submit" [loading]="loading" styleClass="p-button-primary"></p-button>
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
