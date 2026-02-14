import { CommonModule } from '@angular/common';
import { PayrollRun, Payslip } from '../../../../models/payroll-processing.models';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { ChartModule } from 'primeng/chart';
import { TagModule } from 'primeng/tag';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';
import { Component, OnInit, signal } from '@angular/core';
import { PayrollProcessingService } from '../../../../services/payroll-processing.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-payroll-run-details',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    CardModule,
    ChartModule,
    TagModule,
    ActionButtonsComponent
  ],
  templateUrl: './payroll-run-details.component.html',
  styleUrl: './payroll-run-details.component.scss'
})
export class PayrollRunDetailsComponent implements OnInit {
  run = signal<PayrollRun | null>(null);
  payslips = signal<Payslip[]>([]);
  summary = signal<any>(null);
  loading = signal(false);
  runId = signal<number>(0);
  
  chartData: any;
  chartOptions: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private payrollService: PayrollProcessingService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = +params['id'];
      if (id > 0) {
        this.runId.set(id);
        this.loadDetails();
      }
    });
  }

  loadDetails() {
    this.loading.set(true);
    this.payrollService.getPayrunDetails(this.runId()).subscribe({
      next: (response) => {
        if (response.succeeded && response.data) {
          const data = response.data;
          this.run.set(data.run);
          this.payslips.set(data.payslips || []);
          this.summary.set(data.summary);
          this.updateChart(data.summary);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  updateChart(summary: any) {
    if (!summary) return;

    this.chartData = {
      labels: ['الأساسي', 'البدلات', 'الخصومات'],
      datasets: [{
        data: [summary.totalBasicSalaries, summary.totalAllowances, summary.totalDeductions],
        backgroundColor: [
          '#6366F1', // Indigo for Basic
          '#10B981', // Emerald for Allowances
          '#EF4444'  // Rose for Deductions
        ],
        hoverOffset: 20,
        borderWidth: 0
      }]
    };

    this.chartOptions = {
      cutout: '75%',
      plugins: {
        legend: {
          display: false
        },
        tooltip: {
          backgroundColor: 'rgba(15, 23, 42, 0.9)',
          padding: 12,
          titleFont: { family: 'Cairo', size: 14 },
          bodyFont: { family: 'Cairo', size: 13 },
          cornerRadius: 12
        }
      }
    };
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', { 
      style: 'currency', 
      currency: 'YER',
      maximumFractionDigits: 0 
    }).format(value);
  }

  getMonthName(month: number): string {
    const months = ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو',
                    'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'];
    return months[month - 1] || '';
  }

  getStatusSeverity(status: string | undefined): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    if (!status) return 'secondary';
    switch (status) {
      case 'COMPLETED': return 'success';
      case 'POSTED': return 'info';
      case 'APPROVED': return 'info';
      default: return 'warn';
    }
  }

  getStatusLabel(status: string | undefined): string {
    if (!status) return '-';
    switch (status) {
      case 'COMPLETED': return 'مكتمل';
      case 'POSTED': return 'تم الترحيل';
      case 'APPROVED': return 'معتمد';
      default: return status;
    }
  }

  exportPDF() { window.print(); }
}
