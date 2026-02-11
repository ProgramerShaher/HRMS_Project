import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AttendanceService } from '../services/attendance.service';
import { MessageService } from 'primeng/api';
import { TabsModule } from 'primeng/tabs';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { ButtonModule } from 'primeng/button';
import { Result } from '../models/attendance.models';
import { InputNumberModule } from 'primeng/inputnumber';
import { Toast } from "primeng/toast";

@Component({
  selector: 'app-requests',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TabsModule,
    SelectModule,
    DatePickerModule,
    InputTextModule,
    TextareaModule,
    ButtonModule,
    InputNumberModule,
    Toast
],
  providers: [MessageService],
  templateUrl: './requests.component.html'
})
export class RequestsComponent implements OnInit {
  private fb = inject(FormBuilder);
  private attendanceService = inject(AttendanceService);
  private messageService = inject(MessageService);

  // Forms
  swapForm!: FormGroup;
  overtimeForm!: FormGroup;
  permissionForm!: FormGroup;
  correctionForm!: FormGroup;

  // Data Sources (Mock or Real)
  employees: any[] = []; // In real app, fetch colleague list
  permissionTypes = [
     { label: 'شخصي', value: 'Personal' }, 
     { label: 'مرضي', value: 'Sick' }, 
     { label: 'رسمي', value: 'Official' }
  ];
  correctionTypes = [
     { label: 'نسيان بصمة دخول', value: 'MissedIn' },
     { label: 'نسيان بصمة خروج', value: 'MissedOut' }
  ];

  ngOnInit() {
    this.initForms();
    // Fetch Employees List for Swap Target if API available
    // this.personnelService.getEmployees()...
    // For now, using input for ID or simplified
  }

  initForms() {
    // Swap Request
    this.swapForm = this.fb.group({
      targetEmployeeId: [null, [Validators.required]],
      rosterDate: [null, [Validators.required]],
      reason: ['', [Validators.required]]
    });

    // Overtime Request
    this.overtimeForm = this.fb.group({
      workDate: [null, [Validators.required]],
      hoursRequested: [null, [Validators.required, Validators.min(1)]],
      reason: ['', [Validators.required]]
    });

    // Permission Request
    this.permissionForm = this.fb.group({
      permissionDate: [null, [Validators.required]],
      permissionType: [null, [Validators.required]],
      hours: [null, [Validators.required, Validators.min(0.5)]],
      reason: ['', [Validators.required]]
    });

    // Correction Request
    this.correctionForm = this.fb.group({
      dailyAttendanceId: [null], // This might need a dropdown of recent attendance records
      attendanceDate: [null, [Validators.required]], // Helper to find ID
      correctionType: [null, [Validators.required]],
      newValue: ['', [Validators.required]],
      auditNote: ['', [Validators.required]]
    });
  }

  // Submit Handlers
  submitSwap() {
    if (this.swapForm.invalid) return;
    const val = this.swapForm.value;
    const cmd = {
      requesterId: 0, // Backend contextual
      targetEmployeeId: val.targetEmployeeId,
      rosterDate: val.rosterDate.toISOString(),
      reason: val.reason
    };
    
    // In real implementation, call service.applySwap
    this.attendanceService.applySwap(cmd).subscribe({
        next: () => {
             this.messageService.add({severity:'success', summary: 'تم', detail: 'تم إرسال طلب التبديل بنجاح'});
             this.swapForm.reset();
        },
        error: () => this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل إرسال طلب التبديل'})
    });
  }

  submitOvertime() {
    if (this.overtimeForm.invalid) return;
    const val = this.overtimeForm.value;
    const cmd = {
      employeeId: 0,
      workDate: val.workDate.toISOString(),
      hoursRequested: val.hoursRequested,
      reason: val.reason
    };
    
    this.attendanceService.applyOvertime(cmd).subscribe({
        next: () => {
             this.messageService.add({severity:'success', summary: 'تم', detail: 'تم إرسال طلب العمل الإضافي'});
             this.overtimeForm.reset();
        },
        error: () => this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل إرسال طلب العمل الإضافي'})
    });
  }

  submitPermission() {
    if (this.permissionForm.invalid) return;
    const val = this.permissionForm.value;
    const cmd = {
        employeeId: 0,
        permissionDate: val.permissionDate.toISOString(),
        permissionType: val.permissionType,
        hours: val.hours,
        reason: val.reason
    };
    this.attendanceService.applyPermission(cmd).subscribe({
        next: () => {
            this.messageService.add({severity:'success', summary: 'تم', detail: 'تم إرسال طلب الإذن'});
            this.permissionForm.reset();
        },
        error: (err) => this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل إرسال الطلب'})
    });
  }

  submitCorrection() {
      // Logic to get ID from Date would go here
      this.messageService.add({severity:'warn', summary: 'تنبيه', detail: 'ميزة التصحيح تتطلب تحديد سجل الحضور أولاً'});
  }
}
