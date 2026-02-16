import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/chart';
import { ReportsService } from '../../core/services/reports.service';
import { ComprehensiveDashboardDto } from '../../core/models/reports.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ChartModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  data: any;
  options: any;

  weeklyTrendData: any;
  weeklyTrendOptions: any;
  dashboardData: ComprehensiveDashboardDto | null = null;
  loading = true;

  constructor(private reportsService: ReportsService) {}

  ngOnInit() {
      this.loadDashboardData();
      this.initChartOptions();
  }

  loadDashboardData() {
    this.loading = true;
    this.reportsService.getComprehensiveDashboard().subscribe({
      next: (response) => {
        // Backend returns Result<T> with 'succeeded' property
        if (response.succeeded) {
            this.dashboardData = response.data;
            this.initChartData();
        } else {
             console.error('API returned failure:', response.message);
        }
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load dashboard data', err);
        this.loading = false;
      }
    });
  }

  initChartData() {
      if (!this.dashboardData) return;
      
      const attendance = this.dashboardData.attendanceMetrics;
      
      const documentStyle = getComputedStyle(document.documentElement);
      const textColor = documentStyle.getPropertyValue('--text-color');
      const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
      const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

      this.data = {
          labels: ['حضور', 'غياب', 'إجازات', 'تأخير'],
          datasets: [
              {
                  label: 'إحصائيات اليوم',
                  data: [
                      attendance.totalPresent, 
                      attendance.totalAbsent, 
                      attendance.totalLeaves, 
                      attendance.totalLate
                  ],
                  backgroundColor: [
                      documentStyle.getPropertyValue('--blue-500'), 
                      documentStyle.getPropertyValue('--red-500'), 
                      documentStyle.getPropertyValue('--green-500'),
                      documentStyle.getPropertyValue('--yellow-500')
                  ],
                  hoverBackgroundColor: [
                      documentStyle.getPropertyValue('--blue-400'), 
                      documentStyle.getPropertyValue('--red-400'), 
                      documentStyle.getPropertyValue('--green-400'),
                      documentStyle.getPropertyValue('--yellow-400')
                  ]
              }
          ]
      };

      // Weekly Trend Chart
      if (this.dashboardData.weeklyMetrics?.attendanceTrend) {
        const trend = this.dashboardData.weeklyMetrics.attendanceTrend;
        // Format dates as abbreviated day names (e.g., Sat, Sun)
        const labels = trend.map((t: any) => new Date(t.date).toLocaleDateString('ar-SA', { weekday: 'long' }));
        const presentData = trend.map((t: any) => t.presentCount);
        const lateData = trend.map((t: any) => t.lateCount);

        this.weeklyTrendData = {
            labels: labels,
            datasets: [
                {
                    label: 'حضور',
                    data: presentData,
                    fill: true,
                    borderColor: documentStyle.getPropertyValue('--emerald-500'),
                    backgroundColor: 'rgba(16, 185, 129, 0.1)',
                    tension: 0.4
                },
                {
                    label: 'تأخير',
                    data: lateData,
                    fill: true,
                    borderColor: documentStyle.getPropertyValue('--amber-500'),
                    backgroundColor: 'rgba(245, 158, 11, 0.1)',
                    tension: 0.4
                }
            ]
        };

        this.weeklyTrendOptions = {
            maintainAspectRatio: false,
            aspectRatio: 0.8,
            plugins: {
                legend: {
                    labels: { color: textColor, font: { family: 'Cairo' } }
                }
            },
            scales: {
                x: {
                    ticks: { color: textColorSecondary, font: { family: 'Cairo' } },
                    grid: { color: surfaceBorder, drawBorder: false }
                },
                y: {
                    ticks: { color: textColorSecondary },
                    grid: { color: surfaceBorder, drawBorder: false }
                }
            }
        };
      }
  }

  initChartOptions() {
      const documentStyle = getComputedStyle(document.documentElement);
      const textColor = documentStyle.getPropertyValue('--text-color');
      const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
      const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

      this.options = {
          plugins: {
              legend: {
                  labels: {
                      usePointStyle: true,
                      color: textColor
                  }
              }
          }
      };
  }
}
