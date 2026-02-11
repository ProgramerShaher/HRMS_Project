import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AttendanceService } from '../services/attendance.service';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { RegisterPunchCommand } from '../models/attendance.models';

@Component({
  selector: 'app-punch',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonModule, CardModule],
  templateUrl: './punch.component.html',
  providers: [MessageService]
})
export class PunchComponent {
  private fb = inject(FormBuilder);
  private attendanceService = inject(AttendanceService);
  private messageService = inject(MessageService);

  loading = signal(false);
  currentTime = signal(new Date());

  constructor() {
    setInterval(() => {
      this.currentTime.set(new Date());
    }, 1000);
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
    const command: RegisterPunchCommand = {
      employeeId: 0, // Backend should handle this or get from AuthState
      punchType: type,
      punchTime: new Date().toISOString(),
      locationCoordinates: coords
    };

    this.attendanceService.punch(command).subscribe({
      next: (res) => {
        this.messageService.add({ severity: 'success', summary: 'تم بنجاح', detail: `تم تسجيل ${type === 'IN' ? 'الدخول' : 'الخروج'} بنجاح` });
        this.loading.set(false);
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء تسجيل البصمة' });
        this.loading.set(false);
      }
    });

  }
}
