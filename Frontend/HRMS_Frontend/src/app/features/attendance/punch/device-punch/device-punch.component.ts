import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AttendanceService } from '../../services/attendance.service';
import { MessageService } from 'primeng/api';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { TagModule } from 'primeng/tag';
import { RegisterPunchCommand } from '../../models/attendance.models';
import { ToastModule } from 'primeng/toast';
import { EmployeeService } from '../../../personnel/services/employee.service';
import { TableModule } from 'primeng/table';

@Component({
  selector: 'app-device-punch',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    DialogModule, 
    ButtonModule, 
    InputTextModule, 
    TooltipModule, 
    TagModule,
    ToastModule,
    TableModule
  ],
  providers: [MessageService],
  templateUrl: './device-punch.component.html',
  styleUrls: ['./device-punch.component.scss']
})
export class DevicePunchComponent implements OnInit, OnDestroy {
  attendanceService = inject(AttendanceService);
  employeeService = inject(EmployeeService); // Using existing employee service
  messageService = inject(MessageService);

  currentTime = signal(new Date());
  timer: any;

  // State
  employees: any[] = []; // Will hold list of employees
  filteredEmployees: any[] = [];
  searchQuery = '';
  
  punchLoading = false;
  
  // Device Config
  readonly DEVICE_ID = 'KIOSK-001';

  ngOnInit() {
    this.startClock();
    this.loadEmployees();
  }

  ngOnDestroy() {
    if (this.timer) clearInterval(this.timer);
  }

  startClock() {
    this.timer = setInterval(() => {
      this.currentTime.set(new Date());
    }, 1000);
  }

  loadEmployees() {
    this.attendanceService.getDeviceEmployees().subscribe({
      next: (res: any) => {
        let data: any[] = [];
        if (Array.isArray(res)) data = res;
        else if (Array.isArray(res.data)) data = res.data;
        else if (res.data?.items) data = res.data.items;
        else if (res.items) data = res.items;

        // Map to display model ensuring proper field mapping matching our device DTO
        const mappedData = data.map((e: any) => ({
          ...e,
          id: e.employeeId, 
          fullNameAr: e.fullNameAr || e.employeeName || 'مجهول',
          currentShift: e.currentShift || 'بدون مناوبة',
          status: e.status || 'Out', 
          lastPunchIn: e.lastPunchIn ? new Date(e.lastPunchIn) : null,
          lastPunchOut: e.lastPunchOut ? new Date(e.lastPunchOut) : null,
          jobTitle: e.jobTitle || 'موظف', 
          photoUrl: null 
        }));

        setTimeout(() => {
            this.employees = mappedData;
            this.filteredEmployees = mappedData;
        });

        console.log('Device Punch: Loaded employees', mappedData.length);
      },
      error: (err: any) => console.error('Failed to load employees', err)
    });
  }

  filterEmployees() {
    if (!this.searchQuery) {
      this.filteredEmployees = this.employees;
      return;
    }
    const query = this.searchQuery.toLowerCase();
    this.filteredEmployees = this.employees.filter(e => 
      e.fullNameAr?.toLowerCase().includes(query) || 
      e.id?.toString().includes(query) ||
      e.employeeNumber?.toLowerCase().includes(query)
    );
  }

  quickPunch(emp: any, type: 'IN' | 'OUT') {
    this.punchLoading = true;
    const command: RegisterPunchCommand = {
      employeeId: emp.id,
      punchType: type,
      punchTime: new Date().toISOString(),
      deviceId: this.DEVICE_ID
    };

    this.attendanceService.punch(command).subscribe({
      next: (res: any) => {
        // Handle case where server returns 200 OK but result is a business failure
        if (res && res.succeeded === false) {
            this.messageService.add({
               severity: 'error', 
               summary: 'فشل التسجيل',
               detail: res.message || (res.errors && res.errors.length > 0 ? res.errors[0] : 'حدث خطأ غير معروف')
            });
            this.punchLoading = false;
            return;
        }

        const timeString = new Date().toLocaleTimeString('en-US', {hour: '2-digit', minute:'2-digit'});

        this.messageService.add({
          severity: 'success', 
          summary: type === 'IN' ? 'تم تسجيل الدخول' : 'تم تسجيل الخروج',
          detail: `الموظف: ${emp.fullNameAr} - الساعة: ${timeString}`
        });
        
        // Update mock status and local display
        emp.status = type === 'IN' ? 'In' : 'Out';
        if (type === 'IN') {
            emp.lastPunchIn = new Date();
        } else {
            emp.lastPunchOut = new Date();
        }
        this.punchLoading = false;
      },
      error: (err) => {
        // If the backend returns BadRequest(result.Errors), err.error is an array of strings
        let errorMsg = 'حدث خطأ أثناء الاتصال بالجهاز';
        if (Array.isArray(err.error) && err.error.length > 0) {
            errorMsg = err.error[0];
        } else if (err.error?.message) {
            errorMsg = err.error.message;
        } else if (typeof err.error === 'string') {
            errorMsg = err.error;
        }

        this.messageService.add({
          severity: 'error', 
          summary: 'فشل التسجيل',
          detail: errorMsg
        });
        this.punchLoading = false;
      }
    });
  }
}
