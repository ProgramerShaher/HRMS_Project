import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FloatLabelModule } from 'primeng/floatlabel';
import { SelectModule } from 'primeng/select';
import { SetupService } from '../../services/setup.service';
import { MessageService } from 'primeng/api';
import { Country, PaginatedResult } from '../../models/setup.models';

@Component({
  selector: 'app-city-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, InputTextModule, ButtonModule, FloatLabelModule, SelectModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="save()" class="p-4" dir="rtl">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            
            <!-- Ar Name -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="cityNameAr" formControlName="cityNameAr" class="w-full" />
                    <label for="cityNameAr">اسم المدينة (عربي)</label>
                </p-floatLabel>
            </div>

            <!-- En Name -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="cityNameEn" formControlName="cityNameEn" class="w-full" />
                    <label for="cityNameEn">اسم المدينة (إنجليزي)</label>
                </p-floatLabel>
            </div>

             <!-- Country Select -->
             <div class="col-span-2">
                <p-floatLabel>
                    <p-select [options]="countries" formControlName="countryId" optionLabel="countryNameAr" optionValue="countryId" [filter]="true" filterBy="countryNameAr" styleClass="w-full" appendTo="body"></p-select>
                    <label>الدولة التابعة لها</label>
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
export class CityFormComponent implements OnInit {
    form!: FormGroup;
    loading = false;
    isEdit = false;
    id!: number;
    countries: Country[] = [];

    constructor(
        private fb: FormBuilder,
        public ref: DynamicDialogRef,
        public config: DynamicDialogConfig,
        private setupService: SetupService,
        private messageService: MessageService
    ) {}

    ngOnInit() {
        this.isEdit = !!this.config.data?.cityId;
        this.id = this.config.data?.cityId;
        
        // Load Countries for Dropdown
        this.loadCountries();

        this.form = this.fb.group({
            cityNameAr: [this.config.data?.cityNameAr || '', Validators.required],
            cityNameEn: [this.config.data?.cityNameEn || ''],
            countryId: [this.config.data?.countryId || null, Validators.required]
        });
    }

    loadCountries() {
        // Assuming Get All Countries endpoint
        this.setupService.getAll<PaginatedResult<Country>>('Countries').subscribe({
            next: (res: any) => {
                 const list = res?.data?.items || res?.items || [];
                 this.countries = list;
            }
        });
    }

    save() {
        if (this.form.invalid) return;

        this.loading = true;
        const payload = this.form.value;

        const request = this.isEdit
            ? this.setupService.update('Cities', this.id, payload)
            : this.setupService.create('Cities', payload);

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
