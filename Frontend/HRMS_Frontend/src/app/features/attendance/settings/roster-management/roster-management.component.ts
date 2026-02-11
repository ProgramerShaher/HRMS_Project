import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { AttendanceSettingsService } from '../../services/attendance-settings.service';
import { ShiftTypeDto, MyRosterDto } from '../../models/attendance.models';
import { MessageService } from 'primeng/api';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { CardModule } from 'primeng/card';
import { InputNumberModule } from 'primeng/inputnumber';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { CheckboxModule } from 'primeng/checkbox';

@Component({
  selector: 'app-roster-management',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    FormsModule,
    SelectModule, 
    DatePickerModule, 
    ButtonModule, 
    ToastModule, 
    CardModule, 
    InputNumberModule,
    TableModule,
    DialogModule,
    CheckboxModule
  ],
  providers: [MessageService],
  templateUrl: './roster-management.component.html'
})
export class RosterManagementComponent implements OnInit {
  private fb = inject(FormBuilder);
  private settingsService = inject(AttendanceSettingsService);
  private messageService = inject(MessageService);

  // Forms
  initForm!: FormGroup;
  editDayForm!: FormGroup;

  // State
  shifts = signal<ShiftTypeDto[]>([]);
  employeeRoster = signal<MyRosterDto[]>([]);
  selectedEmployeeId = signal<number | null>(null);
  
  // UI Controls
  editDialogVisible = false;
  editingDate: Date | null = null;

  ngOnInit() {
    this.createForms();
    this.loadShifts();
  }

  createForms() {
    this.initForm = this.fb.group({
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      shiftId: [null, Validators.required],
      employeeId: [null] 
    });

    this.editDayForm = this.fb.group({
       shiftId: [null],
       isOffDay: [false]
    });
  }

  loadShifts() {
    this.settingsService.getAllShifts().subscribe(data => this.shifts.set(data));
  }

  onInitialize() {
    if (this.initForm.invalid) return;
    const val = this.initForm.value;
    const cmd = {
      startDate: val.startDate.toISOString(),
      endDate: val.endDate.toISOString(),
      shiftId: val.shiftId,
      employeeId: val.employeeId || undefined
    };

    this.settingsService.initializeRoster(cmd).subscribe({
      next: () => this.messageService.add({severity:'success', summary: 'تم', detail: 'تم تهيئة الجدول بنجاح'}),
      error: () => this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل تهيئة الجدول'})
    });
  }

  // --- Manual Editor Logic ---

  fetchEmployeeRoster() {
      if (!this.selectedEmployeeId()) return;
      this.settingsService.getEmployeeRoster(this.selectedEmployeeId()!).subscribe({
          next: (res) => {
              if (res.succeeded) {
                  this.employeeRoster.set(res.data);
                  this.messageService.add({severity:'success', summary: 'تم', detail: 'تم تحميل الجدول'});
              } else {
                  this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل تحميل الجدول'});
              }
          },
          error: () => this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل الاتصال'})
      });
  }

  openEditDialog(day: MyRosterDto) {
      this.editingDate = new Date(day.date);
      // Find shift ID from name is tricky unless we have ID in DTO. 
      // HRMS.Application\Features\Attendance\Roster\Queries\GetMyRoster\GetMyRosterQuery.cs DTO doesn't have ShiftId.
      // Assuming user selects new shift.
      
      this.editDayForm.patchValue({
          shiftId: null, // Reset or find if DTO is updated (I didn't update DTO)
          isOffDay: day.isOffDay
      });
      this.editDialogVisible = true;
  }

  saveDayEdit() {
      if (!this.selectedEmployeeId() || !this.editingDate) return;
      const val = this.editDayForm.value;

      if (!val.isOffDay && !val.shiftId) {
          this.messageService.add({severity:'warn', summary: 'تنبيه', detail: 'اختر المناوبة أو حدد إجازة'});
          return;
      }

      const cmd = {
          employeeId: this.selectedEmployeeId()!,
          date: this.editingDate.toISOString(),
          shiftId: val.isOffDay ? null : val.shiftId,
          isOffDay: val.isOffDay
      };

      this.settingsService.updateRosterDay(cmd).subscribe({
          next: (res) => {
              if (res.succeeded) {
                  this.messageService.add({severity:'success', summary: 'تم', detail: 'تم تحديث المناوبة'});
                  this.editDialogVisible = false;
                  this.fetchEmployeeRoster(); // Refresh
              } else {
                  this.messageService.add({severity:'error', summary: 'خطأ', detail: res.errors[0] || 'فشل التحديث'});
              }
          }
      });
  }

  getSeverity(isOffDay: boolean): 'success' | 'info' | 'warning' | 'danger' | 'secondary' | 'contrast' | undefined {
    return isOffDay ? 'warning' : 'success';
  }
}
