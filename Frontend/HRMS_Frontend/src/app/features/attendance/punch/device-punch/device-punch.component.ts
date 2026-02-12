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
    // Using getAll to fetch employees. Fetching first 50 for simulation.
    this.employeeService.getAll(1, 1000).subscribe({
      next: (res: any) => {
        // Handle both Result wrapper or direct array depending on service
        let data: any[] = [];
        if (Array.isArray(res)) data = res;
        else if (Array.isArray(res.data)) data = res.data;
        else if (res.data?.items) data = res.data.items;
        else if (res.items) data = res.items;

        // Map to display model
        this.employees = data.map((e: any) => ({
          ...e,
          id: e.employeeId, 
          fullNameAr: e.fullNameAr || e.firstNameAr + ' ' + e.lastNameAr || 'مجهول',
          currentShift: 'صباحي (8:00 - 16:00)', // Mock shift for now until we join Roster
          status: e.lastPunchOut ? 'Out' : (e.lastPunchIn ? 'In' : 'Out'), 
          lastPunchIn: e.lastPunchIn,
          lastPunchOut: e.lastPunchOut,
          jobTitle: e.jobTitle || 'موظف', 
          photoUrl: null 
        }));
        this.filteredEmployees = this.employees;
        console.log('Device Punch: Loaded employees', this.employees.length);
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
    // 1. Check-Out Validation (Early Exit)
    if (type === 'OUT') {
        if (!emp.lastPunchIn) {
            this.messageService.add({severity: 'error', summary: 'خطأ', detail: 'يجب تسجيل الدخول أولاً'});
            return;
        }
        
        // Parse Mock Shift End Time (16:00)
        // Shift format: "صباحي (8:00 - 16:00)"
        const shiftTimes = emp.currentShift.match(/(\d{1,2}:\d{2})/g); // ["8:00", "16:00"]
        if (shiftTimes && shiftTimes.length === 2) {
            const endTimeStr = shiftTimes[1]; // "16:00"
            const [endHour, endMinute] = endTimeStr.split(':').map(Number);
            
            const now = new Date();
            const shiftEnd = new Date();
            shiftEnd.setHours(endHour, endMinute, 0);

            // strict validation: Refuse if now < shiftEnd
            // For testing/demo purposes, we might need a workaround, but user asked to REFUSE.
            /* 
               Uncomment strictly if you want to block testing. 
               For simulation, maybe we block if > 1 hour early? 
               Let's strictly follow user request: "Refuse".
            */
            if (now < shiftEnd) {
                 this.messageService.add({
                    severity: 'warn', 
                    summary: 'تنبيه - خروج مبكر', 
                    detail: `لا يمكن تسجيل الانصراف قبل انتهاء الدوام (${endTimeStr})`
                });
                return;
            }
        }
    }

    // 2. Check-In Validation (Repeated)
    if (type === 'IN' && emp.status === 'In') {
         this.messageService.add({severity: 'warn', summary: 'تنبيه', detail: 'الموظف مسجل دخول بالفعل'});
         return;
    }

    this.punchLoading = true;
    const command: RegisterPunchCommand = {
      employeeId: emp.id,
      punchType: type,
      punchTime: new Date().toISOString(),
      deviceId: this.DEVICE_ID
    };

    this.attendanceService.punch(command).subscribe({
      next: () => {
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
        this.messageService.add({
          severity: 'error', 
          summary: 'فشل التسجيل',
          detail: err.error?.message || 'حدث خطأ أثناء الاتصال بالجهاز'
        });
        this.punchLoading = false;
      }
    });
  }
}
