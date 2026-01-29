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

@Component({
  selector: 'app-country-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, InputTextModule, ButtonModule, FloatLabelModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="save()" class="p-4" dir="rtl">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            
            <!-- Ar Name -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="countryNameAr" formControlName="countryNameAr" class="w-full" />
                    <label for="countryNameAr">اسم الدولة (عربي)</label>
                </p-floatLabel>
            </div>

            <!-- En Name -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="countryNameEn" formControlName="countryNameEn" class="w-full" />
                    <label for="countryNameEn">اسم الدولة (إنجليزي)</label>
                </p-floatLabel>
            </div>

             <!-- Citizenship Ar -->
             <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="citizenshipNameAr" formControlName="citizenshipNameAr" class="w-full" />
                    <label for="citizenshipNameAr">اسم الجنسية (عربي)</label>
                </p-floatLabel>
            </div>

            <!-- Citizenship En -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="citizenshipNameEn" formControlName="citizenshipNameEn" class="w-full" />
                    <label for="citizenshipNameEn">اسم الجنسية (إنجليزي)</label>
                </p-floatLabel>
            </div>

            <!-- ISO Code -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="isoCode" formControlName="isoCode" class="w-full uppercase" />
                    <label for="isoCode">رمز الدولة (ISO)</label>
                </p-floatLabel>
            </div>

        </div>

        <div class="flex justify-end gap-2 mt-8">
            <p-button label="إلغاء" icon="pi pi-times" styleClass="p-button-text p-button-secondary" (onClick)="close()"></p-button>
            <p-button label="حفظ" icon="pi pi-check" type="submit" [loading]="loading" styleClass="p-button-primary"></p-button>
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
        private messageService: MessageService
    ) {}

    ngOnInit() {
        this.isEdit = !!this.config.data?.id;
        this.id = this.config.data?.id;

        this.form = this.fb.group({
            countryNameAr: [this.config.data?.countryNameAr || '', Validators.required],
            countryNameEn: [this.config.data?.countryNameEn || ''],
            citizenshipNameAr: [this.config.data?.citizenshipNameAr || '', Validators.required],
            citizenshipNameEn: [this.config.data?.citizenshipNameEn || ''],
            isoCode: [this.config.data?.isoCode || '', Validators.required]
        });
    }

    save() {
        if (this.form.invalid) return;

        this.loading = true;
        const payload = this.form.value;

        const request = this.isEdit
            ? this.setupService.update('Countries', this.id, payload)
            : this.setupService.create('Countries', payload);

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
