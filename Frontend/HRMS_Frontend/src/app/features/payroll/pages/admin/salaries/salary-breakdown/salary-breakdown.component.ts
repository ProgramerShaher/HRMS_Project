import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ChartModule } from 'primeng/chart';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';
import { PayrollProcessingService } from '../../../../services/payroll-processing.service';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

interface SalaryBreakdown {
  employee: {
    employeeId: number;
    employeeCode: string;
    employeeNameAr: string;
    departmentName?: string;
    jobTitle?: string;
    email?: string;
    hireDate?: string;
  };
  basicSalary: number;
  earnings: ElementBreakdown[];
  deductions: ElementBreakdown[];
  attendance: {
    month: number;
    year: number;
    totalWorkingDays: number;
    actualWorkingDays: number;
    absenceDays: number;
    absenceDeduction: number;
    lateMinutes: number;
    latenessDeduction: number;
    overtimeHours: number;
    overtimeAmount: number;
  };
  activeLoans: LoanDeduction[];
  summary: {
    grossSalary: number;
    netSalary: number;
    totalEarnings: number;
    totalDeductions: number;
    structureDeductions: number;
    loanDeductions: number;
    attendanceDeductions: number;
    otherDeductions: number;
  };
}

interface ElementBreakdown {
  elementId: number;
  elementNameAr: string;
  elementType: string;
  amount: number;
  percentage?: number;
  isTaxable?: boolean;
}

interface LoanDeduction {
  loanId: number;
  loanAmount: number;
  monthlyInstallment: number;
  remainingAmount: number;
  installmentNumber: number;
}

/**
 * تفاصيل راتب موظف - واجهة إدارية
 * Employee Salary Breakdown - Admin Interface
 */
@Component({
  selector: 'app-salary-breakdown',
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    ButtonModule,
    TableModule,
    TagModule,
    ChartModule,
    ActionButtonsComponent
  ],
  templateUrl: './salary-breakdown.component.html',
  styleUrl: './salary-breakdown.component.scss'
})
export class SalaryBreakdownComponent implements OnInit {
  breakdown = signal<SalaryBreakdown | null>(null);
  loading = signal(false);
  employeeId = signal<number>(0);

  // Chart data
  chartData: any;
  chartOptions: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private payrollService: PayrollProcessingService
  ) {
    this.initializeChartOptions();
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = +params['id'];
      if (id > 0) {
        this.employeeId.set(id);
        this.loadBreakdown(id);
      }
    });
  }

  loadBreakdown(employeeId: number) {
    this.loading.set(true);
    
    // استخدام الشهر والسنة الحاليين كقيم افتراضية
    const now = new Date();
    const currentMonth = now.getMonth() + 1;
    const currentYear = now.getFullYear();

    this.payrollService.getEmployeeSalaryBreakdown(employeeId, currentMonth, currentYear).subscribe({
      next: (response: any) => {
        if (response.succeeded && response.data) {
          this.breakdown.set(response.data);
          this.updateChart(response.data);
        }
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading breakdown:', err);
        this.loading.set(false);
      }
    });
  }

  initializeChartOptions() {
    this.chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom',
          labels: {
            usePointStyle: true,
            padding: 15,
            font: {
              family: 'Cairo',
              size: 12
            }
          }
        },
        tooltip: {
          callbacks: {
            label: (context: any) => {
              const label = context.label || '';
              const value = this.formatCurrency(context.parsed);
              return `${label}: ${value} ريال`;
            }
          }
        }
      }
    };
  }

  updateChart(data: SalaryBreakdown) {
    const labels = ['الراتب الأساسي', ...data.earnings.map(a => a.elementNameAr)];
    const values = [data.basicSalary, ...data.earnings.map(a => a.amount)];
    const colors = [
      '#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', 
      '#ec4899', '#06b6d4', '#14b8a6', '#f97316'
    ];

    this.chartData = {
      labels: labels,
      datasets: [{
        data: values,
        backgroundColor: colors.slice(0, values.length),
        borderWidth: 0
      }]
    };
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', {
      style: 'decimal',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(value);
  }

  printBreakdown() {
    window.print();
  }

  goBack() {
    this.router.navigate(['/payroll/salaries/all']);
  }
}
