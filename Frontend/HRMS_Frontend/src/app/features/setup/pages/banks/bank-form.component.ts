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
    <form [formGroup]="form" (ngSubmit)="save()" class="p-4" dir="rtl">
        <div class="grid grid-cols-1 gap-6">
            
            <!-- Ar Name -->
            <div class="field">
                <p-floatLabel>
                    <input pInputText id="bankNameAr" formControlName="bankNameAr" class="w-full" />
                    <label for="bankNameAr">اسم البنك (عربي)</label>
                </p-floatLabel>
            </div>

            <!-- En Name -->
            <div class="field">
                <p-floatLabel>
                    <input pInputText id="bankNameEn" formControlName="bankNameEn" class="w-full" />
                    <label for="bankNameEn">اسم البنك (إنجليزي)</label>
                </p-floatLabel>
            </div>

             <!-- SWIFT Code -->
             <div class="field">
                <p-floatLabel>
                    <input pInputText id="swiftCode" formControlName="swiftCode" class="w-full uppercase" />
                    <label for="swiftCode">سويفت كود (SWIFT Code)</label>
                </p-floatLabel>
            </div>

            <!-- Is Active -->
            <div class="field flex items-center gap-2 p-2 border border-slate-100 rounded-lg">
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
