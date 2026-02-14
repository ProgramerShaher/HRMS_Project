import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { Chart, registerables } from 'chart.js';
import { SalaryStructureService } from '../../../services/salary-structure.service';
import { SalaryBreakdown } from '../../../models/salary-structure.models';
import { AuthService } from '../../../../../core/auth/services/auth.service';

Chart.register(...registerables);

@Component({
  selector: 'app-my-salary',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule],
  templateUrl: './my-salary.component.html',
  styleUrls: ['./my-salary.component.scss'],
  styles: [`
    canvas { max-height: 350px; width: 100% !important; }
  `]
})
export class MySalaryComponent implements OnInit {
  private structureService = inject(SalaryStructureService);
  private authService = inject(AuthService);
  private router = inject(Router);

  salaryBreakdown = signal<SalaryBreakdown | null>(null);
  loading = signal(false);
  private chart: Chart | null = null;

  ngOnInit() {
    this.loadSalaryStructure();
  }

  ngOnDestroy() {
    if (this.chart) {
      this.chart.destroy();
    }
  }

  loadSalaryStructure() {
    const employeeId = this.authService.currentUser()?.employeeId;
    if (!employeeId) return;

    this.loading.set(true);
    this.structureService.getSalaryBreakdown(employeeId).subscribe({
      next: (breakdown) => {
        this.salaryBreakdown.set(breakdown);
        setTimeout(() => this.renderChart(breakdown), 0);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading salary structure:', err);
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

  private renderChart(breakdown: SalaryBreakdown) {
    const ctx = document.getElementById('salaryChart') as HTMLCanvasElement;
    if (!ctx) return;

    if (this.chart) {
      this.chart.destroy();
    }

    const labels = ['الأساسي', ...breakdown.allowances.map(a => a.elementNameAr)];
    const data = [breakdown.basicSalary, ...breakdown.allowances.map(a => a.amount)];
    const colors = ['#3b82f6', '#10b981', '#f59e0b', '#8b5cf6', '#ec4899', '#06b6d4'];

    this.chart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: labels,
        datasets: [{
          data: data,
          backgroundColor: colors,
          hoverOffset: 20,
          borderWidth: 0,
          spacing: 8,
          borderRadius: 10
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'bottom',
            labels: {
              usePointStyle: true,
              padding: 25,
              font: { family: 'Cairo', size: 12 }
            }
          },
          tooltip: {
            padding: 12,
            titleFont: { family: 'Cairo', size: 14 },
            bodyFont: { family: 'Cairo', size: 13 },
            callbacks: {
              label: (context) => {
                const label = context.label || '';
                const value = this.formatCurrency(context.parsed as number);
                return `${label}: ${value} ريال`;
              }
            }
          }
        },
        cutout: '75%'
      }
    });
  }
}
