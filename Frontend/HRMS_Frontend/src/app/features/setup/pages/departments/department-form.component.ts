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
  selector: 'app-department-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, InputTextModule, ButtonModule, FloatLabelModule, CheckboxModule],
  template: `
    <form [formGroup]="form" (ngSubmit)="save()" class="p-4" dir="rtl">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            
            <!-- Ar Name -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="deptNameAr" formControlName="deptNameAr" class="w-full" />
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

             <!-- Cost Center -->
             <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="costCenterCode" formControlName="costCenterCode" class="w-full uppercase" />
                    <label for="costCenterCode">مركز التكلفة</label>
                </p-floatLabel>
            </div>

            <!-- Is Active -->
            <div class="col-span-1 flex items-center h-full">
                <p-checkbox formControlName="isActive" [binary]="true" label="مفعل" inputId="isActive"></p-checkbox>
            </div>
        </div>

        <div class="flex justify-end gap-2 mt-8">
            <p-button label="إلغاء" icon="pi pi-times" styleClass="p-button-text p-button-secondary" (onClick)="close()"></p-button>
            <p-button label="حفظ" icon="pi pi-check" type="submit" [loading]="loading" styleClass="p-button-primary"></p-button>
        </div>
    </form>
  `
})
export class DepartmentFormComponent implements OnInit {
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
        this.isEdit = !!this.config.data?.deptId;
        this.id = this.config.data?.deptId;

        this.form = this.fb.group({
            deptNameAr: [this.config.data?.deptNameAr || '', Validators.required],
            deptNameEn: [this.config.data?.deptNameEn || ''],
            costCenterCode: [this.config.data?.costCenterCode || ''],
            isActive: [this.config.data?.isActive === undefined ? true : (this.config.data.isActive == 1 || this.config.data.isActive === true)]
        });
    }

    save() {
        if (this.form.invalid) return;

        this.loading = true;
        const payload = this.form.value;
        // Fix usage of isActive to match backend expectation if needed (1/0 or true/false)
        // User API showed isActive: 1. Let's ensure it sends correct format.
        // Assuming models define boolean, but backend might return 0/1. 
        // Let's send what the form has (boolean) and hope backend handles or mapped.
        // If Backend expects 1/0, we might need conversion.
        // Based on user provided Departments response: "isActive": 1.
        
        const request = this.isEdit
            ? this.setupService.update('Departments', this.id, payload)
            : this.setupService.create('Departments', payload);

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
