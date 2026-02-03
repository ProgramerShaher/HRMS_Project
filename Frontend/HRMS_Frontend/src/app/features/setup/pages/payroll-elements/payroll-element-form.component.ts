import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { SetupService } from '../../services/setup.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-payroll-element-form',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    ButtonModule, 
    InputTextModule, 
    CheckboxModule, 
    InputNumberModule,
    SelectModule
  ],
  templateUrl: './payroll-element-form.component.html',
  styleUrls: ['./payroll-element-form.component.scss']
})
export class PayrollElementFormComponent implements OnInit {
  fb = inject(FormBuilder);
  ref = inject(DynamicDialogRef);
  config = inject(DynamicDialogConfig);
  setupService = inject(SetupService);
  messageService = inject(MessageService);

  form!: FormGroup;
  isEditMode = false;
  loading = false;

  elementTypes = [
    { label: 'استحقاق', value: 'EARNING' },
    { label: 'استقطاع', value: 'DEDUCTION' }
  ];

  ngOnInit() {
    this.initForm();
    if (this.config.data && this.config.data.elementId) {
      this.isEditMode = true;
      this.form.patchValue(this.config.data);
    }
  }

  initForm() {
    this.form = this.fb.group({
      elementId: [null],
      elementNameAr: ['', [Validators.required]],
      elementType: ['EARNING', [Validators.required]],
      isTaxable: [false],
      isGosiBase: [false],
      isRecurring: [true],
      isBasic: [false],
      defaultPercentage: [0, [Validators.min(0), Validators.max(100)]]
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.loading = true;
    const formValue = this.form.value;

    if (this.isEditMode) {
      this.setupService.update('PayrollSettings/elements', formValue.elementId, formValue).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم التعديل بنجاح' });
          this.ref.close(true);
          this.loading = false;
        },
        error: () => this.loading = false
      });
    } else {
      this.setupService.create('PayrollSettings/elements', formValue).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الإضافة بنجاح' });
          this.ref.close(true);
          this.loading = false;
        },
        error: () => this.loading = false
      });
    }
  }

  onCancel() {
    this.ref.close(false);
  }
}
