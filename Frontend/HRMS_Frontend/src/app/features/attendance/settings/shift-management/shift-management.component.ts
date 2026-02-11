import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AttendanceSettingsService } from '../../services/attendance-settings.service';
import { ShiftTypeDto, CreateShiftTypeCommand } from '../../models/attendance.models';
import { MessageService, ConfirmationService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@Component({
  selector: 'app-shift-management',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    TableModule, 
    DialogModule, 
    ButtonModule, 
    InputTextModule, 
    CheckboxModule, 
    ToastModule, 
    ConfirmDialogModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './shift-management.component.html'
})
export class ShiftManagementComponent implements OnInit {
  private attendanceSettingsService = inject(AttendanceSettingsService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  shifts = signal<ShiftTypeDto[]>([]);
  shiftDialog = signal(false);
  shiftForm!: FormGroup;
  isEditMode = signal(false);
  currentShiftId = signal<number | null>(null);

  ngOnInit() {
    this.initForm();
    this.loadShifts();
  }

  initForm() {
    this.shiftForm = this.fb.group({
      shiftNameAr: ['', [Validators.required]],
      startTime: ['', [Validators.required, Validators.pattern(/^([01][0-9]|2[0-3]):[0-5][0-9]$/)]],
      endTime: ['', [Validators.required, Validators.pattern(/^([01][0-9]|2[0-3]):[0-5][0-9]$/)]],
      isCrossDay: [false]
    });
  }

  loadShifts() {
    this.attendanceSettingsService.getAllShifts().subscribe({
      next: (data) => this.shifts.set(data),
      error: () => this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل المناوبات' })
    });
  }

  openNew() {
    this.shiftForm.reset({ isCrossDay: false });
    this.isEditMode.set(false);
    this.shiftDialog.set(true);
  }

  editShift(shift: ShiftTypeDto) {
    this.shiftForm.patchValue({
        shiftNameAr: shift.shiftNameAr,
        startTime: shift.startTime,
        endTime: shift.endTime,
        isCrossDay: shift.isCrossDay === 1
    });
    this.currentShiftId.set(shift.shiftId);
    this.isEditMode.set(true);
    this.shiftDialog.set(true);
  }

  deleteShift(shift: ShiftTypeDto) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف هذه المناوبة؟',
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.attendanceSettingsService.deleteShift(shift.shiftId).subscribe({
          next: () => {
             this.messageService.add({severity:'success', summary: 'نجاح', detail: 'تم الحذف بنجاح'});
             this.loadShifts();
          },
          error: () => this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل الحذف'})
        });
      }
    });
  }

  saveShift() {
    if (this.shiftForm.invalid) {
      this.shiftForm.markAllAsTouched();
      return;
    }

    const formValue = this.shiftForm.value;
    const command: CreateShiftTypeCommand = {
      shiftNameAr: formValue.shiftNameAr,
      startTime: formValue.startTime,
      endTime: formValue.endTime,
      isCrossDay: formValue.isCrossDay ? 1 : 0
    };

    if (this.isEditMode()) {
        this.attendanceSettingsService.updateShift(this.currentShiftId()!, command).subscribe({
            next: () => {
                this.messageService.add({severity:'success', summary: 'نجاح', detail: 'تم التعديل بنجاح'});
                this.shiftDialog.set(false);
                this.loadShifts();
            },
            error: () => this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل التعديل'})
        });
    } else {
        this.attendanceSettingsService.createShift(command).subscribe({
            next: () => {
                this.messageService.add({severity:'success', summary: 'نجاح', detail: 'تمت الإضافة بنجاح'});
                this.shiftDialog.set(false);
                this.loadShifts();
            },
            error: () => this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل الإضافة'})
        });
    }
  }

  hideDialog() {
    this.shiftDialog.set(false);
  }
}
