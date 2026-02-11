import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AttendanceService } from '../services/attendance.service';
import { LiveStatusDto, AttendanceExceptionDto } from '../models/attendance.models';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-attendance-dashboard',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, TooltipModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  private attendanceService = inject(AttendanceService);

  liveStatus = signal<LiveStatusDto | null>(null);
  exceptions = signal<AttendanceExceptionDto[]>([]);

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.attendanceService.getLiveStatus().subscribe(res => {
      if (res.succeeded) {
        this.liveStatus.set(res.data);
      }
    });

    this.attendanceService.getExceptions().subscribe(res => {
      if (res.succeeded) {
        this.exceptions.set(res.data);
      }
    });
  }
}
