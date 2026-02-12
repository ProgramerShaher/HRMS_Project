import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AttendanceService } from '../services/attendance.service';
import { MyRosterDto } from '../models/attendance.models';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-my-roster',
  standalone: true,
  imports: [CommonModule, TableModule, TagModule, CardModule],
  templateUrl: './my-roster.component.html'
})
export class MyRosterComponent implements OnInit {
  private attendanceService = inject(AttendanceService);
  
  roster = signal<MyRosterDto[]>([]);
  loading = signal(true);

  ngOnInit() {
    this.loadRoster();
  }

  loadRoster() {
    this.loading.set(true);
    this.attendanceService.getMyRoster().subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.roster.set(res.data);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  getSeverity(status: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast' | undefined {
    switch (status?.toUpperCase()) {
      case 'PRESENT': return 'success';
      case 'ABSENT': return 'danger';
      case 'LEAVE': return 'info';
      case 'OFF': return 'warn';
      case 'HOLIDAY': return 'info';
      case 'MISSING_PUNCH': return 'danger';
      default: return 'secondary';
    }
  }

  getStatusLabel(item: MyRosterDto): string {
    if (item.isOffDay) return 'إجازة';
    switch (item.status?.toUpperCase()) {
      case 'PRESENT': return 'حاضر';
      case 'ABSENT': return 'غائب';
      case 'LEAVE': return 'في إجازة';
      case 'OFF': return 'يوم راحة';
      case 'HOLIDAY': return 'عطلة';
      case 'MISSING_PUNCH': return 'نقص بصمة';
      default: return 'مجدول';
    }
  }
}
