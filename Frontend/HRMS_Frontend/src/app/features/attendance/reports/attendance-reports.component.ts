import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AttendanceService } from '../services/attendance.service';
import { PayrollAttendanceSummaryDto } from '../models/attendance.models';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { FormsModule } from '@angular/forms';
import { SelectModule } from 'primeng/select';

@Component({
  selector: 'app-attendance-reports',
  standalone: true,
  imports: [CommonModule, TableModule, CardModule, ButtonModule, DatePickerModule, FormsModule, SelectModule],
  templateUrl: './attendance-reports.component.html'
})
export class AttendanceReportsComponent implements OnInit {
  attendanceService = inject(AttendanceService);
  
  reportData = signal<PayrollAttendanceSummaryDto[]>([]);
  loading = signal(false);
  
  // Filters
  selectedDate = new Date(); // Default to current date for month/year extraction
  
  ngOnInit() {
    this.generateReport();
  }

  generateReport() {
    const month = this.selectedDate.getMonth() + 1;
    const year = this.selectedDate.getFullYear();
    
    this.loading.set(true);
    this.attendanceService.getPayrollSummary(month, year).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.reportData.set(res.data);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
