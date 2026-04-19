import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

// PrimeNG
import { ChartModule } from 'primeng/chart';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';

import { PerformanceService } from '../../services/performance.service';
import { EmployeeAppraisal } from '../../models/performance.model';

@Component({
  selector: 'app-performance-analytics',
  standalone: true,
  imports: [CommonModule, ChartModule, CardModule, TableModule, TagModule],
  templateUrl: './analytics.component.html'
})
export class PerformanceAnalyticsComponent implements OnInit {
  private performanceService = inject(PerformanceService);

  loading = signal(false);
  barData: any;
  barOptions: any;
  pieData: any;
  pieOptions: any;
  topPerformers = signal<EmployeeAppraisal[]>([]);

  ngOnInit() {
    this.initChartOptions();
    this.loadAnalyticsData();
  }

  initChartOptions() {
    this.barOptions = {
      plugins: {
        legend: { labels: { color: '#4b5563' } }
      },
      scales: {
        x: { ticks: { color: '#6b7280' }, grid: { color: '#f3f4f6' } },
        y: { ticks: { color: '#6b7280' }, grid: { color: '#f3f4f6' } }
      }
    };

    this.pieOptions = {
      plugins: {
        legend: { position: 'bottom', labels: { color: '#4b5563' } }
      }
    };
  }

  loadAnalyticsData() {
    this.loading.set(true);
    this.performanceService.getAppraisals().subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.succeeded) {
          this.processData(res.data);
        }
      },
      error: () => this.loading.set(false)
    });
  }

  processData(data: EmployeeAppraisal[]) {
    // 1. Bar Chart: Score Distribution (0-20, 20-40, 40-60, 60-80, 80-100)
    const ranges = [0, 0, 0, 0, 0];
    const completed = data.filter(a => a.status === 'COMPLETED');

    completed.forEach(a => {
      const s = a.finalScore || 0;
      if (s < 20) ranges[0]++;
      else if (s < 40) ranges[1]++;
      else if (s < 60) ranges[2]++;
      else if (s < 80) ranges[3]++;
      else ranges[4]++;
    });

    this.barData = {
      labels: ['0-20', '21-40', '41-60', '61-80', '81-100'],
      datasets: [
        {
          label: 'توزيع درجات الأداء',
          data: ranges,
          backgroundColor: ['#ef4444', '#f59e0b', '#3b82f6', '#10b981', '#059669']
        }
      ]
    };

    // 2. Pie Chart: Status Breakdown
    const statusCounts: { [key: string]: number } = {};
    data.forEach(a => {
      statusCounts[a.status] = (statusCounts[a.status] || 0) + 1;
    });

    this.pieData = {
      labels: Object.keys(statusCounts),
      datasets: [
        {
          data: Object.values(statusCounts),
          backgroundColor: ['#64748b', '#0ea5e9', '#f59e0b', '#ef4444', '#10b981']
        }
      ]
    };

    // 3. Top Performers
    this.topPerformers.set(
      [...completed].sort((a, b) => (b.finalScore || 0) - (a.finalScore || 0)).slice(0, 5)
    );
  }
}
