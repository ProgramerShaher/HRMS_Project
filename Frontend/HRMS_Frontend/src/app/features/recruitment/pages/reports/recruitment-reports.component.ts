import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/chart';
import { RecruitmentService } from '../../services/recruitment.service';
import { RecruitmentReports, ChartItem } from '../../models/recruitment.models';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-recruitment-reports',
  standalone: true,
  imports: [CommonModule, ChartModule, ToastModule],
  providers: [MessageService],
  templateUrl: './recruitment-reports.component.html'
})
export class RecruitmentReportsComponent implements OnInit {
  private recruitmentService = inject(RecruitmentService);
  private messageService = inject(MessageService);

  reports = signal<RecruitmentReports | null>(null);
  loading = signal<boolean>(true);

  // Chart Data
  statusChartData: any;
  sourceChartData: any;
  deptChartData: any;
  pipelineChartData: any;

  chartOptions: any;

  ngOnInit() {
    this.initChartOptions();
    this.loadReports();
  }

  loadReports() {
    this.loading.set(true);
    this.recruitmentService.getStats().subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.reports.set(res.data);
          this.prepareChartData(res.data);
        } else {
          this.messageService.add({ severity: 'error', summary: 'خطأ', detail: res.message });
        }
        this.loading.set(false);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل التقارير' });
        this.loading.set(false);
      }
    });
  }

  initChartOptions() {
    this.chartOptions = {
      plugins: {
        legend: {
          position: 'bottom',
          labels: {
            color: '#495057',
            font: { family: 'Cairo', size: 12 }
          }
        }
      },
      maintainAspectRatio: false,
      scales: {
        x: { ticks: { color: '#495057' }, grid: { color: '#ebedef' } },
        y: { ticks: { color: '#495057' }, grid: { color: '#ebedef' } }
      }
    };
  }

  prepareChartData(data: RecruitmentReports) {
    // 1. Status Pie Chart
    this.statusChartData = {
      labels: data.candidatesByStatus.map(x => x.label),
      datasets: [{
        data: data.candidatesByStatus.map(x => x.value),
        backgroundColor: ['#4F46E5', '#10B981', '#F59E0B', '#EF4444', '#6366F1', '#8B5CF6'],
        hoverBackgroundColor: ['#4338CA', '#059669', '#D97706', '#DC2626', '#4F46E5', '#7C3AED']
      }]
    };

    // 2. Source Doughnut Chart
    this.sourceChartData = {
      labels: data.candidatesBySource.map(x => x.label),
      datasets: [{
        data: data.candidatesBySource.map(x => x.value),
        backgroundColor: ['#3B82F6', '#10B981', '#6366F1', '#F43F5E', '#F59E0B'],
        innerWidth: '60%'
      }]
    };

    // 3. Department Bar Chart
    this.deptChartData = {
      labels: data.vacanciesByDepartment.map(x => x.label),
      datasets: [{
        label: 'الوظائف الشاغرة',
        backgroundColor: '#7C3AED',
        data: data.vacanciesByDepartment.map(x => x.value),
        borderRadius: 8
      }]
    };

    // 4. Pipeline Horizontal Bar Chart
    this.pipelineChartData = {
      labels: data.applicationPipeline.map(x => x.label),
      datasets: [{
        label: 'عدد الطلبات',
        backgroundColor: '#10B981',
        data: data.applicationPipeline.map(x => x.value),
        borderRadius: 8
      }]
    };
  }
}
