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

  getSeverity(isOffDay: boolean): 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast' | undefined {
    return isOffDay ? 'warn' : 'success';
  }
}
