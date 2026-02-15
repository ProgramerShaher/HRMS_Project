import { Component, OnInit, signal, effect, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { Chart, registerables } from 'chart.js';
import { PayrollProcessingService } from '../../services/payroll-processing.service';
import { LoanService } from '../../services/loan.service';
import { SalaryStructureService } from '../../services/salary-structure.service';
import { PayrollDashboardStats } from '../../models/payroll-processing.models';
import { LoanSummary } from '../../models/loan.models';
import { SalaryBreakdown } from '../../models/salary-structure.models';
import { AuthService } from '../../../../core/auth/services/auth.service';

Chart.register(...registerables);

/**
 * لوحة تحكم الرواتب
 * Payroll Dashboard Component
 */
@Component({
  selector: 'app-payroll-dashboard',
  standalone: true,
  imports: [CommonModule, ButtonModule],
  templateUrl: './payroll-dashboard.component.html',
  styles: [`
    :host { display: block; }
    canvas { max-height: 300px; width: 100% !important; }
  `]
})
export class PayrollDashboardComponent implements OnInit, OnDestroy {
  private payrollService = inject(PayrollProcessingService);
  private loanService = inject(LoanService);
  private structureService = inject(SalaryStructureService);
  private authService = inject(AuthService);
  private router = inject(Router);

  stats = signal<PayrollDashboardStats | null>(null);
  loanSummary = signal<LoanSummary | null>(null);
  salaryBreakdown = signal<SalaryBreakdown | null>(null);
  loading = signal(false);

  // Luxury enhancements
  greeting = signal<string>('');
  currentTime = signal<string>(new Date().toLocaleTimeString('ar-YE', { hour: '2-digit', minute: '2-digit' }));

  private salaryChart: Chart | null = null;
  private loanChart: Chart | null = null;
  private trendChart: Chart | null = null;

  constructor() {
    this.updateGreeting();

    // Re-render charts when data updates
    effect(() => {
      const breakdown = this.salaryBreakdown();
      if (breakdown) {
        setTimeout(() => this.renderSalaryChart(breakdown), 0);
      }
    });

    effect(() => {
      const loans = this.loanSummary();
      if (loans && loans.activeLoans > 0) {
        setTimeout(() => this.renderLoanChart(loans), 0);
      }
    });

    effect(() => {
      if (this.stats()) {
        setTimeout(() => this.renderTrendChart(), 0);
      }
    });
  }

  ngOnInit() {
    this.loadDashboardData();
    setInterval(() => this.currentTime.set(new Date().toLocaleTimeString('ar-YE', { hour: '2-digit', minute: '2-digit' })), 60000);
  }

  ngOnDestroy() {
    [this.salaryChart, this.loanChart, this.trendChart].forEach(chart => chart?.destroy());
  }

  private updateGreeting() {
    const hour = new Date().getHours();
    if (hour < 12) this.greeting.set('صباح الخير');
    else if (hour < 17) this.greeting.set('طاب يومك');
    else this.greeting.set('مساء الخير');
  }

  loadDashboardData() {
    const employeeId = this.authService.currentUser()?.employeeId;
    if (!employeeId) return;

    this.loading.set(true);

    // Initial Stats Load
    this.payrollService.getDashboardStats(employeeId).subscribe({
      next: (stats) => {
        this.stats.set(stats);
        if (!this.salaryBreakdown()) this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading payroll stats:', err);
        this.loading.set(false);
      }
    });

    // Load loan summary
    this.loanService.getLoanSummary(employeeId).subscribe({
      next: (summary) => this.loanSummary.set(summary),
      error: (err) => console.error('Error loading loan summary:', err)
    });

    // Load salary breakdown
    this.structureService.getSalaryBreakdown(employeeId).subscribe({
      next: (breakdown) => {
        this.salaryBreakdown.set(breakdown);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading salary breakdown:', err);
        this.loading.set(false);
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', {
      style: 'decimal',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(value);
  }

  navigateToPayslips() { this.router.navigate(['/payroll/payslips']); }
  navigateToLoans() { this.router.navigate(['/payroll/loans/my-loans']); }
  navigateToSalary() { this.router.navigate(['/payroll/salary/my-structure']); }

  private renderSalaryChart(breakdown: SalaryBreakdown) {
    const ctx = document.getElementById('salaryDistributionChart') as HTMLCanvasElement;
    if (!ctx) return;
    if (this.salaryChart) this.salaryChart.destroy();

    const labels = ['الأساسي', ...breakdown.allowances.map(a => a.elementNameAr)];
    const data = [breakdown.basicSalary, ...breakdown.allowances.map(a => a.amount)];

    const gradient = ctx.getContext('2d')?.createLinearGradient(0, 0, 0, 400);
    gradient?.addColorStop(0, '#3b82f6');
    gradient?.addColorStop(1, '#1d4ed8');

    this.salaryChart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: labels,
        datasets: [{
          data: data,
          backgroundColor: ['#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ec4899', '#06b6d4'],
          hoverOffset: 15,
          borderWidth: 0,
          spacing: 10,
          borderRadius: 8
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: { position: 'bottom', labels: { usePointStyle: true, font: { family: 'Cairo', size: 12 } } },
          tooltip: {
            padding: 12,
            titleFont: { family: 'Cairo', size: 14 },
            bodyFont: { family: 'Cairo', size: 13 },
            callbacks: {
              label: (ctx) => `${ctx.label}: ${this.formatCurrency(ctx.parsed as number)} ريال`
            }
          }
        },
        cutout: '75%'
      }
    });
  }

  private renderLoanChart(summary: LoanSummary) {
    const ctx = document.getElementById('loanProgressChart') as HTMLCanvasElement;
    if (!ctx) return;
    if (this.loanChart) this.loanChart.destroy();

    const paidAmount = summary.totalAmount - summary.totalRemaining;

    this.loanChart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: ['مدفوع', 'متبقي'],
        datasets: [{
          data: [paidAmount, summary.totalRemaining],
          backgroundColor: ['#10b981', '#f1f5f9'],
          borderWidth: 0,
          borderRadius: 10,
          spacing: 5
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: { position: 'bottom', labels: { usePointStyle: true, font: { family: 'Cairo', size: 12 } } },
          tooltip: {
            callbacks: {
              label: (ctx) => `${ctx.label}: ${this.formatCurrency(ctx.parsed as number)} ريال`
            }
          }
        },
        cutout: '80%'
      }
    });
  }

  private renderTrendChart() {
    const ctx = document.getElementById('salaryTrendChart') as HTMLCanvasElement;
    if (!ctx) return;
    if (this.trendChart) this.trendChart.destroy();

    // Mock trend data for last 6 months
    const months = ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو'];
    const netSalaryTrend = [250000, 260000, 255000, 270000, 265000, this.stats()?.currentMonthNetSalary || 280000];

    this.trendChart = new Chart(ctx, {
      type: 'line',
      data: {
        labels: months,
        datasets: [{
          label: 'صافي الراتب',
          data: netSalaryTrend,
          borderColor: '#3b82f6',
          backgroundColor: 'rgba(59, 130, 246, 0.1)',
          fill: true,
          tension: 0.4,
          pointBackgroundColor: '#3b82f6',
          pointBorderColor: '#fff',
          pointBorderWidth: 2,
          pointRadius: 6,
          pointHoverRadius: 8
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: { display: false },
          tooltip: {
            mode: 'index',
            intersect: false,
            callbacks: {
              label: (ctx) => `الراتب: ${this.formatCurrency(ctx.parsed?.y ?? 0)} ريال`
            }
          }
        },
        scales: {
          y: { beginAtZero: false, grid: { display: false }, ticks: { font: { family: 'Cairo' } } },
          x: { grid: { display: false }, ticks: { font: { family: 'Cairo' } } }
        }
      }
    });
  }
}
