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
    <form [formGroup]="form" (ngSubmit)="save()" class="p-4" dir="rtl">
        <div class="grid grid-cols-1 gap-6">
            
            <!-- Ar Name -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="jobTitleAr" formControlName="jobTitleAr" class="w-full" autofocus />
                    <label for="jobTitleAr">اسم الوظيفة (عربي)</label>
                </p-floatLabel>
            </div>

            <!-- En Name -->
            <div class="col-span-1">
                <p-floatLabel>
                    <input pInputText id="jobTitleEn" formControlName="jobTitleEn" class="w-full" />
                    <label for="jobTitleEn">اسم الوظيفة (إنجليزي)</label>
                </p-floatLabel>
            </div>

            <!-- Default Grade -->
            <div class="col-span-1">
                <p-floatLabel>
                    <p-select 
                        [options]="jobGradesList" 
                        formControlName="defaultGradeId" 
                        optionLabel="gradeNameAr" 
                        optionValue="jobGradeId"
                        [filter]="true"
                        filterBy="gradeNameAr,gradeNameEn" 
                        [showClear]="true"
                        placeholder="اختر الدرجة الافتراضية"
                        styleClass="w-full"
                        appendTo="body"
                        scrollHeight="250px">
                        <ng-template pTemplate="selectedItem" let-selectedOption>
                            <div class="flex align-items-center gap-2" *ngIf="selectedOption">
                                <div>{{ selectedOption.gradeNameAr }}</div>
                            </div>
                        </ng-template>
                        <ng-template pTemplate="item" let-grade>
                            <div class="flex flex-col gap-1 border-b border-gray-50 last:border-0 py-1">
                                <span class="font-bold text-sm">{{ grade.gradeNameAr }}</span>
                                <div class="flex gap-2 text-xs text-slate-400">
                                    <span>{{ grade.minSalary | number }} - {{ grade.maxSalary | number }} ر.س</span>
                                </div>
                            </div>
                        </ng-template>
                    </p-select>
                    <label for="defaultGradeId">الدرجة الوظيفية الافتراضية</label>
                </p-floatLabel>
            </div>

            <!-- Is Medical -->
            <div class="col-span-1 flex items-center h-full p-2 border border-slate-100 rounded-xl bg-slate-50 dark:bg-zinc-900/50 dark:border-zinc-800">
                <div class="flex items-center gap-2">
                    <p-checkbox formControlName="isMedical" [binary]="true" inputId="isMedical"></p-checkbox>
                    <label for="isMedical" class="cursor-pointer select-none text-slate-700 dark:text-slate-300 font-medium">
                        <i class="pi pi-briefcase text-blue-500 mr-2 ml-1"></i>
                        هل هذه وظيفة طبية؟
                    </label>
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
