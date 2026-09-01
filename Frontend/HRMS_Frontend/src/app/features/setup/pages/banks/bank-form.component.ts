import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FloatLabelModule } from 'primeng/floatlabel';
import { CheckboxModule } from 'primeng/checkbox';
import { SetupService } from '../../services/setup.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-bank-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, InputTextModule, ButtonModule, FloatLabelModule, CheckboxModule],
    template: `
    <form [formGroup]="form" (ngSubmit)="save()" class="flex flex-col h-full bg-white dark:bg-slate-900 rounded-xl shadow-2xl border border-slate-200 dark:border-slate-800" dir="rtl">
        
        <!-- Dialog Header -->
        <div class="px-5 py-4 border-b border-slate-100 dark:border-slate-800 flex justify-between items-center bg-slate-50/50 dark:bg-slate-900/50 rounded-t-xl">
            <div class="flex items-center gap-3">
                <div class="w-8 h-8 rounded-lg bg-blue-50 dark:bg-blue-900/20 text-blue-600 dark:text-blue-400 flex items-center justify-center">
                    <i [class]="isEdit ? 'pi pi-pencil' : 'pi pi-plus'" class="text-[13px]"></i>
                </div>
                <h3 class="text-sm font-bold text-slate-900 dark:text-white">
                    {{ isEdit ? 'تعديل بيانات البنك' : 'إضافة بنك جديد' }}
                </h3>
            </div>
            <button type="button"
                class="w-8 h-8 flex items-center justify-center rounded-lg text-slate-400 hover:text-red-500 hover:bg-red-50 dark:hover:bg-red-900/20 transition-all"
                (click)="close()">
                <i class="pi pi-times text-[12px]"></i>
            </button>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-x-5 gap-y-4 p-5">
            
            <!-- Ar Name -->
            <div class="col-span-1 md:col-span-2 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">اسم البنك (عربي) <span class="text-red-500">*</span></label>
                <input pInputText id="bankNameAr" formControlName="bankNameAr" 
                    class="w-full !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

            <!-- En Name -->
            <div class="col-span-1 md:col-span-2 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">اسم البنك (إنجليزي)</label>
                <input pInputText id="bankNameEn" formControlName="bankNameEn" 
                    class="w-full !h-9 !px-3 !rounded-md !bg-slate-50 dark:!bg-slate-800/50 !border-slate-200 dark:!border-slate-700 !text-[12px] focus:!bg-white dark:focus:!bg-slate-900 focus:!border-blue-500 transition-all" />
            </div>

             <!-- SWIFT Code -->
             <div class="col-span-1 space-y-1.5">
                <label class="block text-[11px] font-bold text-slate-700 dark:text-slate-300">سويفت كود (SWIFT Code)</label>
                <input pInputText id="swiftCode" formControlName="swiftCode" 
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
            <button pButton [label]="isEdit ? 'حفظ التغييرات' : 'إضافة البنك'" icon="pi pi-check" type="submit" [loading]="loading"
                class="p-button-primary !h-9 !bg-blue-600 hover:!bg-blue-700 !border-none !text-[12px] !px-6 !rounded-lg shadow-sm font-bold transition-colors">
            </button>
        </div>
    </form>
  `
})
export class BankFormComponent implements OnInit {
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
        this.isEdit = !!this.config.data?.bankId;
        this.id = this.config.data?.bankId;

        this.form = this.fb.group({
            bankNameAr: [this.config.data?.bankNameAr || '', Validators.required],
            bankNameEn: [this.config.data?.bankNameEn || ''],
            swiftCode: [this.config.data?.swiftCode || ''],
            isActive: [this.config.data?.isActive === undefined ? true : (this.config.data.isActive == 1 || this.config.data.isActive === true)]
        });
    }

    save() {
        if (this.form.invalid) return;

        this.loading = true;
        const payload = this.form.value;
        const request = this.isEdit
            ? this.setupService.update('banks', this.id, payload)
            : this.setupService.create('banks', payload);

        request.subscribe({
            next: () => {
                this.loading = false;
                this.messageService.add({severity:'success', summary:'نجاح', detail: 'تم الحفظ بنجاح'});
                this.ref.close(true);
            },
            error: () => {
                this.loading = false;
                this.messageService.add({severity:'error', summary:'خطأ', detail: 'حدث خطأ أثناء الحفظ'});
            }
        });
    }

    close() {
        this.ref.close();
    }
}
