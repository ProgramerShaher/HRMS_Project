import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { Chart, registerables } from 'chart.js';
import { PayrollProcessingService } from '../../services/payroll-processing.service';
import { LoanService } from '../../services/loan.service';

Chart.register(...registerables);

@Component({
  selector: 'app-payroll-dashboard',
  standalone: true,
  imports: [CommonModule, ButtonModule, TableModule, TagModule, InputTextModule, FormsModule],
  templateUrl: './payroll-dashboard.component.html',
  styles: [`:host { display: block; } canvas { max-height: 300px; width: 100% !important; }`]
})
export class PayrollDashboardComponent implements OnInit, OnDestroy {
  private payrollService = inject(PayrollProcessingService);
  private loanService = inject(LoanService);
  private router = inject(Router);

  // State
  loading = signal(false);
  allEmployeesSalaries = signal<any[]>([]);
  filteredEmployees = signal<any[]>([]);
  searchTerm = '';

  // Summary KPIs
  totalGrossSalary = signal(0);
  totalNetSalary = signal(0);
  totalDeductions = signal(0);
  totalActiveLoans = signal(0);
  employeesWithSalary = signal(0);
  employeesWithoutSalary = signal(0);

  // Charts
  private deptChart: Chart | null = null;
  private salaryRangeChart: Chart | null = null;

  // Time
  greeting = signal('');
  currentTime = signal(new Date().toLocaleTimeString('ar-YE', { hour: '2-digit', minute: '2-digit' }));

  ngOnInit() {
    this.updateGreeting();
    setInterval(() => this.currentTime.set(new Date().toLocaleTimeString('ar-YE', { hour: '2-digit', minute: '2-digit' })), 60000);
    this.loadData();
  }

  ngOnDestroy() {
    this.deptChart?.destroy();
    this.salaryRangeChart?.destroy();
  }

  private updateGreeting() {
    const hour = new Date().getHours();
    if (hour < 12) this.greeting.set('صباح الخير');
    else if (hour < 17) this.greeting.set('طاب نهارك');
    else this.greeting.set('مساء الخير');
  }

  loadData() {
    this.loading.set(true);
    this.payrollService.getAllEmployeesSalaries().subscribe({
      next: (res: any) => {
        const data: any[] = res.data || res;
        this.allEmployeesSalaries.set(data);
        this.filteredEmployees.set(data);
        this.calculateKPIs(data);
        setTimeout(() => {
          this.renderDeptChart(data);
          this.renderSalaryRangeChart(data);
        }, 100);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  filterEmployees() {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredEmployees.set(this.allEmployeesSalaries());
    } else {
      this.filteredEmployees.set(
        this.allEmployeesSalaries().filter(e =>
          e.employeeNameAr?.toLowerCase().includes(term) ||
          e.employeeCode?.toLowerCase().includes(term) ||
          e.departmentName?.toLowerCase().includes(term)
        )
      );
    }
  }

  private calculateKPIs(data: any[]) {
    const withSalary = data.filter(e => e.hasSalaryStructure);
    const withoutSalary = data.filter(e => !e.hasSalaryStructure);
    this.employeesWithSalary.set(withSalary.length);
    this.employeesWithoutSalary.set(withoutSalary.length);
    this.totalGrossSalary.set(data.reduce((sum, e) => sum + (e.grossSalary || 0), 0));
    this.totalNetSalary.set(data.reduce((sum, e) => sum + (e.netSalary || 0), 0));
    this.totalDeductions.set(data.reduce((sum, e) => sum + (e.totalDeductions || 0), 0));
    this.totalActiveLoans.set(data.reduce((sum, e) => sum + (e.activeLoansCount || 0), 0));
  }

  private renderDeptChart(data: any[]) {
    const ctx = document.getElementById('deptChart') as HTMLCanvasElement;
    if (!ctx) return;
    this.deptChart?.destroy();

    const deptMap: Record<string, number> = {};
    data.forEach(e => {
      const dept = e.departmentName || 'غير محدد';
      deptMap[dept] = (deptMap[dept] || 0) + (e.grossSalary || 0);
    });
    const labels = Object.keys(deptMap);
    const values = Object.values(deptMap);

    this.deptChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'إجمالي الرواتب',
          data: values,
          backgroundColor: ['#3b82f6','#10b981','#f59e0b','#8b5cf6','#ec4899','#06b6d4','#f97316','#14b8a6'],
          borderRadius: 8,
          borderSkipped: false
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: { display: false },
          tooltip: {
            callbacks: { label: (c) => `${this.formatCurrency(c.parsed.y ?? 0)} ريال` }
          }
        },
        scales: {
          y: { beginAtZero: true, grid: { display: false }, ticks: { font: { family: 'Cairo' }, callback: (v) => `${this.formatCurrency(+v)}` } },
          x: { grid: { display: false }, ticks: { font: { family: 'Cairo' } } }
        }
      }
    });
  }

  private renderSalaryRangeChart(data: any[]) {
    const ctx = document.getElementById('salaryRangeChart') as HTMLCanvasElement;
    if (!ctx) return;
    this.salaryRangeChart?.destroy();

    const ranges = { 'أقل من 500k': 0, '500k - 1M': 0, '1M - 2M': 0, 'أكثر من 2M': 0 };
    data.forEach(e => {
      const net = e.netSalary || 0;
      if (net < 500000) ranges['أقل من 500k']++;
      else if (net < 1000000) ranges['500k - 1M']++;
      else if (net < 2000000) ranges['1M - 2M']++;
      else ranges['أكثر من 2M']++;
    });

    this.salaryRangeChart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: Object.keys(ranges),
        datasets: [{
          data: Object.values(ranges),
          backgroundColor: ['#3b82f6','#10b981','#f59e0b','#8b5cf6'],
          borderWidth: 0,
          hoverOffset: 12,
          borderRadius: 6,
          spacing: 4
        }]
      },
      options: {
        responsive: true,
        cutout: '70%',
        plugins: {
          legend: { position: 'bottom', labels: { usePointStyle: true, font: { family: 'Cairo', size: 12 } } }
        }
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-EG', { minimumFractionDigits: 0, maximumFractionDigits: 0 }).format(value);
  }

  getStatusSeverity(employee: any): string {
    if (!employee.hasSalaryStructure) return 'danger';
    if ((employee.netSalary || 0) < 0) return 'danger';
    return 'success';
  }

  navigateToAdmin() { this.router.navigate(['/payroll/admin']); }
  navigateToLoans() { this.router.navigate(['/payroll/loans']); }
  navigateToPayslips() { this.router.navigate(['/payroll/payslips']); }
}
