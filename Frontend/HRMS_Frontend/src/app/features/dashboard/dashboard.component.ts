import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/chart';
import { RouterModule } from '@angular/router';
import { ReportsService } from '../../core/services/reports.service';
import { ComprehensiveDashboardDto } from '../../core/models/reports.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ChartModule, RouterModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  weeklyTrendData: any;
  weeklyTrendOptions: any;
  
  payrollChartData: any;
  payrollChartOptions: any;

  dashboardData: ComprehensiveDashboardDto | null = null;
  loading = true;

  constructor(private reportsService: ReportsService) {}

  ngOnInit() {
      this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading = true;
    this.reportsService.getComprehensiveDashboard().subscribe({
      next: (response) => {
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
      
      const documentStyle = getComputedStyle(document.documentElement);
      const isDarkMode = document.documentElement.classList.contains('dark');
      
      const textColor = isDarkMode ? '#e2e8f0' : '#475569';
      const textColorSecondary = isDarkMode ? '#94a3b8' : '#64748b';
      const surfaceBorder = isDarkMode ? '#334155' : '#e2e8f0';

      // Weekly Trend Chart with Gradients
      if (this.dashboardData.weeklyMetrics?.attendanceTrend) {
        const trend = this.dashboardData.weeklyMetrics.attendanceTrend;
        const labels = trend.map((t: any) => new Date(t.date).toLocaleDateString('ar-SA', { weekday: 'long' }));
        const presentData = trend.map((t: any) => t.presentCount);
        const lateData = trend.map((t: any) => t.lateCount);

        this.weeklyTrendData = {
            labels: labels,
            datasets: [
                {
                    label: 'الحضور والانضباط',
                    data: presentData,
                    fill: {
                        target: 'origin',
                        above: 'rgba(79, 70, 229, 0.1)' // indigo-600 with opacity
                    },
                    borderColor: '#4f46e5',
                    borderWidth: 4,
                    pointBackgroundColor: '#4f46e5',
                    pointBorderColor: '#fff',
                    pointHoverBackgroundColor: '#fff',
                    pointHoverBorderColor: '#4f46e5',
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    tension: 0.4
                },
                {
                    label: 'حالات التأخير',
                    data: lateData,
                    fill: false,
                    borderColor: '#f59e0b',
                    borderWidth: 2,
                    borderDash: [5, 5],
                    pointBackgroundColor: '#f59e0b',
                    pointRadius: 0,
                    pointHoverRadius: 4,
                    tension: 0.4
                }
            ]
        };

        this.weeklyTrendOptions = {
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'top',
                    align: 'end',
                    labels: {
                        color: textColor,
                        font: { family: 'Cairo', size: 12, weight: 'bold' },
                        usePointStyle: true,
                        pointStyle: 'circle'
                    }
                },
                tooltip: {
                    backgroundColor: isDarkMode ? '#1e293b' : '#ffffff',
                    titleColor: isDarkMode ? '#ffffff' : '#0f172a',
                    bodyColor: isDarkMode ? '#e2e8f0' : '#475569',
                    borderColor: surfaceBorder,
                    borderWidth: 1,
                    padding: 12,
                    boxPadding: 4,
                    usePointStyle: true,
                    titleFont: { family: 'Cairo', size: 14, weight: 'bold' },
                    bodyFont: { family: 'Cairo', size: 13 }
                }
            },
            scales: {
                x: {
                    ticks: { color: textColorSecondary, font: { family: 'Cairo', size: 11 } },
                    grid: { display: false }
                },
                y: {
                    beginAtZero: true,
                    ticks: { color: textColorSecondary, font: { family: 'Cairo', size: 11 } },
                    grid: { color: surfaceBorder, drawBorder: false, borderDash: [4, 4] }
                }
            }
        };
      }

      // Payroll Distribution Chart (Horizontal Bar)
      if (this.dashboardData.monthlyMetrics?.salaryByDepartment) {
        const payroll = this.dashboardData.monthlyMetrics.salaryByDepartment;
        const labels = Object.keys(payroll);
        const values = Object.values(payroll);

        this.payrollChartData = {
            labels: labels,
            datasets: [
                {
                    label: 'كلفة القسم بريال',
                    data: values,
                    backgroundColor: (context: any) => {
                        const chart = context.chart;
                        const {ctx, chartArea} = chart;
                        if (!chartArea) return null;
                        const gradient = ctx.createLinearGradient(0, chartArea.bottom, 0, chartArea.top);
                        gradient.addColorStop(0, '#4f46e5'); // indigo-600
                        gradient.addColorStop(1, '#818cf8'); // indigo-400
                        return gradient;
                    },
                    hoverBackgroundColor: '#4338ca',
                    borderRadius: 12,
                    barThickness: 24,
                    maxBarThickness: 32
                }
            ]
        };

        this.payrollChartOptions = {
            indexAxis: 'y',
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: isDarkMode ? '#1e293b' : '#ffffff',
                    titleColor: isDarkMode ? '#ffffff' : '#0f172a',
                    bodyColor: isDarkMode ? '#e2e8f0' : '#475569',
                    borderColor: surfaceBorder,
                    borderWidth: 1,
                    padding: 12,
                    callbacks: {
                        label: (context: any) => ` ${context.raw.toLocaleString()} ريال`
                    },
                    titleFont: { family: 'Cairo', size: 14, weight: 'bold' },
                    bodyFont: { family: 'Cairo', size: 13 }
                }
            },
            scales: {
                x: {
                    ticks: { color: textColorSecondary, font: { family: 'Cairo', size: 11 } },
                    grid: { color: surfaceBorder, drawBorder: false, borderDash: [4, 4] }
                },
                y: {
                    ticks: { color: textColor, font: { family: 'Cairo', size: 12, weight: 'bold' } },
                    grid: { display: false }
                }
            }
        };
      }
  }
}
