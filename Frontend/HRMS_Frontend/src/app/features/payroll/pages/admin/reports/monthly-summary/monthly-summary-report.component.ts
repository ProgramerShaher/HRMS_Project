import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';
import { PayrollProcessingService } from '../../../../services/payroll-processing.service';
import { DepartmentService } from '../../../../../setup/services/department.service';
import { forkJoin } from 'rxjs';

interface DepartmentSummary {
  departmentId: number;
  departmentName: string;
  employeeCount: number;
  totalBasicSalaries: number;
  totalAllowances: number;
  totalDeductions: number;
  totalNetSalaries: number;
}

interface MonthlySummary {
  month: number;
  year: number;
  totalEmployees: number;
  totalBasicSalaries: number;
  totalAllowances: number;
  totalDeductions: number;
  totalOvertimePayments: number;
  totalNetSalaries: number;
  departmentBreakdown: DepartmentSummary[];
}

@Component({
  selector: 'app-monthly-summary-report',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    ChartModule, 
    TableModule, 
    ButtonModule, 
    SelectModule, 
    CardModule, 
    TagModule,
    TooltipModule,
    ActionButtonsComponent
  ],
  templateUrl: './monthly-summary-report.component.html',
  styleUrl: './monthly-summary-report.component.scss'
})
export class MonthlySummaryReportComponent implements OnInit {
  private payrollService = inject(PayrollProcessingService);
  private deptService = inject(DepartmentService);

  loading = signal(false);
  reportData = signal<MonthlySummary | null>(null);
  departments = signal<any[]>([]);
  
  selectedMonth = signal<number>(new Date().getMonth() + 1);
  selectedYear = signal<number>(new Date().getFullYear());
  selectedDept = signal<number | null>(null);

  months = [
    { label: 'يناير', value: 1 }, { label: 'فبراير', value: 2 }, { label: 'مارس', value: 3 },
    { label: 'أبريل', value: 4 }, { label: 'مايو', value: 5 }, { label: 'يونيو', value: 6 },
    { label: 'يوليو', value: 7 }, { label: 'أغسطس', value: 8 }, { label: 'سبتمبر', value: 9 },
    { label: 'أكتوبر', value: 10 }, { label: 'نوفمبر', value: 11 }, { label: 'ديسمبر', value: 12 }
  ];

  years = Array.from({ length: 5 }, (_, i) => new Date().getFullYear() - i);

  chartData: any;
  chartOptions: any;

  ngOnInit() {
    this.loadInitialData();
  }

  loadInitialData() {
    this.loading.set(true);
    this.deptService.getAll().subscribe({
      next: (data: any) => {
        this.departments.set(data || []);
        this.loadReport();
      },
      error: () => this.loading.set(false)
    });
  }

  loadReport() {
    this.loading.set(true);
    this.payrollService.getMonthlySummary(
      this.selectedMonth(),
      this.selectedYear(),
      this.selectedDept() || undefined
    ).subscribe({
      next: (response: any) => {
        if (response.succeeded && response.data) {
          this.reportData.set(response.data);
          this.updateChart(response.data);
        }
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading report:', err);
        this.loading.set(false);
      }
    });
  }

  updateChart(data: MonthlySummary) {
    const labels = data.departmentBreakdown.map(d => d.departmentName);
    const netSalaries = data.departmentBreakdown.map(d => d.totalNetSalaries);
    const employeeCounts = data.departmentBreakdown.map(d => d.employeeCount);

    this.chartData = {
      labels: labels,
      datasets: [
        {
          type: 'bar',
          label: 'إجمالي صافي الرواتب',
          backgroundColor: '#3B82F6',
          hoverBackgroundColor: '#2563EB',
          data: netSalaries,
          borderColor: 'white',
          borderWidth: 2,
          borderRadius: 12,
          yAxisID: 'y',
          barPercentage: 0.6
        },
        {
          type: 'line',
          label: 'عدد الموظفين',
          borderColor: '#EC4899',
          borderWidth: 4,
          fill: false,
          tension: 0.5,
          data: employeeCounts,
          yAxisID: 'y1',
          pointBackgroundColor: '#EC4899',
          pointBorderColor: 'white',
          pointBorderWidth: 2,
          pointRadius: 6,
          pointHoverRadius: 8
        }
      ]
    };

    this.chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false // سنستخدم Legend مخصص في الـ HTML
        },
        tooltip: {
          backgroundColor: 'rgba(15, 23, 42, 0.9)',
          padding: 12,
          titleFont: { family: 'Cairo', size: 14, weight: 'bold' },
          bodyFont: { family: 'Cairo', size: 13 },
          cornerRadius: 12,
          displayColors: true
        }
      },
      scales: {
        x: {
          ticks: { 
            color: '#94a3b8', 
            font: { family: 'Cairo', size: 11, weight: 'bold' } 
          },
          grid: { display: false }
        },
        y: {
          type: 'linear',
          display: true,
          position: 'right', // ليتناسب مع الـ RTL بشكل أفضل
          ticks: { 
            color: '#3B82F6', 
            font: { family: 'Cairo', size: 10 },
            callback: (value: any) => this.formatCurrency(value)
          },
          grid: { color: 'rgba(226, 232, 240, 0.5)' }
        },
        y1: {
          type: 'linear',
          display: true,
          position: 'left',
          grid: { drawOnChartArea: false },
          ticks: { 
            color: '#EC4899', 
            font: { family: 'Cairo', size: 10, weight: 'bold' } 
          }
        }
      }
    };
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', {
      style: 'decimal',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(value);
  }

  exportPDF() {
    window.print();
  }
}
