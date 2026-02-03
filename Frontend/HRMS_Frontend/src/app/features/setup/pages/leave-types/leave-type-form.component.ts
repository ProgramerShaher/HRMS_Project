import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { InputNumberModule } from 'primeng/inputnumber';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { SetupService } from '../../services/setup.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-leave-type-form',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    ButtonModule, 
    InputTextModule, 
    CheckboxModule, 
    InputNumberModule
  ],
  templateUrl: './leave-type-form.component.html',
  styleUrls: ['./leave-type-form.component.scss']
})
export class LeaveTypeFormComponent implements OnInit {
  fb = inject(FormBuilder);
  ref = inject(DynamicDialogRef);
  config = inject(DynamicDialogConfig);
  setupService = inject(SetupService);
  messageService = inject(MessageService);

  form!: FormGroup;
  isEditMode = false;
  loading = false;

  ngOnInit() {
    this.initForm();
    if (this.config.data && this.config.data.leaveTypeId) {
      this.isEditMode = true;
      // Convert 1/0 numbers to booleans for the checkbox
      const patchData = {
        ...this.config.data,
        isDeductible: this.config.data.isDeductible === 1,
        requiresAttachment: this.config.data.requiresAttachment === 1
      };
      this.form.patchValue(patchData);
    }
  }

  initForm() {
    this.form = this.fb.group({
      leaveTypeId: [null],
      leaveTypeNameAr: ['', [Validators.required]],
      defaultDays: [0, [Validators.required, Validators.min(0)]],
      isDeductible: [true], // Default true? User didn't specify, safely assume mostly yes or check existing data.
      requiresAttachment: [false]
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.loading = true;
    const formValue = this.form.value;

    // Convert booleans back to 1/0 for API
    const apiData = {
      ...formValue,
      isDeductible: formValue.isDeductible ? 1 : 0,
      requiresAttachment: formValue.requiresAttachment ? 1 : 0
    };

    if (this.isEditMode) {
      this.setupService.update('LeaveConfiguration/leave-types', apiData.leaveTypeId, apiData).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم التعديل بنجاح' });
          this.ref.close(true);
          this.loading = false;
        },
        error: () => this.loading = false
      });
    } else {
      this.setupService.create('LeaveConfiguration/leave-types', apiData).subscribe({
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
