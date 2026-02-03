import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputMaskModule } from 'primeng/inputmask';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { SetupService } from '../../services/setup.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-shift-type-form',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    ButtonModule, 
    InputTextModule, 
    CheckboxModule, 
    InputNumberModule,
    InputMaskModule
  ],
  templateUrl: './shift-type-form.component.html',
  styleUrls: ['./shift-type-form.component.scss']
})
export class ShiftTypeFormComponent implements OnInit {
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
    if (this.config.data && this.config.data.shiftId) {
      this.isEditMode = true;
      const patchData = {
        ...this.config.data,
        isCrossDay: this.config.data.isCrossDay === 1
      };
      this.form.patchValue(patchData);
    }
  }

  initForm() {
    this.form = this.fb.group({
      shiftId: [null],
      shiftNameAr: ['', [Validators.required]],
      startTime: ['', [Validators.required]], // Mask 99:99
      endTime: ['', [Validators.required]],   // Mask 99:99
      hoursCount: [8, [Validators.required, Validators.min(0)]],
      isCrossDay: [false]
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.loading = true;
    const formValue = this.form.value;

    const apiData = {
      ...formValue,
      isCrossDay: formValue.isCrossDay ? 1 : 0
    };

    if (this.isEditMode) {
      this.setupService.update('AttendanceSettings/shifts', apiData.shiftId, apiData).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم التعديل بنجاح' });
          this.ref.close(true);
          this.loading = false;
        },
        error: () => this.loading = false
      });
    } else {
      this.setupService.create('AttendanceSettings/shifts', apiData).subscribe({
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
