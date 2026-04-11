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
import { InputNumberModule } from 'primeng/inputnumber';
import { Toast } from "primeng/toast";
import { EmployeeService } from '../../personnel/services/employee.service';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';

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
    Toast,
    TableModule,
    TagModule,
    DialogModule
],
  providers: [MessageService],
  templateUrl: './requests.component.html'
})
export class RequestsComponent implements OnInit {
  private fb = inject(FormBuilder);
  private attendanceService = inject(AttendanceService);
  private personnelService = inject(EmployeeService);
  private messageService = inject(MessageService);

  // Forms
  swapForm!: FormGroup;
  overtimeForm!: FormGroup;
  permissionForm!: FormGroup;
  correctionForm!: FormGroup;

  // Data Sources (Mock or Real)
  employees: any[] = []; // In real app, fetch colleague list
  permissionTypes = [
     { label: 'تأخير دخول (استئذان صباحي)', value: 'LateEntry' }, 
     { label: 'خروج مبكر (استئذان مسائي)', value: 'EarlyExit' }
  ];
  correctionTypes = [
     { label: 'نسيان بصمة دخول', value: 'MissedIn' },
     { label: 'نسيان بصمة خروج', value: 'MissedOut' }
  ];

  // Dialog States
  showSwapDialog = false;
  showOvertimeDialog = false;
  showPermissionDialog = false;
  showCorrectionDialog = false;

  // History Tables
  myPermissions: any[] = [];
  myOvertimes: any[] = [];
  mySwaps: any[] = [];
  
  loadingTables = false;

  ngOnInit() {
    this.initForms();
    this.loadEmployees();
    this.loadHistory();
  }

  loadEmployees() {
    this.personnelService.getAll(1, 100).subscribe({
      next: (res) => {
        this.employees = res.items || res.data || res;
      },
      error: () => console.error('Failed to load employees')
    });
  }

  loadHistory() {
      this.loadingTables = true;
      // In a real ERP with authentication, these endpoints return history for the logged-in user.
      // Or if HR Officer, they fetch pending approvals. We simulate by fetching the history endpoints.
      this.attendanceService.getMyPermissions().subscribe({
          next: (res: any) => this.myPermissions = res.data || res,
          error: () => console.error('Failed to load permissions')
      });
      
      this.attendanceService.getMyOvertimeRequests().subscribe({
          next: (res: any) => this.myOvertimes = res.data || res,
          error: () => console.error('Failed to load overtime')
      });

      this.attendanceService.getMySwapRequests().subscribe({
          next: (res: any) => {
              this.mySwaps = res.data || res;
              this.loadingTables = false;
          },
          error: () => this.loadingTables = false
      });
  }

  initForms() {
    this.swapForm = this.fb.group({
      requesterId: [null, Validators.required],
      targetEmployeeId: [null, Validators.required],
      rosterDate: [null, Validators.required],
      reason: ['', Validators.required]
    });

    this.overtimeForm = this.fb.group({
      employeeId: [null, Validators.required],
      workDate: [null, Validators.required],
      hoursRequested: [null, [Validators.required, Validators.min(1)]],
      reason: ['', Validators.required]
    });

    this.permissionForm = this.fb.group({
      employeeId: [null, Validators.required],
      permissionDate: [null, Validators.required],
      permissionType: [null, Validators.required],
      hours: [null, [Validators.required, Validators.min(0.5)]],
      reason: ['', Validators.required]
    });

    this.correctionForm = this.fb.group({
      employeeId: [null, Validators.required],
      dailyAttendanceId: [null], 
      attendanceDate: [null, Validators.required],
      correctionType: [null, Validators.required],
      newValue: ['', Validators.required],
      auditNote: ['', Validators.required]
    });
  }

  // Utils
  toLocalISOString(date: Date): string {
      const tzOffset = date.getTimezoneOffset() * 60000; // offset in milliseconds
      const localISOTime = (new Date(date.getTime() - tzOffset)).toISOString().slice(0, -1);
      return localISOTime;
  }

  // Submit Handlers
  submitSwap() {
    if (this.swapForm.invalid) return;
    const val = this.swapForm.value;
    const cmd = {
      requesterId: val.requesterId,
      targetEmployeeId: val.targetEmployeeId,
      rosterDate: this.toLocalISOString(val.rosterDate),
      reason: val.reason
    };
    
    this.attendanceService.applySwap(cmd).subscribe({
        next: () => {
             this.messageService.add({severity:'success', summary: 'تم', detail: 'تم إرسال طلب التبديل بنجاح'});
             this.showSwapDialog = false;
             this.swapForm.reset();
             this.loadHistory();
        },
        error: (err) => this.messageService.add({severity:'error', summary: 'خطأ', detail: err.error?.message || 'فشل إرسال طلب التبديل'})
    });
  }

  submitOvertime() {
    if (this.overtimeForm.invalid) return;
    const val = this.overtimeForm.value;
    const cmd = {
      employeeId: val.employeeId,
      workDate: this.toLocalISOString(val.workDate),
      hoursRequested: val.hoursRequested,
      reason: val.reason
    };
    
    this.attendanceService.applyOvertime(cmd).subscribe({
        next: () => {
             this.messageService.add({severity:'success', summary: 'تم', detail: 'تم إرسال طلب العمل الإضافي'});
             this.showOvertimeDialog = false;
             this.overtimeForm.reset();
             this.loadHistory();
        },
        error: (err) => this.messageService.add({severity:'error', summary: 'خطأ', detail: err.error?.message || 'فشل إرسال طلب العمل الإضافي'})
    });
  }

  submitPermission() {
    if (this.permissionForm.invalid) return;
    const val = this.permissionForm.value;
    const cmd = {
        employeeId: val.employeeId,
        permissionDate: this.toLocalISOString(val.permissionDate),
        permissionType: val.permissionType,
        hours: val.hours,
        reason: val.reason
    };
    this.attendanceService.applyPermission(cmd).subscribe({
        next: () => {
            this.messageService.add({severity:'success', summary: 'تم', detail: 'تم إرسال طلب الإذن'});
            this.showPermissionDialog = false;
            this.permissionForm.reset();
            this.loadHistory();
        },
        error: (err) => this.messageService.add({severity:'error', summary: 'خطأ', detail: err.error?.message || 'فشل إرسال الطلب'})
    });
  }

  submitCorrection() {
      if (this.correctionForm.invalid) return;
      const val = this.correctionForm.value;
      const cmd = {
          employeeId: val.employeeId,
          attendanceDate: this.toLocalISOString(val.attendanceDate),
          dailyAttendanceId: val.dailyAttendanceId,
          correctionType: val.correctionType,
          newValue: val.newValue,
          auditNote: val.auditNote
      };
      
      this.attendanceService.manualCorrection(cmd as any).subscribe({
          next: () => {
              this.messageService.add({severity:'success', summary: 'تم', detail: 'تم إرسال طلب التصحيح'});
              this.showCorrectionDialog = false;
              this.correctionForm.reset();
          },
          error: (err) => this.messageService.add({severity:'error', summary: 'خطأ', detail: err.error?.message || 'فشل إرسال طلب التصحيح'})
      });
  }
}
