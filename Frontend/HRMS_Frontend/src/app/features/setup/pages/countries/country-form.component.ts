import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FloatLabelModule } from 'primeng/floatlabel';
import { CreateCountryCommand, Country } from '../../models/setup.models';
import { SetupService } from '../../services/setup.service';
import { MessageService } from 'primeng/api';
import { ChangeDetectorRef } from '@angular/core';

@Component({
    selector: 'app-country-form',
    standalone: true,
    imports: [CommonModule, FormsModule, ReactiveFormsModule, InputTextModule, ButtonModule, FloatLabelModule],
    template: `
    <form [formGroup]="form" (ngSubmit)="save()" class="flex flex-col h-full" dir="rtl">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-x-4 gap-y-3 p-1">
            
            <!-- Ar Name -->
            <div class="col-span-1 space-y-1">
                <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300">اسم الدولة (عربي) <span class="text-red-500">*</span></label>
                <input pInputText id="countryNameAr" formControlName="countryNameAr" 
                    class="w-full !h-8 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[11px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

            <!-- En Name -->
            <div class="col-span-1 space-y-1">
                <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300">اسم الدولة (إنجليزي) <span class="text-red-500">*</span></label>
                <input pInputText id="countryNameEn" formControlName="countryNameEn" 
                    class="w-full !h-8 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[11px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

             <!-- Citizenship Ar -->
             <div class="col-span-1 space-y-1">
                <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300">اسم الجنسية (عربي)</label>
                <input pInputText id="citizenshipNameAr" formControlName="citizenshipNameAr" 
                    class="w-full !h-8 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[11px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>      

            <!-- Citizenship En -->
            <div class="col-span-1 space-y-1">
                <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300">اسم الجنسية (إنجليزي)</label>
                <input pInputText id="citizenshipNameEn" formControlName="citizenshipNameEn" 
                    class="w-full !h-8 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[11px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

            <!-- ISO Code -->
            <div class="col-span-1 space-y-1">
                <label class="block text-[10px] font-bold text-slate-700 dark:text-slate-300">رمز الدولة (ISO) <span class="text-red-500">*</span></label>
                <input pInputText id="isoCode" formControlName="isoCode" maxlength="2"
                    class="w-full uppercase !h-8 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[11px] font-mono tracking-widest focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

        </div>

        <div class="flex justify-end gap-2 mt-6 pt-4 border-t border-slate-100 dark:border-slate-800">
            <button pButton label="إلغاء" type="button"
                class="p-button-text !h-8 !text-[11px] !text-slate-600 dark:!text-slate-400 !font-bold hover:!bg-slate-200 dark:hover:!bg-slate-800 !px-4 !rounded-md transition-colors"
                (click)="close()">
            </button>
            <button pButton [label]="isEdit ? 'حفظ التغييرات' : 'إضافة الدولة'" icon="pi pi-check" type="submit" [loading]="loading"
                class="p-button-primary !h-8 !bg-blue-600 hover:!bg-blue-700 !border-none !text-[11px] !px-4 !rounded-md shadow-sm font-bold transition-colors">
            </button>
        </div>
    </form>
  `
})
export class CountryFormComponent implements OnInit {
    form!: FormGroup;
    loading = false;
    isEdit = false;
    id!: number;

    constructor(
        private fb: FormBuilder,
        public ref: DynamicDialogRef,
        public config: DynamicDialogConfig,
        private setupService: SetupService,
        private messageService: MessageService,
        private cd: ChangeDetectorRef
    ) { }

    ngOnInit() {
        this.isEdit = !!this.config.data?.id;
        this.id = this.config.data?.id;

        this.form = this.fb.group({
            countryNameAr: [this.config.data?.countryNameAr || '', [Validators.required, Validators.maxLength(100)]],
            countryNameEn: [this.config.data?.countryNameEn || '', [Validators.required, Validators.maxLength(100)]],
            citizenshipNameAr: [this.config.data?.citizenshipNameAr || '', [Validators.maxLength(100)]],
            citizenshipNameEn: [this.config.data?.citizenshipNameEn || '', [Validators.maxLength(100)]],
            isoCode: [this.config.data?.isoCode || '', [Validators.required, Validators.minLength(2), Validators.maxLength(2), Validators.pattern('^[a-zA-Z]{2}$')]]
        });
    }

    save() {
        if (this.form.invalid) return;

        this.loading = true;
        this.cd.detectChanges();
        const payload = this.form.value;

        const request = this.isEdit
            ? this.setupService.update('Countries', this.id, payload)
            : this.setupService.create('Countries', payload);

        request.subscribe({
            next: () => {
                this.loading = false;
                this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الحفظ بنجاح' });
                this.ref.close(true);
            },
            error: () => {
                this.loading = false;
                this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء الحفظ' });
                this.cd.detectChanges();
            }
        });
    }

    close() {
        this.ref.close();
    }
}
