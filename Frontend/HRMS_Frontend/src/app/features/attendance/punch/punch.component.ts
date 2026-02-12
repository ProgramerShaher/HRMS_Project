import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AttendanceService } from '../services/attendance.service';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { RegisterPunchCommand, TimesheetDayDto } from '../models/attendance.models';
import { AuthService } from '../../../core/auth/services/auth.service';

@Component({
  selector: 'app-punch',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonModule, CardModule],
  templateUrl: './punch.component.html',
  providers: [MessageService]
})
export class PunchComponent implements OnInit {
  private fb = inject(FormBuilder);
  private attendanceService = inject(AttendanceService);
  private messageService = inject(MessageService);
  private authService = inject(AuthService);

  loading = signal(false);
  currentTime = signal(new Date());

  // State
  todayPunch = signal<TimesheetDayDto | undefined>(undefined);
  alreadyPunchedIn = signal(false);
  alreadyPunchedOut = signal(false);

  constructor() {
    setInterval(() => {
      this.currentTime.set(new Date());
    }, 1000);
  }

  ngOnInit() {
    this.checkTodayStatus();
  }

  checkTodayStatus() {
    const user = this.authService.currentUser();
    if (!user || !user.employeeId) return;

    const today = new Date();
    const month = today.getMonth() + 1;
    const year = today.getFullYear();

    this.loading.set(true);
    this.attendanceService.getTimesheet(user.employeeId, month, year).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.succeeded) {
          const todayStr = today.toISOString().split('T')[0];
          const record = res.data.find(d => d.date.startsWith(todayStr));
          
          if (record) {
            this.todayPunch.set(record);
            if (record.inTime) this.alreadyPunchedIn.set(true);
            if (record.outTime) this.alreadyPunchedOut.set(true);
          }
        }
      },
      error: () => this.loading.set(false)
    });
  }

  punch(type: 'IN' | 'OUT') {
    if (this.loading()) return;

    this.loading.set(true);
    
    // Get Location
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          this.submitPunch(type, `${position.coords.latitude},${position.coords.longitude}`);
        },
        (error) => {
          console.error("Geolocation error", error);
          this.messageService.add({ severity: 'warn', summary: 'تنبيه موقع', detail: 'تعذر تحديد الموقع، سيتم التسجيل بدونه.' });
          this.submitPunch(type, undefined);
        }
      );
    } else {
      this.submitPunch(type, undefined);
    }
  }

  private submitPunch(type: 'IN' | 'OUT', coords?: string) {
    const user = this.authService.currentUser();
    const command: RegisterPunchCommand = {
      employeeId: user?.employeeId ?? 0, 
      punchType: type,
      punchTime: new Date().toISOString(),
      locationCoordinates: coords
    };

    this.attendanceService.punch(command).subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'تم بنجاح', detail: `تم تسجيل ${type === 'IN' ? 'الدخول' : 'الخروج'} بنجاح` });
        this.loading.set(false);
        // Refresh status
        this.checkTodayStatus();
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء تسجيل البصمة' });
        this.loading.set(false);
      }
    });

  }
}
